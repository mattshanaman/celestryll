using AmbientSleeper.Models;

namespace AmbientSleeper.Services;

public class PlaybackOrchestrator : IPlaybackOrchestrator
{
    private readonly IAudioService _audio;
    private readonly object _lock = new();
    private List<AudioItem> _playlist = new();
    private int _playlistIndex = 0;
    private bool _loopPlaylist = true;
    private bool _playlistActive = false;

    public bool IsPlaying => _audio.IsAnyPlaying;

    public PlaybackOrchestrator(IAudioService audio)
    {
        _audio = audio;
    }

    // Resolve an AudioItem (bundled or device) to an absolute playable file path
    private static async Task<string> ResolvePathAsync(AudioItem item)
    {
        if (item.SourceType == AudioSourceType.Bundled && item.BundledFileName != null)
        {
            // Copy bundled asset to cache so MediaPlayer/AVAudioPlayer can open by path
            await using var src = await FileSystem.OpenAppPackageFileAsync(item.BundledFileName).ConfigureAwait(false);
            var dst = Path.Combine(FileSystem.CacheDirectory, $"{Guid.NewGuid()}_{item.BundledFileName}");
            await using var fs = File.Create(dst);
            await src.CopyToAsync(fs).ConfigureAwait(false);
            return dst;
        }

        if (!string.IsNullOrWhiteSpace(item.DevicePath))
        {
            // If you got a stream from FilePicker, copy it to cache and set DevicePath to that location
            return item.DevicePath!;
        }

        throw new InvalidOperationException("AudioItem has no resolvable path.");
    }

    // MIX MODE: simultaneous looping
    public async Task StartMixAsync(IEnumerable<AudioItem> items)
    {
        await StopAllInternalAsync(TimeSpan.Zero).ConfigureAwait(false);

        foreach (var it in items)
        {
            var path = await ResolvePathAsync(it).ConfigureAwait(false);
            await _audio.PlayLoopAsync(it.Id, path, it.Volume).ConfigureAwait(false);
        }
    }

    public Task UpdateMixVolumeAsync(string itemId, double volume)
    {
        _audio.SetVolume(itemId, volume);
        return Task.CompletedTask;
    }

    // PLAYLIST MODE: sequential playback, looping playlist
    public async Task StartPlaylistAsync(IEnumerable<AudioItem> items, bool loopPlaylist)
    {
        await StopAllInternalAsync(TimeSpan.Zero).ConfigureAwait(false);

        _playlist = items.ToList();
        _playlistIndex = 0;
        _loopPlaylist = loopPlaylist;
        _playlistActive = _playlist.Count > 0;

        if (!_playlistActive) return;
        await PlayCurrentAsync().ConfigureAwait(false);
    }

    private async Task PlayCurrentAsync()
    {
        if (!_playlistActive || _playlistIndex < 0 || _playlistIndex >= _playlist.Count) return;

        var item = _playlist[_playlistIndex];
        var path = await ResolvePathAsync(item).ConfigureAwait(false);

        // Play once; on completion advance to next
        await _audio.PlayOnceAsync("playlist", path, item.Volume, async () =>
        {
            await OnPlaylistTrackCompletedAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    private async Task OnPlaylistTrackCompletedAsync()
    {
        lock (_lock)
        {
            _playlistIndex++;
            if (_playlistIndex >= _playlist.Count)
            {
                if (_loopPlaylist)
                    _playlistIndex = 0;
                else
                    _playlistActive = false;
            }
        }

        if (_playlistActive)
            await PlayCurrentAsync().ConfigureAwait(false);
    }

    public async Task StopPlaylistAsync(TimeSpan fade)
        => await StopAllInternalAsync(fade).ConfigureAwait(false);

    public async Task StopAllWithFadeAsync(TimeSpan fade)
        => await StopAllInternalAsync(fade).ConfigureAwait(false);

    private async Task StopAllInternalAsync(TimeSpan fade)
    {
        if (fade > TimeSpan.Zero)
            await _audio.FadeOutAllAsync(fade).ConfigureAwait(false);
        _audio.StopAll();
        _playlistActive = false;
        _playlist.Clear();
        _playlistIndex = 0;
    }
}
