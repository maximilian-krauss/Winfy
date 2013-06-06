using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Timers;

namespace Winfy.Core.Broadcast {
    public sealed class BroadcastService : IBroadcastService {

        private readonly AppSettings _Settings;
        private readonly AppContracts _Contracts;
        private readonly ILog _Log;

        private Timer _BroadcastTimer;
        private DateTime _LastTimeChecked;
        private bool _CheckLock;

        public event EventHandler<BroadcastMessageReceivedEventArgs> BroadcastMessageReceived;

        public BroadcastService(AppSettings settings, AppContracts contracts, ILog log) {
            _Settings = settings;
            _Contracts = contracts;
            _Log = log;
            _LastTimeChecked = DateTime.MinValue;
        }

        private void OnBroadcastMessageReceived(BroadcastMessageReceivedEventArgs e) {
            var handler = BroadcastMessageReceived;
            if (handler != null) handler(this, e);
        }

        public void StartListening() {
            _BroadcastTimer = new Timer(1000) {AutoReset = true, Enabled = true};
            _BroadcastTimer.Elapsed += (o, e) => {
                                           if (DateTime.Now.Subtract(_LastTimeChecked).TotalHours < 1 || _CheckLock)
                                               return;

                                           try {
                                               _CheckLock = true;
                                               CheckBroadcast();
                                           }
                                           finally {
                                               _LastTimeChecked = DateTime.Now;
                                               _CheckLock = false;
                                           }
                                       };
        }

        private void CheckBroadcast() {
            try {
                List<BroadcastMessage> broadcastMessages;
                var request = Helper.CreateWebRequest(_Contracts.BroadcastUrl);
                var response = (HttpWebResponse) request.GetResponse();
                using (var responseStream = response.GetResponseStream())
                    broadcastMessages = Serializer.DeserializeFromJson<List<BroadcastMessage>>(responseStream);
                response.Close();

                if (broadcastMessages.Count == 0)
                    return;

                var newMessages = broadcastMessages.Where(m =>
                        m.Active &&
                        _Contracts.ApplicationVersion <= new Version(m.AffectedVersion) &&
                        _Settings.ReadBroadcastMessageIds.All(id => id != m.Id)).ToList();

                if(newMessages.Any())
                    OnBroadcastMessageReceived(new BroadcastMessageReceivedEventArgs(newMessages.First()));
            }
            catch (Exception exc) {
                _Log.WarnException("CheckBroadcast failed", exc);
            }
        }

        public void StopListening() {
            if (_BroadcastTimer != null) {
                _BroadcastTimer.Stop();
                _BroadcastTimer.Dispose();
            }
        }
    }
}
