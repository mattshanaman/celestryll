using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Threading.Tasks;

namespace AmbientSleeper.Services;

public interface IAudioService : IDisposable
{
    // Mix Mode: start a loop for a track key with a path resolved on disk
    Task PlayLoopAsync(string key, string absolutePath, double volume);

    // Non-loop track with completion callback (for Playlist Mode)
    Task PlayOnceAsync(string key, string absolutePath, double volume, Action onCompleted);

    // Adjust volume for a specific key (if playing)
    void SetVolume(string key, double volume);

    // Stop a specific key
    void Stop(string key);

    // Fade out all playing tracks, then return
    Task FadeOutAllAsync(TimeSpan duration);

    // Stop and dispose all players
    void StopAll();

    // Returns true if any players exist/playing
    bool IsAnyPlaying { get; }
}

