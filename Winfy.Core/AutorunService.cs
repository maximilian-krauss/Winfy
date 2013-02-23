using Microsoft.Win32;
using NLog;
using System;
using System.Windows;

namespace Winfy.Core {
    public sealed class AutorunService {
        private readonly Logger _Logger;
        private readonly AppSettings _Settings;
        private readonly AppContracts _Contracts;
        private readonly RegistryKey _AutorunHive;

        private const string AutorunSettingsName = "StartWithWindows";
        
        public AutorunService(Logger logger, AppSettings settings, AppContracts contracts) {
            _Logger = logger;
            _Settings = settings;
            _Contracts = contracts;
            _AutorunHive = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            ValidateAutorun();

            _Settings.PropertyChanged += (o, e) => {
                                             if (e.PropertyName == AutorunSettingsName)
                                                 ValidateAutorun();
                                         };
        }

        private void ValidateAutorun() {
            try {
                if (_Settings.StartWithWindows) { // Add/Update autorun
                    var currentValue = (string) _AutorunHive.GetValue(_Contracts.ApplicationName, string.Empty);
                    var currentLocation = Application.ResourceAssembly.Location;
                    if(currentValue != currentLocation)
                        _AutorunHive.SetValue(_Contracts.ApplicationName, currentLocation);
                }
                else { //Remove autorun
                    if (!string.IsNullOrEmpty((string) _AutorunHive.GetValue(_Contracts.ApplicationName, string.Empty)))
                        _AutorunHive.DeleteValue(_Contracts.ApplicationName, false);
                }
            }
            catch (Exception exc) {
                _Logger.FatalException("Failed to update or remove autorun", exc);
            }
        }
    }
}
