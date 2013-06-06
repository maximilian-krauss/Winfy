using System;

namespace Winfy.Core.Broadcast {
    public interface IBroadcastService {
        event EventHandler<BroadcastMessageReceivedEventArgs> BroadcastMessageReceived;
        void StartListening();
        void StopListening();
    }
}