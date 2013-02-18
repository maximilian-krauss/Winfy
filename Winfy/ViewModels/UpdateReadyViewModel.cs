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

        public UpdateReadyViewModel(IUpdateController updateController, AppContracts contracts, Version newVersion) {
            _UpdateController = updateController;
            _Contracts = contracts;
            NewVersion = newVersion.ToString();
            DisplayName = string.Format("Update ready - {0}", _Contracts.ApplicationName);
        }

        private string _NewVersion;
        public string NewVersion {
            get { return _NewVersion; }
            set { _NewVersion = value; NotifyOfPropertyChange(() => NewVersion); }
        }

        public void Restart() {
            _UpdateController.Restart();
        }

    }
}