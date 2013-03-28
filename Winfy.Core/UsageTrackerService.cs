using System.Net;
using System.Web;
using System;
using System.Timers;

namespace Winfy.Core {
    public class UsageTrackerService : IUsageTrackerService {

        private readonly AppSettings _Settings;
        private readonly AppContracts _Contracts;
        private readonly ILog _Logger;
        private Timer _DelayedTrack;
        private int _Tries;

        protected virtual string UsageTrackerUrl { get { return "http://ping.krausshq.com"; } }

        public UsageTrackerService(AppSettings settings, ILog logger, AppContracts contracts) {
            _Settings = settings;
            _Contracts = contracts;
            _Logger = logger;
        }

        public void Track() {
            _DelayedTrack = new Timer(30*1000 /*30 sec delay*/) { AutoReset = true, Enabled = true};
            _DelayedTrack.Elapsed += (o, e) => {
                                        try {
                                            if (_Tries > 10) {
                                                _DelayedTrack.Stop();
                                            }

                                            TrackUsageInternal();
                                            _DelayedTrack.Stop();
                                            _DelayedTrack.Dispose();
                                        }
                                        catch (Exception exc) {
                                            _Logger.WarnException("Failed to track usage", exc);
                                            _Tries++;
                                        }
                                     };

        }

        private void TrackUsageInternal() {
            var request = Helper.CreateWebRequest(UsageTrackerUrl);
            request.Method = WebRequestMethods.Http.Post;
            request.Headers.Add("X-Khq-Os", Environment.OSVersion.VersionString);
            request.Headers.Add("X-Khq-App", _Contracts.ApplicationName);
            request.Headers.Add("X-Khq-Version", _Contracts.ApplicationVersion.ToString());
            request.Headers.Add("X-Khq-Id", _Settings.UniqueApplicationIdentifier);

            var response = (HttpWebResponse)request.GetResponse();
            try {
                if(response.StatusCode != HttpStatusCode.OK)
                    throw new HttpException((int)response.StatusCode, "Usage tracking failed");
            }
            finally {
                response.Close();    
            }
        }

    }
}
