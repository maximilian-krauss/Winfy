using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Winfy.Core.Deployment {
    [DataContract]
    public sealed class Release {

        public Release() {
            Changes = new List<string>();
        }

        [DataMember(Name = "version")]
        public string Version { get; set; }

        [IgnoreDataMember]
        public Version ReleaseVersion{get { return new Version(Version); }}

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "changes")]
        public List<string> Changes { get; set; }
    }
}
