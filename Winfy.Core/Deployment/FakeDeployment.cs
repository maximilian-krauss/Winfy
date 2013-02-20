using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using NLog;

namespace Winfy.Core.Deployment {
    public sealed class FakeDeployment : AppDeployment {
        
        public FakeDeployment(Logger logger) : base(logger) {
        }

        public override void Update() {
            var dummy = new BackgroundWorker();
            dummy.DoWork += (o, e) => Thread.Sleep(5000);
            dummy.RunWorkerCompleted += (o, e) => {
                                            IsRequired = false;
                                            NewVersion = new Version(Assembly.GetExecutingAssembly().GetName().Version.Major + 1, 0, 0, 0);
                                            DownloadChangelog();
                                        };
            dummy.RunWorkerAsync();
        }

        public override void Restart() {
            var location = Application.ResourceAssembly.Location;
            Process.Start(location);
            Application.Current.Shutdown();
        }
    }
}
