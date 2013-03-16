using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace Winfy.Core {
    public sealed class LocalUsageTrackerService : UsageTrackerService {
        public LocalUsageTrackerService(AppSettings settings, Logger logger, AppContracts contracts) : base(settings, logger, contracts) {
        }
        
        /*protected override string UsageTrackerUrl {
            get { return "http://localhost"; }
        }*/
    }
}
