using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winfy {
    public interface IToggleVisibility {
        event EventHandler<ToggleVisibilityEventArgs> ToggleVisibility;
    }
}
