using Newtonsoft.Json;

namespace Winfy.Core.SpotifyLocal {
    public class ClientVersion {
        [JsonProperty("error")]
        public Error Error { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("client_version")]
        public string ClientVersionString { get; set; }

        [JsonProperty("running")]
        public bool Running { get; set; }
    }

    public class Cfid {
        [JsonProperty("error")]
        public Error Error { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }

    public class Status {

        [JsonProperty("error")]
        public Error Error { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("client_version")]
        public string ClientVersion { get; set; }

        [JsonProperty("playing")]
        public bool Playing { get; set; }

        [JsonProperty("shuffle")]
        public bool Shuffle { get; set; }

        [JsonProperty("repeat")]
        public bool Repeat { get; set; }

        [JsonProperty("play_enabled")]
        public bool PlayEnabled { get; set; }

        [JsonProperty("prev_enabled")]
        public bool PrevEnabled { get; set; }

        [JsonProperty("track")]
        public Track Track { get; set; }

        [JsonProperty("playing_position")]
        public double PlayingPosition { get; set; }

        [JsonProperty("server_time")]
        public int ServerTime { get; set; }

        [JsonProperty("volume")]
        public double Volume { get; set; }

        [JsonProperty("online")]
        public bool Online { get; set; }

        [JsonProperty("open_graph_state")]
        public OpenGraphState OpenGraphState { get; set; }

        [JsonProperty("running")]
        public bool Running { get; set; }
    }


    public class Error {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }


    public class OpenGraphState {

        [JsonProperty("private_session")]
        public bool PrivateSession { get; set; }

        [JsonProperty("posting_disabled")]
        public bool PostingDisabled { get; set; }
    }

    public class Track {
        [JsonProperty("track_resource")]
        public Resource TrackResource { get; set; }

        [JsonProperty("artist_resource")]
        public Resource ArtistResource { get; set; }

        [JsonProperty("album_resource")]
        public Resource AlbumResource { get; set; }

        [JsonProperty("length")]
        public int Length { get; set; }

        [JsonProperty("track_type")]
        public string TrackType { get; set; }
    }

    public class Resource {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }
    }

    public class Location {
        [JsonProperty("og")]
        public string Og { get; set; }
    }

}
