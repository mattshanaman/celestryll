using AmbientSleeper.Models;

namespace AmbientSleeper.Services;

public interface IPlaybackOrchestrator
{
    // Mix Mode
    Task StartMixAsync(IEnumerable<AudioItem> items);
    Task UpdateMixVolumeAsync(string itemId, double volume);
    Task StopAllWithFadeAsync(TimeSpan fade);

    // Playlist Mode
    Task StartPlaylistAsync(IEnumerable<AudioItem> items, bool loopPlaylist);
    Task StopPlaylistAsync(TimeSpan fade);

    bool IsPlaying { get; }
}