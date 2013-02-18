using System;

namespace Winfy.Core.Deployment {
    public interface IUpdateController {
        event EventHandler<UpdateReadyEventArgs> UpdateReady;

        void StartBackgroundCheck();
        void StopBackgroundCheck();
        void Restart();
    }
}