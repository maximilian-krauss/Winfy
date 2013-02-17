using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public List<WindowPosition> Positions { get; set; }

    }
}