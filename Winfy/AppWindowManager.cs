using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using Winfy.ViewModels;
using System.Windows.Interop;
using Winfy.Core;
using Winfy.Core.Extensions;

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
            wnd.Icon = Helper.GetImageSourceFromResource("App.ico");
            TrackLocation(wnd, rootModel);
            if(rootModel is ShellViewModel)
                SetupShell(wnd);

            var canToggleVisibility = (rootModel as IToggleVisibility);
            if (canToggleVisibility != null)
                canToggleVisibility.ToggleVisibility += (o, e) => wnd.Visibility = e.Visibility;                           

            return wnd;
        }

        private void SetupShell(Window window) {
            //Source: http://code-inside.de/blog/2012/11/11/howto-rahmenlose-wpf-apps-mit-schattenwurf/
            window.WindowStyle = WindowStyle.None;
            window.ResizeMode = ResizeMode.NoResize;
            window.ShowInTaskbar = false;
            window.SourceInitialized += (o, e) => {
                                            if (!Helper.IsWindows7)
                                                return;
                                            var helper = new WindowInteropHelper(window);
                                            var val = 2;
                                            DwmSetWindowAttribute(helper.Handle, 2, ref val, 4);
                                            var m = new Margins { bottomHeight = -1, leftWidth = -1, rightWidth = -1, topHeight = -1 };
                                            DwmExtendFrameIntoClientArea(helper.Handle, ref m);
                                            IntPtr hwnd = new WindowInteropHelper(window).Handle;
                                        };
            window.MouseLeftButtonDown += (o, e) => window.DragMove();

            //Track changes on TopMost-settings
            _Settings.PropertyChanged += (o, e) => {
                                             if (e.PropertyName == "AlwaysOnTop")
                                                 window.Topmost = _Settings.AlwaysOnTop;
                                         };
        }

        private void TrackLocation(Window wnd, object rootViewModel) {
            var wndId = rootViewModel.GetType().Name.ToSHA1();

            var savedPosition = _Settings.Positions.FirstOrDefault(p => p.WindowId == wndId);
            if (savedPosition == null) {
                wnd.WindowStartupLocation = wnd.Owner != null
                                                ? WindowStartupLocation.CenterOwner
                                                : WindowStartupLocation.CenterScreen;
            }
            else {
                wnd.WindowStartupLocation = WindowStartupLocation.Manual;
                wnd.Top = savedPosition.Top;
                wnd.Left = savedPosition.Left;
            }

            if (savedPosition == null) {
                savedPosition = new WindowPosition {WindowId = wndId};
                _Settings.Positions.Add(savedPosition);
            }

            wnd.Closing += (o, e) => {
                               savedPosition.Top = ((Window) o).Top;
                               savedPosition.Left = ((Window) o).Left;
                           };
        }
    }
}
