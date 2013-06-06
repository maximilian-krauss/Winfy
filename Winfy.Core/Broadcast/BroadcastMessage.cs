using System.Runtime.Serialization;

namespace Winfy.Core.Broadcast {
    [DataContract]
    public sealed class BroadcastMessage {
        
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "active")]
        public bool Active { get; set; }

        [DataMember(Name = "affected_version")]
        public string AffectedVersion { get; set; }
        
        [DataMember(Name = "title")]
        public string Title { get; set; }
        
        [DataMember(Name = "body")]
        public string Body { get; set; }
        
        [DataMember(Name = "action_name")]
        public string ActionName { get; set; }
        
        [DataMember(Name = "action_url")]
        public string ActionUrl { get; set; }
    }
}