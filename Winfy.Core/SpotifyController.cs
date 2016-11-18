using Caliburn.Micro;
using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Winfy.Core.SpotifyLocal;
using Message = System.Windows.Forms.Message;
using Timer = System.Threading.Timer;

namespace Winfy.Core {
    public class SpotifyController : ISpotifyController {

        /*
         SpotifyController uses code from https://github.com/ranveer5289/SpotifyNotifier-Windows and https://github.com/mscoolnerd/SpotifyLib
         */

        public event EventHandler SpotifyExited;
        public event EventHandler TrackChanged;
        public event EventHandler SpotifyOpened;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        [DllImport("user32")]
        private static extern bool GetMessage(ref Message lpMsg, IntPtr handle, uint mMsgFilterInMain, uint mMsgFilterMax);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        const int KeyMessage = 0x319;
        const int ControlKey = 0x11;

        const long PlaypauseKey = 0xE0000L;
        const long NexttrackKey = 0xB0000L;
        const long PreviousKey = 0xC0000L;

        private const string SpotifyRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Uninstall\Spotify";

        private readonly ILog _Logger;
        private readonly SpotifyLocalApi _LocalApi;

        private Process _SpotifyProcess;
        private Thread _BackgroundChangeTracker;
        private Timer _ProcessWatcher;
        private WinEventDelegate _ProcDelegate;
        private Status _CurrentTrackInfo;

        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        public SpotifyController(ILog logger, SpotifyLocalApi localApi) {
            _Logger = logger;
            _LocalApi = localApi;
            AttachToProcess();
            JoinBackgroundProcess();

            if(_SpotifyProcess == null)
                WaitForSpotify();
        }

        private void JoinBackgroundProcess() {
            if (_BackgroundChangeTracker != null && _BackgroundChangeTracker.IsAlive)
                return;

            _BackgroundChangeTracker = new Thread(BackgroundChangeTrackerWork) { IsBackground = true };
            _BackgroundChangeTracker.Start();
        }

        private void AttachToProcess() {
            _SpotifyProcess = null;
            _SpotifyProcess = Process.GetProcessesByName("spotify").Where(x => x.MainWindowHandle.ToInt32() != 0).FirstOrDefault();
            if (_SpotifyProcess != null) {
                //Renew token for Spotify local api
                _LocalApi.RenewToken();

                _SpotifyProcess.EnableRaisingEvents = true;
                _SpotifyProcess.Exited += (o, e) => {
                                              _SpotifyProcess = null;
                                              _BackgroundChangeTracker.Abort();
                                              _BackgroundChangeTracker = null;
                                              WaitForSpotify();
                                              OnSpotifyExited();
                                          };
            }
        }

        private void WaitForSpotify() {
            _ProcessWatcher = new Timer(WaitForSpotifyCallback, null, 1000, 1000);
        }

        private void WaitForSpotifyCallback(object args) {
            AttachToProcess();
            if (_SpotifyProcess != null) {
             
                //Start track change tracker
                JoinBackgroundProcess();

                //Kill timer
                if (_ProcessWatcher != null) {
                    _ProcessWatcher.Dispose();
                    _ProcessWatcher = null;
                }

                //Notify UI that Spotify is available
                OnSpotifyOpenend();
            }
        }

        protected virtual void OnSpotifyExited() {
            var handler = SpotifyExited;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnTrackChanged() {
            var handler = TrackChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnSpotifyOpenend() {
            var handler = SpotifyOpened;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime) {
            if ((idObject == 0) && (idChild == 0))
                if (hwnd.ToInt32() == _SpotifyProcess.MainWindowHandle.ToInt32()) {
                    try {
                        _CurrentTrackInfo = _LocalApi.Status;
                        if(_CurrentTrackInfo != null && _CurrentTrackInfo.Error != null)
                            throw new Exception(string.Format("Spotify API error: {0}", _CurrentTrackInfo.Error.Message));
                    }
                    catch (Exception exc) {
                        _Logger.WarnException("Failed to retrieve trackinfo", exc);
                        _CurrentTrackInfo = null;
                    }
                    OnTrackChanged();
                }
        }

        private void BackgroundChangeTrackerWork() {
            try {
                if (_SpotifyProcess == null) //Spotify is not running :-(
                    return;

                _ProcDelegate = new WinEventDelegate(WinEventProc);

                if (_SpotifyProcess != null) {
                    var hwndSpotify = _SpotifyProcess.MainWindowHandle;
                    var pidSpotify = _SpotifyProcess.Id;

                    var hWinEventHook = SetWinEventHook(0x0800c, 0x800c, IntPtr.Zero, _ProcDelegate, Convert.ToUInt32(pidSpotify), 0, 0);
                    var msg = new Message();
                    while (GetMessage(ref msg, hwndSpotify, 0, 0))
                        UnhookWinEvent(hWinEventHook);
                }
            }
            catch (ThreadAbortException) { /* Thread was aborted, accept it */ }
            catch (Exception exc) {
                _Logger.WarnException("BackgroundChangeTrackerWork failed", exc);
                Console.WriteLine(exc.ToString());
            }
        }

        private string GetSpotifyWindowTitle() {
            if(_SpotifyProcess == null)
                return string.Empty;

            // Allocate correct string length first
            var length = GetWindowTextLength(_SpotifyProcess.MainWindowHandle);
            var sb = new StringBuilder(length + 1);
            GetWindowText(_SpotifyProcess.MainWindowHandle, sb, sb.Capacity);
            return sb.ToString();
        }

        public bool IsSpotifyOpen() {
            return _SpotifyProcess != null;
        }

        public bool IsSpotifyInstalled() {
            try {
                //first try: the installation directory
                var spotifyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                               "Spotify", "spotify.exe");
                if (File.Exists(spotifyPath))
                    return true;

                //second try: look into the registry
                var registryKey = Registry.CurrentUser.OpenSubKey(SpotifyRegistryKey, false);
                if (registryKey != null && File.Exists((string) registryKey.GetValue("DisplayIcon", string.Empty)))
                    return true; //looks good, return true

                return false;
            }
            catch (Exception exc) {
                _Logger.WarnException("Failed to detect if Spotify is installed or not :(", exc);
                //In case of an error it's better to return true instead of false, because this makes Winfy unusable if there is something wrong with Windows.
                return true;
            }
        }

        public string GetSongName() {
            if (_CurrentTrackInfo != null && _CurrentTrackInfo.Track != null && _CurrentTrackInfo.Track.TrackResource != null)
                return _CurrentTrackInfo.Track.TrackResource.Name;

            string[] title = GetSpotifyWindowTitle().Split(new char[] { '-' }, 2); return title.Count() > 1 ? title[1].Trim() : string.Empty;
        }

        public string GetArtistName() {
            if (_CurrentTrackInfo != null && _CurrentTrackInfo.Track != null && _CurrentTrackInfo.Track.ArtistResource != null)
                return _CurrentTrackInfo.Track.ArtistResource.Name;

            string[] title = GetSpotifyWindowTitle().Split(new char[] { '-' }, 2); return title.Count() > 1 ? title[0].Trim() : string.Empty;
        }

        public Status GetStatus() {
            return _CurrentTrackInfo;
        }

        public void PausePlay() {
            PostMessage(_SpotifyProcess.MainWindowHandle, KeyMessage, IntPtr.Zero, new IntPtr(PlaypauseKey));
        }

        public void NextTrack() {
            PostMessage(_SpotifyProcess.MainWindowHandle, KeyMessage, IntPtr.Zero, new IntPtr(NexttrackKey));
        }

        public void PreviousTrack() {
            PostMessage(_SpotifyProcess.MainWindowHandle, KeyMessage, IntPtr.Zero, new IntPtr(PreviousKey));
        }

        public void VolumeUp() {
            keybd_event(ControlKey, 0x1D, 0, 0);
            PostMessage(_SpotifyProcess.MainWindowHandle, 0x100, new IntPtr(0x26), IntPtr.Zero);
            Thread.Sleep(100);
            keybd_event(ControlKey, 0x1D, 0x2, 0);
        }

        public void VolumeDown() {
            keybd_event(ControlKey, 0x1D, 0, 0);
            PostMessage(_SpotifyProcess.MainWindowHandle, 0x100, new IntPtr(0x28), IntPtr.Zero);
            Thread.Sleep(100);
            keybd_event(ControlKey, 0x1D, 0x2, 0);
        }

        public void Dispose() {
            if(_BackgroundChangeTracker.IsAlive)
                _BackgroundChangeTracker.Abort();
        }
    }
}
