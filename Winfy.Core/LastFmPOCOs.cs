using System.Xml.Serialization;

namespace Winfy.Core {

    [XmlRoot(ElementName = "lfm")]
    public class LastFmResponse {
        [XmlAttribute(AttributeName = "status")]
        public string Status { get; set; }

        [XmlElement(ElementName = "track")]
        public Track[] Track { get; set; }

    }

    [XmlType(TypeName = "track")]
    public class Track {
        [XmlElement("id")]
        public int Id { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("album")]
        public Album Album { get; set; }
    }

    [XmlType(TypeName = "album")]
    public class Album {
        [XmlElement(ElementName = "image")]
        public Image[] Image { get; set; }
    }

    public class Image {
        [XmlAttribute(AttributeName = "size")]
        public string Size { get; set; }

        [XmlText]
        public string Url { get; set; }
    }
}
