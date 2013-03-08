using System.Collections.Generic;
using Caliburn.Micro;

namespace Winfy.Core {
    public sealed class AppSettings : PropertyChangedBase {

        public AppSettings() {
            Positions = new List<WindowPosition>();
        }

        private bool _AlwaysOnTop;
        public bool AlwaysOnTop {
            get { return _AlwaysOnTop; }
            set { _AlwaysOnTop = value; NotifyOfPropertyChange(() => AlwaysOnTop); }
        }

        private bool _StartWithWindows;
        public bool StartWithWindows {
            get { return _StartWithWindows; }
            set { _StartWithWindows = value; NotifyOfPropertyChange(() => StartWithWindows); }
        }

        private bool _HideIfSpotifyClosed;
        public bool HideIfSpotifyClosed {
            get { return _HideIfSpotifyClosed; }
            set { _HideIfSpotifyClosed = value; NotifyOfPropertyChange(() => HideIfSpotifyClosed); }
        }

        public List<WindowPosition> Positions { get; set; }

    }
}