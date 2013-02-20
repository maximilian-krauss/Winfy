using System;
using System.Collections.Generic;

namespace Winfy.Core.Deployment {
    public sealed class UpdateReadyEventArgs : EventArgs {

        public UpdateReadyEventArgs() {
            Changelog = new List<Release>();
        }

        public Version NewVersion { get; set; }
        public bool IsRequired { get; set; }
        public List<Release> Changelog { get; set; }
    }
}