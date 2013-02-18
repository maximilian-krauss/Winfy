using System;

namespace Winfy.Core.Deployment {
    public interface IDeployment {
        event EventHandler<UpdateReadyEventArgs> UpdateReady;
        void Update();
        void Restart();
    }
}