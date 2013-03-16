using System.Net;
using System.Web;
using NLog;
using System;
using System.Timers;

namespace Winfy.Core {
    public class UsageTrackerService : IUsageTrackerService {

        private readonly AppSettings _Settings;
        private readonly AppContracts _Contracts;
        private readonly Logger _Logger;
        private Timer _DelayedTrack;

        protected virtual string UsageTrackerUrl { get { return "http://ping.krausshq.com"; } }

        public UsageTrackerService(AppSettings settings, Logger logger, AppContracts contracts) {
            _Settings = settings;
            _Contracts = contracts;
            _Logger = logger;
        }

        public void Track() {
            _DelayedTrack = new Timer(30*1000 /*1 sec delay*/) { AutoReset = true, Enabled = true};
            _DelayedTrack.Elapsed += (o, e) => {
                                        try {
                                            TrackUsageInternal();
                                            _DelayedTrack.Stop();
                                            _DelayedTrack.Dispose();
                                        }
                                        catch (Exception exc) {
                                            _Logger.WarnException("Failed to track usage", exc);
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
