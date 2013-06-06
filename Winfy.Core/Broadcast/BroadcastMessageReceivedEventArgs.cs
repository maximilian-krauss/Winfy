using System;

namespace Winfy.Core.Broadcast {
    public sealed class BroadcastMessageReceivedEventArgs : EventArgs {
        public BroadcastMessageReceivedEventArgs(BroadcastMessage message) {
            Message = message;
        }

        public BroadcastMessage Message { get; private set; }

    }
}
