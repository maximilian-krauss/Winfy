using System;

namespace Winfy.Core.Deployment {
    public interface IUpdateService {
        event EventHandler<UpdateReadyEventArgs> UpdateReady;

        void StartBackgroundCheck();
        void StopBackgroundCheck();
        void Restart();
    }
}