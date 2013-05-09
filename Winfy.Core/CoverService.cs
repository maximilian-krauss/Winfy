using NLog;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Serialization;
using Winfy.Core.Extensions;
using Winfy.Core.SpotifyLocal;

namespace Winfy.Core {
    public class CoverService : ICoverService {

        private const string CacheFileNameTemplate = "{0}.jpg";
        private readonly AppContracts _Contracts;
        private readonly string _CacheDirectory;
        private readonly SpotifyLocalApi _LocalApi;
        private readonly ILog _Logger;

        public CoverService(AppContracts contracts, ILog logger, SpotifyLocalApi localApi) {
            _Contracts = contracts;
            _CacheDirectory = Path.Combine(contracts.SettingsLocation, "CoverCache");
            _Logger = logger;
            _LocalApi = localApi;
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
            var cachedFileName = Path.Combine(_CacheDirectory, string.Format(CacheFileNameTemplate, (artist + track).ToSHA1()));
            if (File.Exists(cachedFileName))
                return cachedFileName;

            var spotifyCover = FetchSpotifyCover(cachedFileName);
            return string.IsNullOrEmpty(spotifyCover) ? FetchLastFmCover(artist, track, cachedFileName) : spotifyCover;
        }

        private string FetchSpotifyCover(string cachedFileName) {
            return string.Empty;

            if (!_LocalApi.HasValidToken)
                return string.Empty;

            try {
                var trackStatus = _LocalApi.Status;
                if (trackStatus != null) {
                    if (trackStatus.Error != null)
                        throw new Exception(string.Format("API Error: {0} (0x{1})", trackStatus.Error.Message,
                                                          trackStatus.Error.Type));

                    if (trackStatus.Track != null && trackStatus.Track.AlbumResource != null) {
                        var coverUrl = _LocalApi.GetArt(trackStatus.Track.AlbumResource.Uri);
                        if (!string.IsNullOrEmpty(coverUrl))
                            return DownloadAndSaveImage(coverUrl, cachedFileName);
                    }
                }
            }
            catch (WebException webExc) {
                if(webExc.Response != null && ((HttpWebResponse)webExc.Response).StatusCode != HttpStatusCode.NotFound)
                    _Logger.WarnException(string.Format("Failed to retrieve Image via Spotify Local API: {0}", webExc.Message), webExc);
            }
            catch (Exception exc) {
                _Logger.WarnException("Failed to retrieve cover from Spotify", exc);
            }
            return string.Empty;
        }

        private string FetchLastFmCover(string artist, string track, string cachedFileName) {
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