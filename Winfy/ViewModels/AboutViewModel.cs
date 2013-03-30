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

        public class ComponentData {
            public ComponentData(string url, string license, string name) {
                Url = url;
                License = license;
                Name = name;
            }

            public string Name { get; set; }
            public string License { get; set; }
            public string Url { get; set; }
        }

        public AboutViewModel(AppSettings settings, AppContracts contracts) {
            _Settings = settings;
            _Contracts = contracts;

            DisplayName = string.Format("About - {0}", _Contracts.ApplicationName);
            UsedComponents = new List<ComponentData>(new [] {
                                                                new ComponentData("http://caliburnmicro.codeplex.com/","MIT License","Caliburn.Micro"),
                                                                new ComponentData("http://nlog-project.org/","MIT License", "NLog"), 
                                                                new ComponentData("https://github.com/grumpydev/TinyIoC", "Ms-Pl","TinyIoC"), 
                                                                new ComponentData("https://github.com/dbuksbaum/Caliburn.Micro.TinyIOC","MIT License", "Caliburn.Micro.TinyIOC"), 
                                                                new ComponentData("http://json.codeplex.com/","MIT License", "Newtonsoft Json.Net"), 
                                                                new ComponentData("http://jariz.nl", "Apache 2.0 License","Spotify local API")
                                                            });
        }

        public string ApplicationName { get { return _Contracts.ApplicationName; } }
        public Version ApplicationVersion { get { return _Contracts.ApplicationVersion; } }

        private List<ComponentData> _UsedComponents;
        public List<ComponentData> UsedComponents {
            get { return _UsedComponents; }
            set { _UsedComponents = value; NotifyOfPropertyChange(() => UsedComponents); }
        }

        private ComponentData _SelectedComponent;
        public ComponentData SelectedComponent {
            get { return _SelectedComponent; }
            set { _SelectedComponent = value; NotifyOfPropertyChange(() => SelectedComponent); }
        }

        public void GoHome() {
            Helper.OpenUrl(_Contracts.HomepageUrl);
        }

        public void OpenComponentUrl() {
            if (_SelectedComponent == null)
                return;
            Helper.OpenUrl(SelectedComponent.Url);
        }

    }
}
