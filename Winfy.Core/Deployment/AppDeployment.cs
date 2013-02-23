using System.Deployment.Application;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using NLog;
using System;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Winfy.Core.Deployment {
    public class AppDeployment : IDeployment {

        #region Native stuff

        [DllImport("clr.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        private static extern void CorLaunchApplication(uint hostType, string applicationFullName, int manifestPathsCount, string[] manifestPaths, int activationDataCount, string[] activationData, PROCESS_INFORMATION processInformation);

        [StructLayout(LayoutKind.Sequential), SuppressUnmanagedCodeSecurity]
        private class PROCESS_INFORMATION {
            public IntPtr hProcess = IntPtr.Zero;
            public IntPtr hThread = IntPtr.Zero;
            public int dwProcessId = 0;
            public int dwThreadId = 0;
            private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
            ~PROCESS_INFORMATION() {
                Close();
            }

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
            private static extern bool CloseHandle(HandleRef handle);

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            private void Close() {
                if ((this.hProcess != IntPtr.Zero) && (this.hProcess != INVALID_HANDLE_VALUE)) {
                    CloseHandle(new HandleRef(this, this.hProcess));
                    hProcess = INVALID_HANDLE_VALUE;
                }
                if ((this.hThread != IntPtr.Zero) && (this.hThread != INVALID_HANDLE_VALUE)) {
                    CloseHandle(new HandleRef(this, this.hThread));
                    hThread = INVALID_HANDLE_VALUE;
                }
            }
        }

        #endregion

        public event EventHandler<UpdateReadyEventArgs> UpdateReady;

        private readonly Logger _Logger;
        private readonly ApplicationDeployment _Deployment;
        private const string ChangelogLocation = "http://deploy.krausshq.com/winfy/changelog.json";

        public AppDeployment(Logger logger) {
            _Logger = logger;
            if (ApplicationDeployment.IsNetworkDeployed) {
                _Deployment = ApplicationDeployment.CurrentDeployment;
                _Deployment.CheckForUpdateCompleted += DeploymentCheckForUpdateCompleted;
                _Deployment.UpdateCompleted += DeploymentUpdateCompleted;
            }
        }

        protected Version NewVersion { get; set; }
        protected bool IsRequired { get; set; }
        protected List<Release> Changelog { get; set; }

       public virtual void Update() {
            try {
                _Deployment.CheckForUpdateAsync();
            }
            catch (Exception exc) {
                _Logger.WarnException("Update failed!", exc);
            }
        }

        public virtual void Restart() {
            var updatedApplicationFullName = ApplicationDeployment.CurrentDeployment.UpdatedApplicationFullName;
            CorLaunchApplication(2, updatedApplicationFullName, 0, null, 0, null, new PROCESS_INFORMATION());
            Application.Current.Shutdown();
        }

        void DeploymentCheckForUpdateCompleted(object sender, CheckForUpdateCompletedEventArgs e) {
            if (e.Error != null) {
                _Logger.WarnException("Check for updates failed!", e.Error);
                return;
            }

            if (!e.UpdateAvailable)
                return;

            NewVersion = e.AvailableVersion;
            IsRequired = e.IsUpdateRequired;

            _Logger.Info(string.Format("A new update is available! Old version: {0}, new version: {1}, mandatory: {2}",
                                       _Deployment.CurrentVersion, NewVersion, IsRequired));

            try {
                _Deployment.UpdateAsync();
            }
            catch (Exception exc) {
                _Logger.WarnException("Update failed!", exc);
            }
        }

        void DeploymentUpdateCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e) {
            if (e.Error != null) {
                _Logger.WarnException("Failed to apply update", e.Error);
                return;
            }
            DownloadChangelog();
        }

        protected virtual void DownloadChangelog() {
            try {
                var request = Helper.CreateWebRequest(ChangelogLocation);
                var response = request.GetResponse();
                using (var responseStream = response.GetResponseStream())
                    Changelog = Serializer.DeserializeFromJson<List<Release>>(responseStream);

                response.Close();
            }
            catch (Exception exc) {
                _Logger.WarnException("Failed to get the changelog :-(", exc);
                Changelog = new List<Release>();
            }
            finally {
                OnUpdateReady(new UpdateReadyEventArgs {
                    NewVersion = NewVersion,
                    IsRequired = IsRequired,
                    Changelog = Changelog
                });                
            }
        }

        protected void OnUpdateReady(UpdateReadyEventArgs e) {
            var handler = UpdateReady;
            if (handler != null) handler(this, e);
        }
    }
}
