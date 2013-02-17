using System;
using System.Deployment.Application;
using System.IO;
using System.Reflection;

namespace Winfy.Core {
    public sealed class AppContracts {
        public string ApplicationName {
            get { return "Winfy"; }
        }

        public Version ApplicationVersion {
            get {
                try {
                    return ApplicationDeployment.CurrentDeployment.CurrentVersion;
                }
                catch {
                    return Assembly.GetExecutingAssembly().GetName().Version;
                }
            }
        }

        public string SettingsFilename {
            get { return string.Format("{0}.Settings.json", ApplicationName); }
        }

        public string LastFmApiKey {
            get { return "d6177d680cea0ec4c248b3cc4897047a"; }
        }

        public string HomepageUrl {
            get { return "http://krausshq.com"; }
        }

        public string SettingsLocation {
            get {
                var location = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);
                if (!Directory.Exists(location))
                    Directory.CreateDirectory(location);
                return location;
            }
        }
    }
}