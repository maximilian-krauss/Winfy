using System.Collections.Generic;
using Caliburn.Micro;
using System;

namespace Winfy.Core {
	public sealed class AppSettings : PropertyChangedBase {

		public AppSettings() {
			Positions = new List<WindowPosition>();
			ReadBroadcastMessageIds = new List<string>();
			UniqueApplicationIdentifier = Guid.NewGuid().ToString();
			ApplicationSize = ApplicationSize.Medium;
		}

		private bool _AlwaysOnTop;
		public bool AlwaysOnTop {
			get { return _AlwaysOnTop; }
			set { _AlwaysOnTop = value; NotifyOfPropertyChange(() => AlwaysOnTop); }
		}

		private bool _StartWithWindows;
		public bool StartWithWindows {
			get { return _StartWithWindows; }
			set { _StartWithWindows = value; NotifyOfPropertyChange(() => StartWithWindows); }
		}

		private bool _HideIfSpotifyClosed;
		public bool HideIfSpotifyClosed {
			get { return _HideIfSpotifyClosed; }
			set { _HideIfSpotifyClosed = value; NotifyOfPropertyChange(() => HideIfSpotifyClosed); }
		}

		private bool _DisableAnimations;
		public bool DisableAnimations {
			get { return _DisableAnimations; }
			set { _DisableAnimations = value; NotifyOfPropertyChange(() => DisableAnimations); }
		}

		public List<WindowPosition> Positions { get; set; }
		public List<string> ReadBroadcastMessageIds { get; set; }
		public string UniqueApplicationIdentifier { get; set; }

		private ApplicationSize _ApplicationSize;
		public ApplicationSize ApplicationSize {
			get { return _ApplicationSize; }
			set { _ApplicationSize = value; NotifyOfPropertyChange(() => ApplicationSize); }
		}

	}
}