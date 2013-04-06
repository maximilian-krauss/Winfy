using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Text;

namespace Winfy.Core.SpotifyLocal {
    public class SpotifyLocalApi {

        #region LICENSE

        /*
          The SpotifyLocal-Api code was original developed by JariZ<http://jariz.nl/> and is licensed under the terms of the Apache 2.0 License (see below).
         
          (This codes here features some improvements, cleanups and bugfixes which were made by Max)
         
          Original source: https://code.google.com/p/spotify-local-api/
         
         Apache License, Version 2.0 Apache License Version 2.0, January 2004 http://www.apache.org/licenses/
            TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION
            1. Definitions.
            "License" shall mean the terms and conditions for use, reproduction, and distribution as defined by Sections 1 through 9 of this document.
            "Licensor" shall mean the copyright owner or entity authorized by the copyright owner that is granting the License.
            "Legal Entity" shall mean the union of the acting entity and all other entities that control, are controlled by, or are under common control with that entity. For the purposes of this definition, "control" means (i) the power, direct or indirect, to cause the direction or management of such entity, whether by contract or otherwise, or (ii) ownership of fifty percent (50%) or more of the outstanding shares, or (iii) beneficial ownership of such entity.
            "You" (or "Your") shall mean an individual or Legal Entity exercising permissions granted by this License.
            "Source" form shall mean the preferred form for making modifications, including but not limited to software source code, documentation source, and configuration files.
            "Object" form shall mean any form resulting from mechanical transformation or translation of a Source form, including but not limited to compiled object code, generated documentation, and conversions to other media types.
            "Work" shall mean the work of authorship, whether in Source or Object form, made available under the License, as indicated by a copyright notice that is included in or attached to the work (an example is provided in the Appendix below).
            "Derivative Works" shall mean any work, whether in Source or Object form, that is based on (or derived from) the Work and for which the editorial revisions, annotations, elaborations, or other modifications represent, as a whole, an original work of authorship. For the purposes of this License, Derivative Works shall not include works that remain separable from, or merely link (or bind by name) to the interfaces of, the Work and Derivative Works thereof.
            "Contribution" shall mean any work of authorship, including the original version of the Work and any modifications or additions to that Work or Derivative Works thereof, that is intentionally submitted to Licensor for inclusion in the Work by the copyright owner or by an individual or Legal Entity authorized to submit on behalf of the copyright owner. For the purposes of this definition, "submitted" means any form of electronic, verbal, or written communication sent to the Licensor or its representatives, including but not limited to communication on electronic mailing lists, source code control systems, and issue tracking systems that are managed by, or on behalf of, the Licensor for the purpose of discussing and improving the Work, but excluding communication that is conspicuously marked or otherwise designated in writing by the copyright owner as "Not a Contribution."
            "Contributor" shall mean Licensor and any individual or Legal Entity on behalf of whom a Contribution has been received by Licensor and subsequently incorporated within the Work.
            2. Grant of Copyright License.
            Subject to the terms and conditions of this License, each Contributor hereby grants to You a perpetual, worldwide, non-exclusive, no-charge, royalty-free, irrevocable copyright license to reproduce, prepare Derivative Works of, publicly display, publicly perform, sublicense, and distribute the Work and such Derivative Works in Source or Object form.
            3. Grant of Patent License.
            Subject to the terms and conditions of this License, each Contributor hereby grants to You a perpetual, worldwide, non-exclusive, no-charge, royalty-free, irrevocable (except as stated in this section) patent license to make, have made, use, offer to sell, sell, import, and otherwise transfer the Work, where such license applies only to those patent claims licensable by such Contributor that are necessarily infringed by their Contribution(s) alone or by combination of their Contribution(s) with the Work to which such Contribution(s) was submitted. If You institute patent litigation against any entity (including a cross-claim or counterclaim in a lawsuit) alleging that the Work or a Contribution incorporated within the Work constitutes direct or contributory patent infringement, then any patent licenses granted to You under this License for that Work shall terminate as of the date such litigation is filed.
            4. Redistribution.
            You may reproduce and distribute copies of the Work or Derivative Works thereof in any medium, with or without modifications, and in Source or Object form, provided that You meet the following conditions:
            You must give any other recipients of the Work or Derivative Works a copy of this License; and You must cause any modified files to carry prominent notices stating that You changed the files; and You must retain, in the Source form of any Derivative Works that You distribute, all copyright, patent, trademark, and attribution notices from the Source form of the Work, excluding those notices that do not pertain to any part of the Derivative Works; and If the Work includes a "NOTICE" text file as part of its distribution, then any Derivative Works that You distribute must include a readable copy of the attribution notices contained within such NOTICE file, excluding those notices that do not pertain to any part of the Derivative Works, in at least one of the following places: within a NOTICE text file distributed as part of the Derivative Works; within the Source form or documentation, if provided along with the Derivative Works; or, within a display generated by the Derivative Works, if and wherever such third-party notices normally appear. The contents of the NOTICE file are for informational purposes only and do not modify the License. You may add Your own attribution notices within Derivative Works that You distribute, alongside or as an addendum to the NOTICE text from the Work, provided that such additional attribution notices cannot be construed as modifying the License. You may add Your own copyright statement to Your modifications and may provide additional or different license terms and conditions for use, reproduction, or distribution of Your modifications, or for any such Derivative Works as a whole, provided Your use, reproduction, and distribution of the Work otherwise complies with the conditions stated in this License.
            5. Submission of Contributions.
            Unless You explicitly state otherwise, any Contribution intentionally submitted for inclusion in the Work by You to the Licensor shall be under the terms and conditions of this License, without any additional terms or conditions. Notwithstanding the above, nothing herein shall supersede or modify the terms of any separate license agreement you may have executed with Licensor regarding such Contributions.
            6. Trademarks.
            This License does not grant permission to use the trade names, trademarks, service marks, or product names of the Licensor, except as required for reasonable and customary use in describing the origin of the Work and reproducing the content of the NOTICE file.
            7. Disclaimer of Warranty.
            Unless required by applicable law or agreed to in writing, Licensor provides the Work (and each Contributor provides its Contributions) on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied, including, without limitation, any warranties or conditions of TITLE, NON-INFRINGEMENT, MERCHANTABILITY, or FITNESS FOR A PARTICULAR PURPOSE. You are solely responsible for determining the appropriateness of using or redistributing the Work and assume any risks associated with Your exercise of permissions under this License.
            8. Limitation of Liability.
            In no event and under no legal theory, whether in tort (including negligence), contract, or otherwise, unless required by applicable law (such as deliberate and grossly negligent acts) or agreed to in writing, shall any Contributor be liable to You for damages, including any direct, indirect, special, incidental, or consequential damages of any character arising as a result of this License or out of the use or inability to use the Work (including but not limited to damages for loss of goodwill, work stoppage, computer failure or malfunction, or any and all other commercial damages or losses), even if such Contributor has been advised of the possibility of such damages.
            9. Accepting Warranty or Additional Liability.
            While redistributing the Work or Derivative Works thereof, You may choose to offer, and charge a fee for, acceptance of support, warranty, indemnity, or other liability obligations and/or rights consistent with this License. However, in accepting such obligations, You may act only on Your own behalf and on Your sole responsibility, not on behalf of any other Contributor, and only if You agree to indemnify, defend, and hold each Contributor harmless for any liability incurred by, or claims asserted against, such Contributor by reason of your accepting any such warranty or additional liability.
            END OF TERMS AND CONDITIONS
            APPENDIX: How to apply the Apache License to your work
            To apply the Apache License to your work, attach the following boilerplate notice, with the fields enclosed by brackets "[]" replaced with your own identifying information. (Don't include the brackets!) The text should be enclosed in the appropriate comment syntax for the file format. We also recommend that a file or class name and description of purpose be included on the same "printed page" as the copyright notice for easier identification within third-party archives.
            Copyright [yyyy] [name of copyright owner]
            Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
            http://www.apache.org/licenses/LICENSE-2.0
            Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
         */

        #endregion

        private readonly WebClient _Client;
        private readonly ILog _Log;
        private readonly AppSettings _Settings;
        private readonly AppContracts _Contracts;
        private string _Cfid;
        private string _OAuth;

        private const string CoverSize = "300"; //valid sizes: 300, 640

        /// <summary>Initializes a new SpotifyAPI object which can be used to recieve</summary>
        public SpotifyLocalApi(ILog log, AppContracts contracts, AppSettings settings) {
            
            //emulate the embed code [NEEDED]
            _Client = new WebClient();
            _Client.Headers.Add("Origin", "https://embed.spotify.com");
            _Client.Headers.Add("Referer", "https://embed.spotify.com/?uri=spotify:track:5Zp4SWOpbuOdnsxLqwgutt");
            _Client.Encoding = Encoding.UTF8;
            if (_Client.Proxy != null)
                _Client.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;

            _Log = log;
            _Settings = settings;
            _Contracts = contracts;

            RenewToken();
        }

        public void RenewToken() {
            try {
                //Reset fields
                _OAuth = string.Empty;
                _Cfid = string.Empty;

                _OAuth = GetOAuth();
                _Cfid = Cfid.Token;
            }
            catch (Exception exc) {
                _Log.WarnException("Failed to renew Spotify token", exc);
            }
        }

        public bool HasValidToken {
            get { return !string.IsNullOrEmpty(_OAuth) && !string.IsNullOrEmpty(_Cfid); }
        }

        /// <summary>Get a link to the 640x640 cover art image of a spotify album</summary>
        /// <param name="uri">The Spotify album URI</param>
        public string GetArt(string uri) {
            try {
                var albumId = uri.Split(new[] {":"}, StringSplitOptions.RemoveEmptyEntries).Last();
                var lines = _Client.DownloadString(string.Format("http://open.spotify.com/album/{0}", albumId))
                                   .Replace("\t", string.Empty)
                                   .Split(new[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);


                var coverId = (from line in lines
                               where line.StartsWith("<meta property=\"og:image\"")
                               select line.Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries))
                    .First() // item in resultset
                    .Last() // item in string array
                    .Replace("\"", "")
                    .Replace(">", "");

                return string.Format("http://o.scdn.co/{0}/{1}", CoverSize, coverId);
            }
            catch (WebException webException) {
                if (webException.Response != null && ((HttpWebResponse) webException.Response).StatusCode != HttpStatusCode.NotFound)
                    _Log.WarnException("[WebException] Failed to retrieve cover url from Spotify", webException);
            }
            catch(Exception exc) {
                _Log.WarnException("Failed to retrieve cover url from Spotify", exc);
            }
            return string.Empty;
        }


        /// <summary>Gets the current Unix Timestamp. Mostly for internal use</summary>
        private int TimeStamp {
            get {
                return Convert.ToInt32((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
            }
        }

        /// <summary>Gets the 'CFID', a unique identifier for the current session. Note: It's required to get the CFID before making any other calls.</summary>
        public Cfid Cfid {
            get {
                var a = SendLocalRequest("simplecsrf/token.json");
                var d = (List<Cfid>)JsonConvert.DeserializeObject(a, typeof(List<Cfid>));
                _Cfid = d[0].Token;
                return d[0];
            }
        }

        string _Uri = "";
        /// <summary>Used by SpotifyAPI.Play to play Spotify URI's. Change this URI and then call SpotifyAPI.Play</summary>
        public string Uri {
            get {
                return _Uri;
            }
            set {
                _Uri = value;
            }
        }

        /// <summary>Plays a certain URI and returns the status afterwards. Change SpotifyAPI.URI into the needed uri!</summary>
        public Status Play {
            get {
                var a = SendLocalRequest("remote/play.json?uri=" + Uri, true, true, -1);
                var d = (List<Status>)JsonConvert.DeserializeObject(a, typeof(List<Status>));
                return d.FirstOrDefault();
            }
        }

        /// <summary>Resume Spotify playback and return the status afterwards</summary>
        public Status Resume {
            get {
                var a = SendLocalRequest("remote/pause.json?pause=false", true, true, -1);
                var d = (List<Status>)JsonConvert.DeserializeObject(a, typeof(List<Status>));
                return d.FirstOrDefault();
            }
        }

        /// <summary>Pause Spotify playback and return the status afterwards</summary>
        public Status Pause {
            get {
                var a = SendLocalRequest("remote/pause.json?pause=true", true, true, -1);
                var d = (List<Status>)JsonConvert.DeserializeObject(a, typeof(List<Status>));
                return d.FirstOrDefault();
            }
        }

        /// <summary>Returns the current track info.
        /// Change <seealso cref="Wait"/> into the amount of waiting time before it will return
        /// When the current track info changes it will return before elapsing the amount of seconds in <seealso cref="Wait"/>
        /// (look at the project site for more information if you do not understand this)
        /// </summary>
        public Status Status {
            get {
                try {
                    var a = SendLocalRequest("remote/status.json", true, true, _Wait);
                    var d = (List<Status>) JsonConvert.DeserializeObject(a, typeof (List<Status>));

                    var result = d.FirstOrDefault();
                    if (result != null && result.Error != null && result.Error.Message.Contains("Invalid Csrf token")) {
                        RenewToken();

                        a = SendLocalRequest("remote/status.json", true, true, _Wait);
                        d = (List<Status>)JsonConvert.DeserializeObject(a, typeof(List<Status>));
                        result = d.FirstOrDefault();
                    }

                    return result;
                }
                catch (Exception exc) {
                    _Log.WarnException("Failed to get track status", exc);
                    return null;
                }
            }
        }

        int _Wait = -1;
        /// <summary>
        /// Please see <seealso cref="Status"/> for more information
        /// </summary>
        public int Wait {
            get {
                return _Wait;
            }
            set {
                _Wait = value;
            }
        }

        /// <summary>Recieves a OAuth key from the Spotify site</summary>
        private string GetOAuth() {
            var lines = _Client.DownloadString("https://embed.spotify.com/openplay/?uri=spotify:track:5Zp4SWOpbuOdnsxLqwgutt")
                                     .Replace(" ", "")
                                     .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var l in
                    from line in lines
                    where line.StartsWith("tokenData")
                    select line.Split(new[] { "'" }, StringSplitOptions.None))
                return l[1];

            throw new Exception("Could not find OAuth token");
        }


        private string SendLocalRequest(string request) {
            return SendLocalRequest(request, false, false, -1);
        }

        private string SendLocalRequest(string request, bool oauth, bool cfid) {
            return SendLocalRequest(request, oauth, cfid, -1);
        }

        private string SendLocalRequest(string request, bool oauth, bool cfid, int wait) {
            var parameters = "?&ref=&cors=&_=" + TimeStamp;
            if (request.Contains("?"))
                parameters = parameters.Substring(1);

            if (oauth)
                parameters += "&oauth=" + _OAuth;
            if (cfid)
                parameters += "&csrf=" + _Cfid;

            if (wait != -1) {
                parameters += "&returnafter=" + wait;
                parameters += "&returnon=login%2Clogout%2Cplay%2Cpause%2Cerror%2Cap";
            }

            var requestUri = "http://" + _Contracts.SpotifyLocalHost + ":4380/" + request + parameters;
            var response = string.Empty;
            try {
                response = _Client.DownloadString(requestUri);
                response = "[ " + response + " ]";
            }
            catch (Exception wExc) {
                _Log.WarnException("SendLocalRequest failed", wExc);
                //perhaps spotifywebhelper isn't started (happens sometimes)
                if (Process.GetProcessesByName("SpotifyWebHelper").Length < 1) {
                    try {
                        Process.Start(
                            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Spotify", "Data","SpotifyWebHelper.exe"));
                    }
                    catch (Exception exc) {
                        _Log.WarnException("Failed to start the Spotify webhelper", exc);
                        throw new Exception("Could not launch SpotifyWebHelper. Your installation of Spotify might be corrupt or you might not have Spotify installed", exc);
                    }

                    return SendLocalRequest(request, oauth, cfid);
                }
                //spotifywebhelper is running but we still can't connect, wtf?!
                throw new Exception("Unable to connect to SpotifyWebHelper", wExc);
            }
            return response;
        }

        /// <summary>Recieves client version information. Doesn't require a OAuth/CFID</summary>
        public ClientVersion ClientVersion {
            get {
                var a = SendLocalRequest("service/version.json?service=remote");
                var d = (List<ClientVersion>)JsonConvert.DeserializeObject(a, typeof(List<ClientVersion>));
                return d.FirstOrDefault();
            }
        }
    }
}
