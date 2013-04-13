using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Net;
using System.Windows;
using System.Diagnostics;

namespace Winfy.Core {
    public static class Helper {

        public static void OpenUrl(string url) {
            try {
                Process.Start(url);
            }
            catch (Exception) {
                MessageBox.Show(string.Format("Failed to open your default browser. Winfy tried to open the following url for you: {0}", url),
                    "Winfy", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static bool IsDWMSupported {
            get { return Environment.OSVersion.Version.Major >= 6; }
        }
        public static bool IsWindows7 {
            get { return Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1; }
        }

        public static string MakeNiceSize(double size) {
            return MakeNiceSize(size, "auto");
        }

        public static HttpWebRequest CreateWebRequest(string url) {
            var request = (HttpWebRequest) WebRequest.Create(url);
            if (request.Proxy != null) //Fix for NTLM secured proxies (hi ISA)
                request.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            return request;
        }

        private static string MakeNiceSize(double size, string mode) {
            var suffix = new[] { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            var run = 0;

            if (mode == "auto") {
                while (size >= 1024) {
                    size /= 1024;
                    run++;
                }
            }
            else if (mode != "auto") {
                if (suffix.Contains(mode)) {
                    while (suffix[run] != mode) {
                        size /= 1024;
                        run++;
                    }
                }
                else {
                    return "ERROR: Unknown mode";
                }

            }
            return Math.Round(size, 2).ToString("0.00") + " " + suffix[run];
        }

        public static ImageSource GetImageSourceFromResource(string psResourceName) {
            try {
                var oUri = new Uri("pack://application:,,,/Winfy;component/" + psResourceName, UriKind.RelativeOrAbsolute);
                return BitmapFrame.Create(oUri);
            }
            catch (FileFormatException) { return null; }
        }

    }
}
