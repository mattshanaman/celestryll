using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

#if ANDROID
using Android.Media;
#endif
#if IOS
using AVFoundation;
using Foundation;
#endif

namespace AmbientSleeper.Services;

public sealed class AudioService : IAudioService
{
    private sealed class Handle : IDisposable
    {
#if ANDROID
        public MediaPlayer? APlayer;
#endif
#if IOS
        public AVAudioPlayer? IPlayer;
#endif
        public string Path = string.Empty;

        public void Dispose()
        {
#if ANDROID
            var ap = APlayer;
            if (ap != null)
            {
                try { if (ap.IsPlaying) ap.Stop(); } catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"AudioService: Error stopping Android player: {ex.Message}"); }
                
                try
                {
                    ap.Release();
                    ap.Dispose();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"AudioService: Error releasing Android player: {ex.Message}");
                }
                APlayer = null;
            }
#endif
#if IOS
            var ip = IPlayer;
            if (ip != null)
            {
                try { if (ip.Playing) ip.Stop(); } catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"AudioService: Error stopping iOS player: {ex.Message}"); }
                
                try
                {
                    ip.Dispose();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"AudioService: Error disposing iOS player: {ex.Message}");
                }
                IPlayer = null;
            }
#endif
            TryDeleteTemp(Path);
        }
    }

    private readonly ConcurrentDictionary<string, Handle> _players = new();
    private static ILogger? _logger;

    public static void SetLogger(ILogger logger) => _logger = logger;

    public bool IsAnyPlaying => _players.Count > 0;

    public async Task PlayLoopAsync(string key, string absolutePath, double volume)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            LogWarning("PlayLoopAsync called with empty key");
            return;
        }

        if (string.IsNullOrWhiteSpace(absolutePath))
        {
            LogWarning($"PlayLoopAsync called with empty path for key '{key}'");
            return;
        }

        if (!File.Exists(absolutePath))
        {
            LogError($"PlayLoopAsync: File not found for key '{key}': {absolutePath}", null);
            throw new FileNotFoundException($"Audio file not found: {absolutePath}", absolutePath);
        }

        try
        {
            Stop(key);
            var h = new Handle { Path = absolutePath };
#if ANDROID
            var p = new MediaPlayer();
            try
            {
                p.SetDataSource(absolutePath);
                p.Looping = true;
                p.SetVolume((float)Math.Clamp(volume, 0.0, 1.0), (float)Math.Clamp(volume, 0.0, 1.0));
                p.Prepare();
                p.Start();
                h.APlayer = p;
            }
            catch (Exception ex)
            {
                p.Dispose();
                LogError($"PlayLoopAsync: Android playback failed for key '{key}'", ex);
                throw new InvalidOperationException($"Failed to play audio file: {absolutePath}", ex);
            }
#endif
#if IOS
            var url = NSUrl.FromFilename(absolutePath);
            var p = AVAudioPlayer.FromUrl(url);
            if (p == null)
            {
                LogError($"PlayLoopAsync: iOS AVAudioPlayer.FromUrl returned null for key '{key}': {absolutePath}", null);
                throw new InvalidOperationException($"Failed to create audio player for file: {absolutePath}");
            }
            
            try
            {
                p.NumberOfLoops = -1;
                p.Volume = (float)Math.Clamp(volume, 0.0, 1.0);
                p.PrepareToPlay();
                p.Play();
                h.IPlayer = p;
            }
            catch (Exception ex)
            {
                p.Dispose();
                LogError($"PlayLoopAsync: iOS playback failed for key '{key}'", ex);
                throw new InvalidOperationException($"Failed to play audio file: {absolutePath}", ex);
            }
#endif
            _players[key] = h;
        }
        catch (Exception ex) when (ex is not InvalidOperationException && ex is not FileNotFoundException)
        {
            LogError($"PlayLoopAsync: Unexpected error for key '{key}'", ex);
            throw new InvalidOperationException($"Unexpected error playing audio file: {absolutePath}", ex);
        }

        await Task.CompletedTask;
    }

    public async Task PlayOnceAsync(string key, string absolutePath, double volume, Action onCompleted)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            LogWarning("PlayOnceAsync called with empty key");
            return;
        }

        if (string.IsNullOrWhiteSpace(absolutePath))
        {
            LogWarning($"PlayOnceAsync called with empty path for key '{key}'");
            return;
        }

        if (!File.Exists(absolutePath))
        {
            LogError($"PlayOnceAsync: File not found for key '{key}': {absolutePath}", null);
            throw new FileNotFoundException($"Audio file not found: {absolutePath}", absolutePath);
        }

        if (onCompleted == null)
        {
            LogWarning($"PlayOnceAsync called with null onCompleted callback for key '{key}'");
        }

        try
        {
            Stop(key);
            var h = new Handle { Path = absolutePath };
#if ANDROID
            var p = new MediaPlayer();
            try
            {
                p.SetDataSource(absolutePath);
                p.Looping = false;
                p.SetVolume((float)Math.Clamp(volume, 0.0, 1.0), (float)Math.Clamp(volume, 0.0, 1.0));
                p.Completion += (s, e) =>
                {
                    try
                    {
                        onCompleted?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        LogError($"PlayOnceAsync: onCompleted callback failed for key '{key}'", ex);
                    }
                    Stop(key);
                };
                p.Prepare();
                p.Start();
                h.APlayer = p;
            }
            catch (Exception ex)
            {
                p.Dispose();
                LogError($"PlayOnceAsync: Android playback failed for key '{key}'", ex);
                throw new InvalidOperationException($"Failed to play audio file: {absolutePath}", ex);
            }
#endif
#if IOS
            var url = NSUrl.FromFilename(absolutePath);
            var p = AVAudioPlayer.FromUrl(url);
            if (p == null)
            {
                LogError($"PlayOnceAsync: iOS AVAudioPlayer.FromUrl returned null for key '{key}': {absolutePath}", null);
                throw new InvalidOperationException($"Failed to create audio player for file: {absolutePath}");
            }
            
            try
            {
                p.NumberOfLoops = 0;
                p.Volume = (float)Math.Clamp(volume, 0.0, 1.0);
                p.FinishedPlaying += (s, e) =>
                {
                    try
                    {
                        onCompleted?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        LogError($"PlayOnceAsync: onCompleted callback failed for key '{key}'", ex);
                    }
                    Stop(key);
                };
                p.PrepareToPlay();
                p.Play();
                h.IPlayer = p;
            }
            catch (Exception ex)
            {
                p.Dispose();
                LogError($"PlayOnceAsync: iOS playback failed for key '{key}'", ex);
                throw new InvalidOperationException($"Failed to play audio file: {absolutePath}", ex);
            }
#endif
            _players[key] = h;
        }
        catch (Exception ex) when (ex is not InvalidOperationException && ex is not FileNotFoundException)
        {
            LogError($"PlayOnceAsync: Unexpected error for key '{key}'", ex);
            throw new InvalidOperationException($"Unexpected error playing audio file: {absolutePath}", ex);
        }

        await Task.CompletedTask;
    }

    public void SetVolume(string key, double volume)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            LogWarning("SetVolume called with empty key");
            return;
        }

        if (!_players.TryGetValue(key, out var h))
        {
            LogWarning($"SetVolume: Player not found for key '{key}'");
            return;
        }

        try
        {
            var clampedVolume = Math.Clamp(volume, 0.0, 1.0);
#if ANDROID
            h.APlayer?.SetVolume((float)clampedVolume, (float)clampedVolume);
#endif
#if IOS
            var ip = h.IPlayer;
            if (ip != null) ip.Volume = (float)clampedVolume;
#endif
        }
        catch (Exception ex)
        {
            LogError($"SetVolume: Failed for key '{key}'", ex);
        }
    }

    public async Task FadeOutAllAsync(TimeSpan duration)
    {
        if (_players.Count == 0)
        {
            return;
        }

        if (duration <= TimeSpan.Zero)
        {
            LogWarning("FadeOutAllAsync called with zero or negative duration");
            return;
        }

        try
        {
            const int steps = 20;
            var delay = TimeSpan.FromMilliseconds(Math.Max(25, duration.TotalMilliseconds / steps));

            // Capture initial volumes as 1.0; we'll decrement linearly
            for (int i = steps - 1; i >= 0; i--)
            {
                var v = i / (double)steps;
                foreach (var h in _players.Values)
                {
                    try
                    {
#if ANDROID
                        h.APlayer?.SetVolume((float)v, (float)v);
#endif
#if IOS
                        var ip = h.IPlayer;
                        if (ip != null) ip.Volume = (float)v;
#endif
                    }
                    catch (Exception ex)
                    {
                        LogError("FadeOutAllAsync: Error setting volume during fade", ex);
                    }
                }
                await Task.Delay(delay).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            LogError("FadeOutAllAsync: Unexpected error", ex);
        }
    }

    public void Stop(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            LogWarning("Stop called with empty key");
            return;
        }

        try
        {
            if (_players.TryRemove(key, out var h))
            {
                h.Dispose();
            }
        }
        catch (Exception ex)
        {
            LogError($"Stop: Error stopping player for key '{key}'", ex);
        }
    }

    public void StopAll()
    {
        try
        {
            foreach (var kv in _players)
            {
                try
                {
                    kv.Value.Dispose();
                }
                catch (Exception ex)
                {
                    LogError($"StopAll: Error disposing player for key '{kv.Key}'", ex);
                }
            }
            _players.Clear();
        }
        catch (Exception ex)
        {
            LogError("StopAll: Unexpected error", ex);
        }
    }

    public void Dispose() => StopAll();

    private static void TryDeleteTemp(string path)
    {
        // Delete only if inside CacheDirectory; bundled files are not here
        try
        {
            if (!string.IsNullOrWhiteSpace(path)
                && path.StartsWith(FileSystem.CacheDirectory, StringComparison.OrdinalIgnoreCase)
                && File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"AudioService: TryDeleteTemp failed for '{path}': {ex.Message}");
        }
    }

    private static void LogError(string message, Exception? ex)
    {
        var fullMessage = $"AudioService: {message}";
        if (ex != null)
        {
            _logger?.LogError(ex, fullMessage);
            System.Diagnostics.Debug.WriteLine($"ERROR: {fullMessage}");
            System.Diagnostics.Debug.WriteLine($"  Exception: {ex}");
        }
        else
        {
            _logger?.LogError(fullMessage);
            System.Diagnostics.Debug.WriteLine($"ERROR: {fullMessage}");
        }
    }

    private static void LogWarning(string message)
    {
        var fullMessage = $"AudioService: {message}";
        _logger?.LogWarning(fullMessage);
        System.Diagnostics.Debug.WriteLine($"WARNING: {fullMessage}");
    }
}
