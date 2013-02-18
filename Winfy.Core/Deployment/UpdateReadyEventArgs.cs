using System;

namespace Winfy.Core.Deployment {
    public sealed class UpdateReadyEventArgs : EventArgs {
        public Version NewVersion { get; set; }
        public bool IsRequired { get; set; }
    }
}