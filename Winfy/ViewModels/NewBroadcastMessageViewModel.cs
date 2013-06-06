using System.Windows;
using Caliburn.Micro;
using Winfy.Core;
using Winfy.Core.Broadcast;

namespace Winfy.ViewModels {
    public class NewBroadcastMessageViewModel : Screen, IFixedPosition {
        private readonly AppSettings _Settings;
        private readonly BroadcastMessage _Message;

        public NewBroadcastMessageViewModel(AppSettings settings, BroadcastMessage message) {
            _Settings = settings;
            _Message = message;
            ActionName = message.ActionName;
            Title = message.Title;
            Body = message.Body;
            ActionToolTip = string.Format("Opens \"{0}\"", _Message.ActionUrl);
        }

        protected override void OnViewLoaded(object view) {
            base.OnViewLoaded(view);
            DisplayName = "Important Winfy broadcast message";
        }

        protected override void OnDeactivate(bool close) {
            base.OnDeactivate(close);

            if(close)
                _Settings.ReadBroadcastMessageIds.Add(_Message.Id);
        }

        public WindowStartupLocation WindowStartupLocation {
            get { return WindowStartupLocation.CenterScreen; }
        }

        private string _Title;
        public string Title {
            get { return _Title; }
            set { _Title = value; NotifyOfPropertyChange(() => Title); }
        }

        private string _Body;
        public string Body {
            get { return _Body; }
            set { _Body = value;NotifyOfPropertyChange(() => Body); }
        }

        private string _ActionName;
        public string ActionName {
            get { return _ActionName; }
            set { _ActionName = value; NotifyOfPropertyChange(() => ActionName); NotifyOfPropertyChange(() => HasAction); }
        }

        private string _ActionToolTip;
        public string ActionToolTip {
            get { return _ActionToolTip; }
            set { _ActionToolTip = value; NotifyOfPropertyChange(() => ActionToolTip); }
        }

        public bool HasAction { get { return !string.IsNullOrEmpty(_ActionName) && !string.IsNullOrEmpty(_Message.ActionUrl); } }

        public void WindowClose() {
            TryClose();
        }

        public void ExecuteAction() {
            if (string.IsNullOrEmpty(_Message.ActionUrl))
                return;

            Helper.OpenUrl(_Message.ActionUrl);
        }
    }
}