using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Windows;
using Winfy.ViewModels;
using System.Windows.Interop;
using Winfy.Core;

namespace Winfy {
    public sealed class AppWindowManager : WindowManager {

        [StructLayout(LayoutKind.Sequential)]
        public struct Margins {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }
        [DllImport("dwmapi.dll", PreserveSig = true)]
        static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        [DllImport("dwmapi.dll")]
        static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMarInset);

        private readonly AppSettings _Settings;

        public AppWindowManager(AppSettings settings) {
            _Settings = settings;
        }

        protected override Window CreateWindow(object rootModel, bool isDialog, object context, IDictionary<string, object> settings) {
            var wnd = base.CreateWindow(rootModel, isDialog, context, settings);
            wnd.Topmost = _Settings.AlwaysOnTop;
            wnd.SizeToContent = SizeToContent.WidthAndHeight;
            wnd.ResizeMode = ResizeMode.NoResize;
            wnd.Icon = GetImageSourceFromResource("App.ico");
            if(rootModel is ShellViewModel)
                SetupShell(wnd);

            return wnd;
        }

        private void SetupShell(Window window) {
            //Source: http://code-inside.de/blog/2012/11/11/howto-rahmenlose-wpf-apps-mit-schattenwurf/
            window.WindowStyle = WindowStyle.None;
            window.ResizeMode = ResizeMode.NoResize;
            window.ShowInTaskbar = false;
            window.SourceInitialized += (o, e) => {
                                            var helper = new WindowInteropHelper(window);
                                            var val = 2;
                                            DwmSetWindowAttribute(helper.Handle, 2, ref val, 4);
                                            var m = new Margins { bottomHeight = -1, leftWidth = -1, rightWidth = -1, topHeight = -1 };
                                            DwmExtendFrameIntoClientArea(helper.Handle, ref m);
                                            IntPtr hwnd = new WindowInteropHelper(window).Handle;
                                        };
            window.MouseLeftButtonDown += (o, e) => window.DragMove();
        }

        //TODO: Move me to a better accessible place 
        private ImageSource GetImageSourceFromResource(string psResourceName) {
            var oUri = new Uri("pack://application:,,,/Winfy;component/" + psResourceName, UriKind.RelativeOrAbsolute);
            return BitmapFrame.Create(oUri);
        }
    }
}
