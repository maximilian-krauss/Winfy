using NLog;

namespace Winfy.Core {
    public sealed class LocalUsageTrackerService : UsageTrackerService {
        public LocalUsageTrackerService(AppSettings settings, ILog logger, AppContracts contracts) : base(settings, logger, contracts) {
        }
        
        protected override string UsageTrackerUrl {
            get { return "http://localhost"; }
        }
    }
}
