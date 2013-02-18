using System;

namespace Winfy.Core.Deployment {
    public sealed class NoDeployment : IDeployment {
        public event EventHandler<UpdateReadyEventArgs> UpdateReady;

        private void OnUpdateReady(UpdateReadyEventArgs e) {
            var handler = UpdateReady;
            if (handler != null) handler(this, e);
        }

        public void Update() {
        }

        public void Restart() {
        }
    }
}
