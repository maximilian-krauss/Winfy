using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro.TinyIOC;
using Winfy.Core;
using Winfy.ViewModels;
using Caliburn.Micro;
using System.IO;

namespace Winfy {
    public sealed class AppBootstrapper : TinyBootstrapper<ShellViewModel> {
        private AppSettings _Settings;
        private AppContracts _Contracts;
        private string _SettingsPath;
        
        protected override void Configure() {
            base.Configure();

            _Contracts = new AppContracts();
            _SettingsPath = Path.Combine(_Contracts.SettingsLocation, _Contracts.SettingsFilename);
            _Settings = File.Exists(_SettingsPath)
                            ? Serializer.DeserializeFromJson<AppSettings>(_SettingsPath)
                            : new AppSettings();

            Container.Register<AppContracts>(_Contracts);
            Container.Register<AppSettings>(_Settings);
            Container.Register<IWindowManager>(new AppWindowManager(_Settings));
            Container.Register<ISpotifyController>(new SpotifyController());
            Container.Register<ICoverService>(new CoverService(_Contracts));

        }

        protected override void OnExit(object sender, EventArgs e) {
            base.OnExit(sender, e);
            Serializer.SerializeToJson(_Settings, _SettingsPath);
        }
    }
}