using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winfy.Core;
using Winfy.Core.Deployment;
using Action = System.Action;
using TinyIoC;
using NLog;

namespace Winfy.ViewModels {
    public sealed class ShellViewModel : Screen {
        private readonly IWindowManager _WindowManager;
        private readonly ISpotifyController _SpotifyController;
        private readonly ICoverService _CoverService;
        private readonly IEventAggregator _EventAggregator;
        private readonly IUpdateController _UpdateController;
        private readonly AppSettings _Settings;
        private readonly Logger _Logger;
        private const string NoCoverUri = @"pack://application:,,,/Winfy;component/Images/LogoWhite.png";

        public ShellViewModel(IWindowManager windowManager, ISpotifyController spotifyController, ICoverService coverService, IEventAggregator eventAggregator, AppSettings settings, Logger logger, IUpdateController updateController) {
            _WindowManager = windowManager;
            _SpotifyController = spotifyController;
            _CoverService = coverService;
            _EventAggregator = eventAggregator;
            _Settings = settings;
            _Logger = logger;
            _UpdateController = updateController;

            CoverImage = NoCoverUri;
            UpdateView();

            _SpotifyController.TrackChanged += (o, e) => UpdateView();
            _SpotifyController.SpotifyOpened += (o, e) => UpdateView();
            _SpotifyController.SpotifyExited += (o, e) => UpdateView();
            _UpdateController.UpdateReady += UpdateReady;
            _UpdateController.StartBackgroundCheck();
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

        private void UpdateView() {
            try {
                var track = _SpotifyController.GetSongName();
                var artist = _SpotifyController.GetArtistName();

                CurrentTrack = string.IsNullOrEmpty(track) ? "-" : track;
                CurrentArtist = string.IsNullOrEmpty(artist) ? "-" : artist;

                CanPlayPause = _SpotifyController.IsSpotifyOpen();
                CanPlayPrevious = _SpotifyController.IsSpotifyOpen();
                CanPlayNext = _SpotifyController.IsSpotifyOpen();

                if (_SpotifyController.IsSpotifyOpen() && !string.IsNullOrEmpty(track) && !string.IsNullOrEmpty(artist)) {
                    var updateCoverAction = new Action(() => {
                                                           var coverUri = _CoverService.FetchCover(artist, track);
                                                           if (string.IsNullOrEmpty(coverUri))
                                                               coverUri = NoCoverUri;
                                                           CoverImage = coverUri;
                                                       });
                    updateCoverAction.BeginInvoke(UpdateCoverActionCallback, null);
                }
                else
                    CoverImage = NoCoverUri;
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
                _UpdateController.Restart();
            
            Execute.OnUIThread(() => _WindowManager.ShowDialog(
                TinyIoCContainer.Current.Resolve<UpdateReadyViewModel>(
                    new NamedParameterOverloads(new Dictionary<string, object> {{"newVersion", e.NewVersion}}))));

        }
    }
}