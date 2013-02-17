using System;
using Caliburn.Micro.TinyIOC;
using Winfy.Core;
using Winfy.ViewModels;
using Caliburn.Micro;
using System.IO;
using NLog;

namespace Winfy {
    public sealed class AppBootstrapper : TinyBootstrapper<ShellViewModel> {
        private AppSettings _Settings;
        private AppContracts _Contracts;
        private Logger _Logger;
        private string _SettingsPath;
        
        protected override void Configure() {
            base.Configure();

            _Logger = NLog.LogManager.GetCurrentClassLogger();
            _Contracts = new AppContracts();
            _SettingsPath = Path.Combine(_Contracts.SettingsLocation, _Contracts.SettingsFilename);
            _Settings = File.Exists(_SettingsPath)
                            ? Serializer.DeserializeFromJson<AppSettings>(_SettingsPath)
                            : new AppSettings();

            Container.Register<AppContracts>(_Contracts);
            Container.Register<AppSettings>(_Settings);
            Container.Register<IWindowManager>(new AppWindowManager(_Settings));
            Container.Register<ISpotifyController>(new SpotifyController(_Logger));
            Container.Register<ICoverService>(new CoverService(_Contracts, _Logger));
            Container.Register<Logger>(_Logger);
        }

        protected override void OnExit(object sender, EventArgs e) {
            base.OnExit(sender, e);
            Serializer.SerializeToJson(_Settings, _SettingsPath);
        }
    }
}