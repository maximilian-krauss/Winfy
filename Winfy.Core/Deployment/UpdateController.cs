using NLog;
using System;
using System.Deployment.Application;
using System.Timers;

namespace Winfy.Core.Deployment {
    public class UpdateController : IUpdateController {
        public event EventHandler<UpdateReadyEventArgs> UpdateReady;

        private readonly Logger _Logger;
        private readonly IDeployment _Deployment;
        private readonly Timer _UpdateTimer;
        private readonly TimeSpan _UpdateCheckInterval;

        private DateTime _NextUpdateCheck;
        private bool _UpdateCheckIsBusy;

        public UpdateController(Logger logger) {
            _Logger = logger;
            _UpdateCheckInterval = new TimeSpan(0, 1, 0, 0); //Check for updates every hour, because quick deploy is nice
            _Deployment = ApplicationDeployment.IsNetworkDeployed
                              ? (IDeployment) new AppDeployment(_Logger)
                              : (IDeployment) new NoDeployment();

            _UpdateTimer = new Timer(1000) { AutoReset = true, Enabled = false };
            _UpdateTimer.Elapsed += UpdateTimerElapsed;
            _NextUpdateCheck = DateTime.Now.AddMinutes(1);
            _UpdateCheckIsBusy = false;
            _Deployment.UpdateReady += (o, e) => OnUpdateReady(e);
        }

        void UpdateTimerElapsed(object sender, ElapsedEventArgs e) {
            if (DateTime.Now < _NextUpdateCheck || _UpdateCheckIsBusy)
                return;

            try {
                _UpdateCheckIsBusy = true;
                _Deployment.Update();
            }
            catch (Exception exc) {
                _Logger.WarnException("Check for updates failed", exc);
            }
            finally {
                _UpdateCheckIsBusy = false;
                _NextUpdateCheck = DateTime.Now.Add(_UpdateCheckInterval);
            }
        }

        public void StartBackgroundCheck() {
            _UpdateTimer.Start();
        }

        public void StopBackgroundCheck() {
            if(_UpdateTimer != null)
                _UpdateTimer.Stop();
        }

        public void Restart() {
            _Deployment.Restart();
        }

        protected virtual void OnUpdateReady(UpdateReadyEventArgs e) {
            var handler = UpdateReady;
            if (handler != null) handler(this, e);
        }
    }
}