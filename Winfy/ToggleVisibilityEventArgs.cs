using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Winfy {
    public sealed class ToggleVisibilityEventArgs : EventArgs {

        public ToggleVisibilityEventArgs(Visibility visibility) {
            Visibility = visibility;
        }

        public Visibility Visibility { get; private set; }
    }
}
