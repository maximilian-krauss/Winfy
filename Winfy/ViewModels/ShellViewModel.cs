using System.Runtime.Remoting.Messaging;
using System.Windows;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using Winfy.Core;
using Winfy.Core.Deployment;
using Action = System.Action;
using TinyIoC;
using NLog;

namespace Winfy.ViewModels {
    public sealed class ShellViewModel : Screen, IToggleVisibility {
        private readonly IWindowManager _WindowManager;
        private readonly ISpotifyController _SpotifyController;
        private readonly ICoverService _CoverService;
        private readonly IEventAggregator _EventAggregator;
        private readonly IUpdateService _UpdateService;
        private readonly IUsageTrackerService _UsageTrackerService;
        private readonly AppSettings _Settings;
        private readonly Core.ILog _Logger;
        private const string NoCoverUri = @"pack://application:,,,/Winfy;component/Images/LogoWhite.png";
        private const string UnknownCoverUri = @"pack://application:,,,/Winfy;component/Images/LogoUnknown.png";

        public event EventHandler<ToggleVisibilityEventArgs> ToggleVisibility;
        public event EventHandler CoverDisplayFadeOut;
        public event EventHandler CoverDisplayFadeIn;
        
        public ShellViewModel(IWindowManager windowManager, ISpotifyController spotifyController, ICoverService coverService, IEventAggregator eventAggregator, AppSettings settings, Core.ILog logger, IUpdateService updateService, IUsageTrackerService usageTrackerService) {
            _WindowManager = windowManager;
            _SpotifyController = spotifyController;
            _CoverService = coverService;
            _EventAggregator = eventAggregator;
            _Settings = settings;
            _Logger = logger;
            _UpdateService = updateService;
            _UsageTrackerService = usageTrackerService;

            CoverImage = NoCoverUri;
            UpdateView();

            _SpotifyController.TrackChanged += (o, e) => UpdateView();
            _SpotifyController.SpotifyOpened += (o, e) => SpotifyOpened();
            _SpotifyController.SpotifyExited += (o, e) => SpotifyExited();
            _UpdateService.UpdateReady += UpdateReady;
            _UpdateService.StartBackgroundCheck();
            _UsageTrackerService.Track();
        }

        protected override void OnViewLoaded(object view) {
            base.OnViewLoaded(view);

            if (!_SpotifyController.IsSpotifyInstalled())
                _WindowManager.ShowDialog(TinyIoCContainer.Current.Resolve<NoSpotifyViewModel>());

            if(_Settings.HideIfSpotifyClosed && !_SpotifyController.IsSpotifyOpen())
                OnToggleVisibility(new ToggleVisibilityEventArgs(Visibility.Hidden));
        }

        #region Properties

        private string _CurrentTrack;
        public string CurrentTrack {
            get { return _CurrentTrack; }
            set { _CurrentTrack = value; NotifyOfPropertyChange(() => CurrentTrack); }
        }

        private string _CurrentArtist;
        public string CurrentArtist {
            get { return _CurrentArtist; }
            set { _CurrentArtist = value; NotifyOfPropertyChange(() => CurrentArtist); }
        }

        private string _CoverImage;
        public string CoverImage {
            get { return _CoverImage; }
            set { _CoverImage = value; NotifyOfPropertyChange(() => CoverImage); }
        }

        private bool _CanPlayPause;
        public bool CanPlayPause {
            get { return _CanPlayPause; }
            set { _CanPlayPause = value; NotifyOfPropertyChange(() => CanPlayPause); }
        }

        private bool _CanPlayPrevious;
        public bool CanPlayPrevious {
            get { return _CanPlayPrevious; }
            set { _CanPlayPrevious = value; NotifyOfPropertyChange(() => CanPlayPrevious); }
        }

        private bool _CanPlayNext;
        public bool CanPlayNext {
            get { return _CanPlayNext; }
            set { _CanPlayNext = value; NotifyOfPropertyChange(() => CanPlayNext); }
        }

        private bool _HasTrackInformation;
        public bool HasTrackInformation {
            get { return _HasTrackInformation; }
            set { _HasTrackInformation = value; NotifyOfPropertyChange(() => HasTrackInformation); }
        }

        #endregion

        public void ShowSettings() {
            _WindowManager.ShowDialog(TinyIoCContainer.Current.Resolve<SettingsViewModel>());
        }

        public void ShowAbout() {
            _WindowManager.ShowDialog(TinyIoCContainer.Current.Resolve<AboutViewModel>());
        }

        public void PlayPause() {
            _SpotifyController.PausePlay();
        }

        public void PlayPrevious() {
            _SpotifyController.PreviousTrack();
        }

        public void PlayNext() {
            _SpotifyController.NextTrack();
        }

        public void VolumeUp() {
            _SpotifyController.VolumeUp();
        }

        public void VolumeDown() {
            _SpotifyController.VolumeDown();
        }

        private void SpotifyOpened() {
            if(_Settings.HideIfSpotifyClosed)
                OnToggleVisibility(new ToggleVisibilityEventArgs(Visibility.Visible));

            UpdateView();
        }

        private void SpotifyExited() {
            if(_Settings.HideIfSpotifyClosed)
                OnToggleVisibility(new ToggleVisibilityEventArgs(Visibility.Hidden));

            UpdateView();
        }

        private void UpdateView() {
            try {
                var status = _SpotifyController.GetStatus();
                var track = _SpotifyController.GetSongName();
                var artist = _SpotifyController.GetArtistName();
                var fade = (status != null && status.Playing);

                if(fade)
                    OnCoverDisplayFadeOut();

                HasTrackInformation = (!string.IsNullOrEmpty(track) || !string.IsNullOrEmpty(artist));
                CurrentTrack = string.IsNullOrEmpty(track) ? "-" : track;
                CurrentArtist = string.IsNullOrEmpty(artist) ? "-" : artist;

                CanPlayPause = _SpotifyController.IsSpotifyOpen();
                CanPlayPrevious = _SpotifyController.IsSpotifyOpen();
                CanPlayNext = _SpotifyController.IsSpotifyOpen();

                if (_SpotifyController.IsSpotifyOpen() && !string.IsNullOrEmpty(track) && !string.IsNullOrEmpty(artist)) {
                    if(_Settings.DisableAnimations)
                        CoverImage = NoCoverUri; //Reset cover image, no cover is better than an old one

                    var updateCoverAction = new Action(() => {
                                                           var coverUri = _CoverService.FetchCover(artist, track);
                                                           if (string.IsNullOrEmpty(coverUri))
                                                               coverUri = UnknownCoverUri;
                                                           CoverImage = coverUri;
                                                           if (fade)
                                                               OnCoverDisplayFadeIn();
                                                       });
                    updateCoverAction.BeginInvoke(UpdateCoverActionCallback, null);
                }
                else {
                    CoverImage = NoCoverUri;
                    if(fade)
                        OnCoverDisplayFadeIn();
                }
            }
            catch (Exception exc) {
                _Logger.FatalException("UpdateView() failed hard", exc);
            }
        }

        private void UpdateCoverActionCallback(IAsyncResult result) {
            var asyncResult = result as AsyncResult;
            if (asyncResult != null) {
                var d = asyncResult.AsyncDelegate as Action;
                if (d != null)
                    d.EndInvoke(result);
            }
        }

        void UpdateReady(object sender, UpdateReadyEventArgs e) {
            if(e.IsRequired) //important update, do the restart asap
                _UpdateService.Restart();

            Execute.OnUIThread(() => _WindowManager.ShowDialog(
                TinyIoCContainer.Current.Resolve<UpdateReadyViewModel>(
                    new NamedParameterOverloads(new Dictionary<string, object> {
                                                                                   {"newVersion", e.NewVersion},
                                                                                   {"changelog",e.Changelog}
                                                                               }))));

        }
        

        private void OnToggleVisibility(ToggleVisibilityEventArgs e) {
            Execute.OnUIThread(() => {
                                   var handler = ToggleVisibility;
                                   if (handler != null) handler(this, e);
                               });
        }
        private void OnCoverDisplayFadeOut() {
            Execute.OnUIThread(() => {
                                   if (_Settings.DisableAnimations)
                                       return;

                                   var handler = CoverDisplayFadeOut;
                                   if (handler != null) handler(this, EventArgs.Empty);
                               });
        }

        private void OnCoverDisplayFadeIn() {
            Execute.OnUIThread(() => {
                                   if (_Settings.DisableAnimations)
                                       return;

                                   var handler = CoverDisplayFadeIn;
                                   if (handler != null) handler(this, EventArgs.Empty);
                               });
        }
    }
}