using Caliburn.Micro;
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

        public SettingsViewModel(AppSettings settings, AppContracts contracts, ICoverService coverService) {
            _Settings = settings;
            _Contracts = contracts;
            _CoverService = coverService;
            DisplayName = string.Format("Settings - {0}", _Contracts.ApplicationName);
            AlwaysOnTop = _Settings.AlwaysOnTop;
        }

        public bool AlwaysOnTop {
            get { return _Settings.AlwaysOnTop; }
            set { _Settings.AlwaysOnTop = value; NotifyOfPropertyChange(() => AlwaysOnTop); }
        }

        private bool _CanClearCache = true;
        public bool CanClearCache {
            get { return _CanClearCache; }
            set { _CanClearCache = value; NotifyOfPropertyChange(() => CanClearCache); }
        }

        public void ClearCache() {
            _CoverService.ClearCache();
            CanClearCache = false;
        }

    }
}