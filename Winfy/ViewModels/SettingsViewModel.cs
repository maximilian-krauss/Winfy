using Caliburn.Micro;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winfy.Core;

namespace Winfy.ViewModels {
    public sealed class SettingsViewModel : Screen {
        private readonly AppSettings _Settings;
        private readonly AppContracts _Contracts;
        private readonly ICoverService _CoverService;
        private readonly Core.ILog _Logger;

        public SettingsViewModel(AppSettings settings, AppContracts contracts, ICoverService coverService, Core.ILog logger) {
            _Settings = settings;
            _Contracts = contracts;
            _CoverService = coverService;
            _Logger = logger;
            DisplayName = string.Format("Settings - {0}", _Contracts.ApplicationName);
            CacheSize = Helper.MakeNiceSize(_CoverService.CacheSize());
        }

        public bool AlwaysOnTop {
            get { return _Settings.AlwaysOnTop; }
            set { _Settings.AlwaysOnTop = value; NotifyOfPropertyChange(() => AlwaysOnTop); }
        }

        public bool StartWithWindows {
            get { return _Settings.StartWithWindows; }
            set { _Settings.StartWithWindows = value; NotifyOfPropertyChange(() => StartWithWindows); }
        }

        public bool HideIfSpotifyClosed {
            get { return _Settings.HideIfSpotifyClosed; }
            set { _Settings.HideIfSpotifyClosed = value; NotifyOfPropertyChange(() => HideIfSpotifyClosed); }
        }

        public bool DisableAnimations {
            get { return _Settings.DisableAnimations; }
            set { _Settings.DisableAnimations = value; NotifyOfPropertyChange(() => DisableAnimations); }
        }

        private bool _CanClearCache = true;
        public bool CanClearCache {
            get { return _CanClearCache; }
            set { _CanClearCache = value; NotifyOfPropertyChange(() => CanClearCache); }
        }

        private string _CacheSize;
        public string CacheSize {
            get { return _CacheSize; }
            set { _CacheSize = value; NotifyOfPropertyChange(() => CacheSize); }
        }

        public void ClearCache() {
            try {
                _CoverService.ClearCache();
                CacheSize = Helper.MakeNiceSize(_CoverService.CacheSize());
            }
            catch (Exception exc) {
                _Logger.WarnException("Failed to clear cover cache", exc);
            }
            CanClearCache = false;
        }

    }
}