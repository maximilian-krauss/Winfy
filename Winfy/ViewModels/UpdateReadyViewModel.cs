using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winfy.Core;
using Winfy.Core.Deployment;

namespace Winfy.ViewModels {
    public sealed class UpdateReadyViewModel : Screen {
        private readonly IUpdateController _UpdateController;
        private readonly AppContracts _Contracts;

        public UpdateReadyViewModel(IUpdateController updateController, AppContracts contracts, Version newVersion, List<Release> changelog) {
            _UpdateController = updateController;
            _Contracts = contracts;
            NewVersion = newVersion.ToString();
            DisplayName = string.Format("Update ready - {0}", _Contracts.ApplicationName);
            Changelog = changelog.Where(r => r.ReleaseVersion > contracts.ApplicationVersion).OrderByDescending(r => r.ReleaseVersion).ToList();
        }

        private string _NewVersion;
        public string NewVersion {
            get { return _NewVersion; }
            set { _NewVersion = value; NotifyOfPropertyChange(() => NewVersion); }
        }

        private List<Release> _Changelog;
        public List<Release> Changelog {
            get { return _Changelog; }
            set { _Changelog = value; NotifyOfPropertyChange(() => Changelog); }
        }

        public void Restart() {
            _UpdateController.Restart();
        }

    }
}