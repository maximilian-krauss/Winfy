using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winfy.Core.SpotifyLocal;

namespace Winfy.Core {
    public interface ISpotifyController : IDisposable {
        event EventHandler TrackChanged;
        event EventHandler SpotifyOpened;
        event EventHandler SpotifyExited;
        bool IsSpotifyOpen();
        bool IsSpotifyInstalled();
        string GetSongName();
        string GetArtistName();
        Status GetStatus();
        void PausePlay();
        void NextTrack();
        void PreviousTrack();
        void VolumeUp();
        void VolumeDown();
    }
}