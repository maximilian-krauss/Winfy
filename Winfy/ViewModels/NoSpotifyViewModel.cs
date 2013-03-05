using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winfy.Core;

namespace Winfy.ViewModels {
    public sealed class NoSpotifyViewModel : Screen {
        private readonly AppContracts _Contracts;

        public NoSpotifyViewModel(AppContracts contracts) {
            _Contracts = contracts;
            DisplayName = string.Format("No Spotify installed - {0}", _Contracts.ApplicationName);
        }

        public void Close() {
            TryClose();
        }

        public void GoToSpotify() {
            Helper.OpenUrl(_Contracts.SpotifyUrl);
        }
    }
}
