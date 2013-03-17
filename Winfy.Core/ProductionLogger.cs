using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winfy.Core {
    public sealed class ProductionLogger : ILog {
        private readonly Logger _Logger;

        public ProductionLogger() {
            _Logger = LogManager.GetCurrentClassLogger();
            if (_Logger == null) {
                throw new Exception("Could not initialize NLog");
            }
        }

        public void Info(string message) {
            _Logger.Info(message);
        }

        public void InfoException(string message, Exception exception) {
            _Logger.InfoException(message, exception);
        }

        public void Warn(string message) {
            _Logger.Warn(message);
        }

        public void WarnException(string message, Exception exception) {
            _Logger.WarnException(message, exception);
        }

        public void Fatal(string message) {
            _Logger.Fatal(message);
        }

        public void FatalException(string message, Exception exception) {
            _Logger.FatalException(message, exception);
        }
    }
}
