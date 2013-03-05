using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Winfy.Core;

namespace Winfy.ViewModels {
    public sealed class AboutViewModel : Screen {
        private readonly AppSettings _Settings;
        private readonly AppContracts _Contracts;

        public AboutViewModel(AppSettings settings, AppContracts contracts) {
            _Settings = settings;
            _Contracts = contracts;

            DisplayName = string.Format("About - {0}", _Contracts.ApplicationName);
        }

        public string ApplicationName { get { return _Contracts.ApplicationName; } }
        public Version ApplicationVersion { get { return _Contracts.ApplicationVersion; } }

        public void GoHome() {
            Helper.OpenUrl(_Contracts.HomepageUrl);
        }

    }
}
