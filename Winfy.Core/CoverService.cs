using NLog;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Serialization;
using Winfy.Core.Extensions;

namespace Winfy.Core {
    public class CoverService : ICoverService {

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

        private const string CacheFileNameTemplate = "{0}.jpg";
        private readonly AppContracts _Contracts;
        private readonly string _CacheDirectory;
        private readonly Logger _Logger;

        public CoverService(AppContracts contracts, Logger logger) {
            _Contracts = contracts;
            _CacheDirectory = Path.Combine(contracts.SettingsLocation, "CoverCache");
            _Logger = logger;
            if (!Directory.Exists(_CacheDirectory))
                Directory.CreateDirectory(_CacheDirectory);
        }

        public double CacheSize() {
            return !Directory.Exists(_CacheDirectory)
                       ? 0.0
                       : Directory.GetFiles(_CacheDirectory, "*.jpg").Sum(f => new FileInfo(f).Length);
        }

        public void ClearCache() {
            Directory.GetFiles(_CacheDirectory,"*.jpg").ToList().ForEach(f => {
                                                                             try {
                                                                                 File.Delete(f);
                                                                             }
                                                                             catch (Exception exc) {
                                                                                 _Logger.WarnException("Failed to delete file", exc);
                                                                             }
                                                                         });
        }

        public string FetchCover(string artist, string track) {
            var cachedFileName = Path.Combine(_CacheDirectory, string.Format(CacheFileNameTemplate, (artist+ track).ToSHA1()));
            if (File.Exists(cachedFileName))
                return cachedFileName;

            var requestUrl = string.Format("http://ws.audioscrobbler.com/2.0/?method=track.getInfo&api_key={0}&track={1}&artist={2}",
                    _Contracts.LastFmApiKey,
                    HttpUtility.UrlEncode(CleanTrackName(track)),
                    HttpUtility.UrlEncode(artist));
            try {
                var request = Helper.CreateWebRequest(requestUrl);
                var response = (HttpWebResponse)request.GetResponse();
                LastFmResponse lfmResponse;
                using (var reader = new StreamReader(response.GetResponseStream())) {
                    var serializer = new XmlSerializer(typeof (LastFmResponse));
                    lfmResponse = serializer.Deserialize(reader) as LastFmResponse;
                    if(lfmResponse == null || lfmResponse.Status != "ok")
                        throw new Exception("Could not fetch lastfm details");
                }
                response.Close();

                if (lfmResponse.Status == "ok" && lfmResponse.Track != null && lfmResponse.Track.Length > 0) {
                    var lfmTrack = lfmResponse.Track[0];
                    if (lfmTrack.Album != null && lfmTrack.Album.Image != null && lfmTrack.Album.Image.Length > 0) {
                        var images = lfmTrack.Album.Image;
                        var largeImage = images.FirstOrDefault(i => i.Size == "large");
                        return DownloadAndSaveImage(largeImage != null ? largeImage.Url : images.Last().Url, cachedFileName);
                    }
                }
                return string.Empty;
            }
            catch (Exception exc) {
                _Logger.WarnException(string.Format("Failed to retrieve cover. Endpoint: {0}", requestUrl), exc);
                return string.Empty;
            }
        }

        private string DownloadAndSaveImage(string url, string destination) {
            var request = Helper.CreateWebRequest(url);
            var response = (HttpWebResponse) request.GetResponse();

            using (var fs = File.Create(destination)) {
                using (var rs = response.GetResponseStream()) {
                    var buffer = new byte[1024];
                    var bytesRead = rs.Read(buffer, 0, buffer.Length);
                    while (bytesRead > 0) {
                        fs.Write(buffer, 0, bytesRead);
                        bytesRead = rs.Read(buffer, 0, buffer.Length);
                    }
                }
            }

            response.Close();
            return destination;
        }

        
        private string CleanTrackName(string track) {
            //By removing these strings, we raise the chance to find a proper cover image
            var misleadingWords = new[] {
                                            "Original Version", "Radio Edit", "Single Version", "Original Mix",
                                            "Explicit Version", "Single Mix"
                                        };

            var formats = new[] {"- {0}", "({0})", "[{0}]"};
            return misleadingWords.Aggregate(track,
                                              (currentWord, word) =>
                                              formats.Aggregate(currentWord, (current, format) =>
                                                  current.Replace(string.Format(format, word), string.Empty)))
                                                  .Trim();
        }
    }
}