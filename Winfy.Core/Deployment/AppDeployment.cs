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

namespace Winfy.Core.Deployment {
    public sealed class AppDeployment : IDeployment {
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

        public event EventHandler<UpdateReadyEventArgs> UpdateReady;

        private readonly Logger _Logger;
        private readonly ApplicationDeployment _Deployment;

        private Version _NewVersion;
        private bool _IsRequired;

        public AppDeployment(Logger logger) {
            _Logger = logger;
            _Deployment = ApplicationDeployment.CurrentDeployment;
            _Deployment.CheckForUpdateCompleted += DeploymentCheckForUpdateCompleted;
            _Deployment.UpdateCompleted += DeploymentUpdateCompleted;
        }

       public void Update() {
            try {
                _Deployment.CheckForUpdateAsync();
            }
            catch (Exception exc) {
                _Logger.WarnException("Update failed!", exc);
            }
        }

        public void Restart() {
            var location = Application.ResourceAssembly.Location;
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

            _NewVersion = e.AvailableVersion;
            _IsRequired = e.IsUpdateRequired;

            _Logger.Info(string.Format("A new update is available! Old version: {0}, new version: {1}, mandatory: {2}",
                                       _Deployment.CurrentVersion, _NewVersion, _IsRequired));

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
            OnUpdateReady(new UpdateReadyEventArgs {
                                                       NewVersion = _NewVersion,
                                                       IsRequired = _IsRequired
                                                   });
        }

        private void OnUpdateReady(UpdateReadyEventArgs e) {
            var handler = UpdateReady;
            if (handler != null) handler(this, e);
        }
    }
}
