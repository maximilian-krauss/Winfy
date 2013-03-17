using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winfy.Core {
    public interface ILog {

        void Info(string message);
        void InfoException(string message, Exception exception);

        void Warn(string message);
        void WarnException(string message, Exception exception);

        void Fatal(string message);
        void FatalException(string message, Exception exception);
    }
}
