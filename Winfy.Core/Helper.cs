using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace Winfy.Core {
    public static class Helper {

        public static bool IsDWMSupported {
            get { return Environment.OSVersion.Version.Major >= 6; }
        }

        /// <summary>Calculates a good Looking file size</summary>
        /// <param name="size">Your size in Bytes</param>
        /// <returns>String, value not greater 1024, with unit</returns>
        public static string MakeNiceSize(double size) {
            return MakeNiceSize(size, "auto");
        }

        /// <summary>Calculates a good Looking file size</summary>
        /// <param name="size">Your size in Bytes</param>
        /// <param name="mode">Any of "auto","B","KB","MB","GB","TB","PB","EB"</param>
        /// <returns>String, value with unit</returns>
        public static string MakeNiceSize(double size, string mode) {
            var Suffix = new[] { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            int run = 0;

            if (mode == "auto") {
                while (size >= 1024) {
                    size /= 1024;
                    run++;
                }
            }
            else if (mode != "auto") {
                if (Suffix.Contains(mode)) {
                    while (Suffix[run] != mode) {
                        size /= 1024;
                        run++;
                    }
                }
                else {
                    return "ERROR: Unknown mode";
                }

            }
            return Math.Round(size, 2).ToString("0.00") + " " + Suffix[run];
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
