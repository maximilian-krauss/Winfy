using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;

namespace Winfy.Core.Deployment {
    public sealed class FakeDeployment : IDeployment {
        public event EventHandler<UpdateReadyEventArgs> UpdateReady;

        private void OnUpdateReady(UpdateReadyEventArgs e) {
            var handler = UpdateReady;
            if (handler != null) handler(this, e);
        }

        public void Update() {
            var dummy = new BackgroundWorker();
            dummy.DoWork += (o, e) => Thread.Sleep(5000);
            dummy.RunWorkerCompleted += (o, e) =>
                                        OnUpdateReady(new UpdateReadyEventArgs {
                                                                                   IsRequired = false,
                                                                                   NewVersion = new Version(Assembly.GetExecutingAssembly()
                                                                                               .GetName().Version.Major + 1, 0, 0, 0)
                                                                               });
            dummy.RunWorkerAsync();
        }

        public void Restart() {
            var location = Application.ResourceAssembly.Location;
            Process.Start(location);
            Application.Current.Shutdown();
        }
    }
}
