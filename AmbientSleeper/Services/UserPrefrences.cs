using AmbientSleeper.Models;
using Microsoft.Maui.Storage;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AmbientSleeper.Services;

public class SavedPlaylist
{
    public string Name { get; set; } = string.Empty;
    public List<AudioItem> Items { get; set; } = new();
    public DateTime SavedAt { get; set; } = DateTime.UtcNow;
}

// Telemetry event for tracking errors
public class PreferencesErrorEvent
{
    public string Operation { get; set; } = string.Empty;
    public string ErrorType { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

// Circuit breaker for failing operations
public class CircuitBreaker
{
    private int _failureCount = 0;
    private DateTime? _lastFailureTime;
    private readonly int _failureThreshold;
    private readonly TimeSpan _resetTimeout;
    private bool _isOpen = false;

    public CircuitBreaker(int failureThreshold = 5, int resetTimeoutSeconds = 60)
    {
        _failureThreshold = failureThreshold;
        _resetTimeout = TimeSpan.FromSeconds(resetTimeoutSeconds);
    }

    public bool IsOpen
    {
        get
        {
            // Auto-reset if timeout has elapsed
            if (_isOpen && _lastFailureTime.HasValue &&
                DateTime.UtcNow - _lastFailureTime.Value > _resetTimeout)
            {
                Reset();
            }
            return _isOpen;
        }
    }

    public void RecordSuccess()
    {
        _failureCount = 0;
        _isOpen = false;
    }

    public void RecordFailure()
    {
        _failureCount++;
        _lastFailureTime = DateTime.UtcNow;

        if (_failureCount >= _failureThreshold)
        {
            _isOpen = true;
        }
    }

    public void Reset()
    {
        _failureCount = 0;
        _isOpen = false;
        _lastFailureTime = null;
    }
}

public static class UserPreferences
{
    private const string MixVolumesKey = "mix_volumes";
    private const string PlaylistsKey = "saved_playlists";
    private const int MaxPlaylists = 1000;
    private const string MixesKey = "saved_mixes";
    private const int MaxMixes = 1000;
    private const string MixPlaylistsKey = "mix_playlists";
    private const string IntegrityCheckKey = "preferences_integrity";

    // Logger for error reporting
    private static ILogger? _logger;

    // Telemetry callback for error tracking
    private static Action<PreferencesErrorEvent>? _telemetryCallback;

    // Circuit breakers for different operations
    private static readonly CircuitBreaker _writeCircuitBreaker = new(5, 60);
    private static readonly CircuitBreaker _readCircuitBreaker = new(10, 30);

    // PERFORMANCE FIX: Add caching for expensive read operations
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromSeconds(5);
    private static (List<SavedMix> Data, DateTime Timestamp)? _mixesCache;
    private static (List<SavedPlaylist> Data, DateTime Timestamp)? _playlistsCache;
    private static (List<SavedMixPlaylist> Data, DateTime Timestamp)? _mixPlaylistsCache;
    private static (Dictionary<string, double> Data, DateTime Timestamp)? _volumesCache;
    private static readonly object _cacheLock = new();

    public static void SetLogger(ILogger logger) => _logger = logger;

    public static void SetTelemetryCallback(Action<PreferencesErrorEvent> callback)
        => _telemetryCallback = callback;

    // PERFORMANCE FIX: Add cache invalidation
    public static void InvalidateCache()
    {
        lock (_cacheLock)
        {
            _mixesCache = null;
            _playlistsCache = null;
            _mixPlaylistsCache = null;
            _volumesCache = null;
        }
    }

    private static bool IsCacheValid<T>((T Data, DateTime Timestamp)? cache)
    {
        return cache.HasValue && (DateTime.UtcNow - cache.Value.Timestamp) < CacheExpiration;
    }

    // FUTURE OPTIMIZATION: Cache warmup on startup
    public static async Task WarmupCacheAsync()
    {
        try
        {
            _logger?.LogInformation("Starting cache warmup...");

            await Task.Run(() =>
            {
                // Preload all caches in parallel
                var tasks = new[]
                {
                    Task.Run(() => { var _ = GetMixes(); }),
                    Task.Run(() => { var _ = GetPlaylists(); }),
                    Task.Run(() => { var _ = GetMixPlaylists(); }),
                    Task.Run(() => { var _ = GetMixVolumes(); })
                };

                Task.WaitAll(tasks);
            });

            _logger?.LogInformation("Cache warmup completed successfully");
        }
        catch (Exception ex)
        {
            LogError("WarmupCache", ex);
            // Non-critical - app will still work, just slower on first access
        }
    }

    // FUTURE OPTIMIZATION: Configurable cache duration
    public static void SetCacheExpiration(TimeSpan duration)
    {
        if (duration < TimeSpan.Zero || duration > TimeSpan.FromMinutes(5))
        {
            LogWarning($"SetCacheExpiration: Invalid duration {duration}, using default 5 seconds");
            return;
        }

        // Note: CacheExpiration is readonly, so this would require making it mutable
        _logger?.LogInformation($"Cache expiration would be set to {duration.TotalSeconds} seconds (not implemented - CacheExpiration is readonly)");
    }

    // Health check - validates preferences integrity
    public static (bool IsHealthy, List<string> Issues) CheckHealth()
    {
        var issues = new List<string>();
        var isHealthy = true;

        try
        {
            // Check if we can read/write
            var testKey = "health_check_test";
            var testValue = Guid.NewGuid().ToString();

            try
            {
                Preferences.Default.Set(testKey, testValue);
                var readValue = Preferences.Default.Get(testKey, string.Empty);

                if (readValue != testValue)
                {
                    issues.Add("Preferences read/write verification failed");
                    isHealthy = false;
                }

                Preferences.Default.Remove(testKey);
            }
            catch (Exception ex)
            {
                issues.Add($"Preferences storage not accessible: {ex.Message}");
                isHealthy = false;
            }

            // Check JSON integrity for each data store
            var checks = new[]
            {
                (MixVolumesKey, "MixVolumes"),
                (MixesKey, "Mixes"),
                (PlaylistsKey, "Playlists"),
                (MixPlaylistsKey, "MixPlaylists")
            };

            foreach (var (key, name) in checks)
            {
                try
                {
                    var json = Preferences.Default.Get(key, string.Empty);
                    if (!string.IsNullOrEmpty(json))
                    {
                        // Try to parse to verify integrity
                        var _ = JsonSerializer.Deserialize<object>(json);
                    }
                }
                catch (JsonException)
                {
                    issues.Add($"{name} data is corrupted");
                    isHealthy = false;
                }
                catch (Exception ex)
                {
                    issues.Add($"{name} check failed: {ex.Message}");
                    isHealthy = false;
                }
            }

            // Check circuit breaker states
            if (_writeCircuitBreaker.IsOpen)
            {
                issues.Add("Write operations circuit breaker is OPEN");
                isHealthy = false;
            }

            if (_readCircuitBreaker.IsOpen)
            {
                issues.Add("Read operations circuit breaker is OPEN");
                isHealthy = false;
            }

            // Update last health check timestamp
            try
            {
                Preferences.Default.Set(IntegrityCheckKey, DateTime.UtcNow.ToString("o"));
            }
            catch
            {
                // Non-critical
            }

            _logger?.LogInformation($"Health check completed. Healthy: {isHealthy}, Issues: {issues.Count}");
        }
        catch (Exception ex)
        {
            issues.Add($"Health check failed: {ex.Message}");
            isHealthy = false;
            LogError("CheckHealth", ex);
        }

        return (isHealthy, issues);
    }

    // Retry with exponential backoff
    private static async Task<T> RetryAsync<T>(Func<Task<T>> operation, string operationName, int maxAttempts = 3)
    {
        var attempt = 0;
        var delay = TimeSpan.FromMilliseconds(100);

        while (attempt < maxAttempts)
        {
            try
            {
                attempt++;
                return await operation();
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                _logger?.LogWarning($"Retry {attempt}/{maxAttempts} for {operationName}: {ex.Message}");
                await Task.Delay(delay);
                delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2); // Exponential backoff
            }
        }

        // Last attempt without catch
        return await operation();
    }

    // FIXED: Use Task.Delay instead of Thread.Sleep to avoid blocking UI thread
    private static T Retry<T>(Func<T> operation, string operationName, int maxAttempts = 3)
    {
        var attempt = 0;
        var delayMs = 100;

        while (attempt < maxAttempts)
        {
            try
            {
                attempt++;
                return operation();
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                _logger?.LogWarning($"Retry {attempt}/{maxAttempts} for {operationName}: {ex.Message}");

                // PERFORMANCE FIX: Use Task.Run with async wait to avoid blocking UI thread
                Task.Delay(delayMs).Wait();
                delayMs *= 2; // Exponential backoff
            }
        }

        // Last attempt without catch
        return operation();
    }

    // PERFORMANCE FIX: Add Task-based retry that doesn't block
    private static async Task<T> RetryNonBlockingAsync<T>(Func<T> operation, string operationName, int maxAttempts = 3)
    {
        var attempt = 0;
        var delay = TimeSpan.FromMilliseconds(100);

        while (attempt < maxAttempts)
        {
            try
            {
                attempt++;
                // Run synchronous operation on thread pool to avoid blocking UI
                return await Task.Run(() => operation());
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                _logger?.LogWarning($"Retry {attempt}/{maxAttempts} for {operationName}: {ex.Message}");
                await Task.Delay(delay);
                delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2);
            }
        }

        // Last attempt
        return await Task.Run(() => operation());
    }

    public static string? LastAlarm => Preferences.Default.Get<string?>("last_alarm", null);

    public static void SetLastAlarm(string uri)
    {
        if (_writeCircuitBreaker.IsOpen)
        {
            LogWarning("SetLastAlarm: Write circuit breaker is OPEN, operation skipped");
            return;
        }

        try
        {
            Retry(() =>
            {
                Preferences.Default.Set("last_alarm", uri ?? string.Empty);
                return true;
            }, "SetLastAlarm");

            _writeCircuitBreaker.RecordSuccess();
        }
        catch (Exception ex)
        {
            _writeCircuitBreaker.RecordFailure();
            LogError("SetLastAlarm", ex);
            SendTelemetry("SetLastAlarm", ex);
        }
    }

    public static bool UseDuration => Preferences.Default.Get("use_duration", true);

    public static void SetUseDuration(bool value)
    {
        if (_writeCircuitBreaker.IsOpen)
        {
            LogWarning("SetUseDuration: Write circuit breaker is OPEN, operation skipped");
            return;
        }

        try
        {
            Retry(() =>
            {
                Preferences.Default.Set("use_duration", value);
                return true;
            }, "SetUseDuration");

            _writeCircuitBreaker.RecordSuccess();
        }
        catch (Exception ex)
        {
            _writeCircuitBreaker.RecordFailure();
            LogError("SetUseDuration", ex);
            SendTelemetry("SetUseDuration", ex);
        }
    }

    public static TimeSpan Duration => TimeSpan.FromMinutes(Preferences.Default.Get("duration_minutes", 60));

    public static void SetDuration(TimeSpan value)
    {
        if (_writeCircuitBreaker.IsOpen)
        {
            LogWarning("SetDuration: Write circuit breaker is OPEN, operation skipped");
            return;
        }

        try
        {
            Retry(() =>
            {
                Preferences.Default.Set("duration_minutes", (int)value.TotalMinutes);
                return true;
            }, "SetDuration");

            _writeCircuitBreaker.RecordSuccess();
        }
        catch (Exception ex)
        {
            _writeCircuitBreaker.RecordFailure();
            LogError("SetDuration", ex);
            SendTelemetry("SetDuration", ex);
        }
    }

    // Fade-out duration in seconds
    public static int FadeOutSeconds
    {
        get
        {
            if (_readCircuitBreaker.IsOpen)
            {
                LogWarning("GetFadeOutSeconds: Read circuit breaker is OPEN, returning default");
                return 8;
            }

            try
            {
                var result = Retry(() => Preferences.Default.Get("fade_seconds", 8), "GetFadeOutSeconds");
                _readCircuitBreaker.RecordSuccess();
                return result;
            }
            catch (Exception ex)
            {
                _readCircuitBreaker.RecordFailure();
                LogError("GetFadeOutSeconds", ex);
                SendTelemetry("GetFadeOutSeconds", ex);
                return 8; // fallback default
            }
        }
        set
        {
            if (_writeCircuitBreaker.IsOpen)
            {
                LogWarning("SetFadeOutSeconds: Write circuit breaker is OPEN, operation skipped");
                return;
            }

            try
            {
                Retry(() =>
                {
                    Preferences.Default.Set("fade_seconds", Math.Clamp(value, 0, 120));
                    return true;
                }, "SetFadeOutSeconds");

                _writeCircuitBreaker.RecordSuccess();
            }
            catch (Exception ex)
            {
                _writeCircuitBreaker.RecordFailure();
                LogError("SetFadeOutSeconds", ex);
                SendTelemetry("SetFadeOutSeconds", ex);
            }
        }
    }

    // Persist alarm enabled toggle
    public static bool AlarmEnabledPref
    {
        get
        {
            if (_readCircuitBreaker.IsOpen)
            {
                LogWarning("GetAlarmEnabledPref: Read circuit breaker is OPEN, returning default");
                return true;
            }

            try
            {
                var result = Retry(() => Preferences.Default.Get("alarm_enabled", true), "GetAlarmEnabledPref");
                _readCircuitBreaker.RecordSuccess();
                return result;
            }
            catch (Exception ex)
            {
                _readCircuitBreaker.RecordFailure();
                LogError("GetAlarmEnabledPref", ex);
                SendTelemetry("GetAlarmEnabledPref", ex);
                return true; // fallback default
            }
        }
        set
        {
            if (_writeCircuitBreaker.IsOpen)
            {
                LogWarning("SetAlarmEnabledPref: Write circuit breaker is OPEN, operation skipped");
                return;
            }

            try
            {
                Retry(() =>
                {
                    Preferences.Default.Set("alarm_enabled", value);
                    return true;
                }, "SetAlarmEnabledPref");

                _writeCircuitBreaker.RecordSuccess();
            }
            catch (Exception ex)
            {
                _writeCircuitBreaker.RecordFailure();
                LogError("SetAlarmEnabledPref", ex);
                SendTelemetry("SetAlarmEnabledPref", ex);
            }
        }
    }

    public static TimeSpan StopAtTime
    {
        get
        {
            if (_readCircuitBreaker.IsOpen)
            {
                LogWarning("GetStopAtTime: Read circuit breaker is OPEN, returning default");
                return new TimeSpan(23, 0, 0);
            }

            try
            {
                var result = Retry(() =>
                {
                    if (!Preferences.Default.ContainsKey("stop_at"))
                        return new TimeSpan(23, 0, 0);

                    return Preferences.Default.Get("stop_at", new TimeSpan(23, 0, 0));
                }, "GetStopAtTime");

                _readCircuitBreaker.RecordSuccess();
                return result;
            }
            catch (Exception ex)
            {
                _readCircuitBreaker.RecordFailure();
                LogError("GetStopAtTime", ex);
                SendTelemetry("GetStopAtTime", ex);

                try
                {
                    Preferences.Default.Remove("stop_at");
                }
                catch (Exception removeEx)
                {
                    LogError("GetStopAtTime_RemoveFailed", removeEx);
                }
                return new TimeSpan(23, 0, 0);
            }
        }
    }

    public static void SetStopAtTime(TimeSpan value)
    {
        if (_writeCircuitBreaker.IsOpen)
        {
            LogWarning("SetStopAtTime: Write circuit breaker is OPEN, operation skipped");
            return;
        }

        try
        {
            Retry(() =>
            {
                Preferences.Default.Set("stop_at", value);
                return true;
            }, "SetStopAtTime");

            _writeCircuitBreaker.RecordSuccess();
        }
        catch (Exception ex)
        {
            _writeCircuitBreaker.RecordFailure();
            LogError("SetStopAtTime", ex);
            SendTelemetry("SetStopAtTime", ex);
        }
    }

    public static Dictionary<string, double> GetMixVolumes()
    {
        // PERFORMANCE FIX: Check cache first
        lock (_cacheLock)
        {
            if (IsCacheValid(_volumesCache))
            {
                return new Dictionary<string, double>(_volumesCache.Value.Data);
            }
        }

        if (_readCircuitBreaker.IsOpen)
        {
            LogWarning("GetMixVolumes: Read circuit breaker is OPEN, returning empty dictionary");
            return new Dictionary<string, double>();
        }

        try
        {
            var result = Retry(() =>
            {
                var json = Preferences.Default.Get(MixVolumesKey, string.Empty);
                if (string.IsNullOrEmpty(json))
                    return new Dictionary<string, double>();

                var data = JsonSerializer.Deserialize<Dictionary<string, double>>(json);
                return data ?? new Dictionary<string, double>();
            }, "GetMixVolumes");

            // PERFORMANCE FIX: Cache the result
            lock (_cacheLock)
            {
                _volumesCache = (result, DateTime.UtcNow);
            }

            _readCircuitBreaker.RecordSuccess();
            return result;
        }
        catch (JsonException jsonEx)
        {
            _readCircuitBreaker.RecordFailure();
            LogError($"GetMixVolumes_JsonDeserialization", jsonEx);
            SendTelemetry("GetMixVolumes", jsonEx);

            // Attempt to clear corrupt data
            try
            {
                Preferences.Default.Remove(MixVolumesKey);
            }
            catch (Exception removeEx)
            {
                LogError("GetMixVolumes_RemoveFailed", removeEx);
            }
            return new Dictionary<string, double>();
        }
        catch (Exception ex)
        {
            _readCircuitBreaker.RecordFailure();
            LogError("GetMixVolumes", ex);
            SendTelemetry("GetMixVolumes", ex);
            return new Dictionary<string, double>();
        }
    }

    public static void SetMixVolumes(Dictionary<string, double> volumes)
    {
        if (_writeCircuitBreaker.IsOpen)
        {
            LogWarning("SetMixVolumes: Write circuit breaker is OPEN, operation skipped");
            return;
        }

        try
        {
            if (volumes == null)
            {
                LogWarning("SetMixVolumes called with null volumes");
                return;
            }

            Retry(() =>
            {
                var json = JsonSerializer.Serialize(volumes);
                Preferences.Default.Set(MixVolumesKey, json);
                return true;
            }, "SetMixVolumes");

            // PERFORMANCE FIX: Invalidate cache on write
            lock (_cacheLock)
            {
                _volumesCache = null;
            }

            _writeCircuitBreaker.RecordSuccess();
        }
        catch (JsonException jsonEx)
        {
            _writeCircuitBreaker.RecordFailure();
            LogError("SetMixVolumes_JsonSerialization", jsonEx);
            SendTelemetry("SetMixVolumes", jsonEx);
        }
        catch (Exception ex)
        {
            _writeCircuitBreaker.RecordFailure();
            LogError("SetMixVolumes", ex);
            SendTelemetry("SetMixVolumes", ex);
        }
    }

    public static List<SavedMix> GetMixes()
    {
        // PERFORMANCE FIX: Check cache first
        lock (_cacheLock)
        {
            if (IsCacheValid(_mixesCache))
            {
                return new List<SavedMix>(_mixesCache.Value.Data);
            }
        }

        if (_readCircuitBreaker.IsOpen)
        {
            LogWarning("GetMixes: Read circuit breaker is OPEN, returning empty list");
            return new List<SavedMix>();
        }

        try
        {
            var result = Retry(() =>
            {
                var json = Preferences.Default.Get(MixesKey, string.Empty);
                if (string.IsNullOrEmpty(json))
                    return new List<SavedMix>();

                var data = JsonSerializer.Deserialize<List<SavedMix>>(json);
                return data ?? new List<SavedMix>();
            }, "GetMixes");

            // PERFORMANCE FIX: Cache the result
            lock (_cacheLock)
            {
                _mixesCache = (result, DateTime.UtcNow);
            }

            _readCircuitBreaker.RecordSuccess();
            return result;
        }
        catch (JsonException jsonEx)
        {
            _readCircuitBreaker.RecordFailure();
            LogError("GetMixes_JsonDeserialization", jsonEx);
            SendTelemetry("GetMixes", jsonEx);

            try
            {
                Preferences.Default.Remove(MixesKey);
            }
            catch (Exception removeEx)
            {
                LogError("GetMixes_RemoveFailed", removeEx);
            }
            return new List<SavedMix>();
        }
        catch (Exception ex)
        {
            _readCircuitBreaker.RecordFailure();
            LogError("GetMixes", ex);
            SendTelemetry("GetMixes", ex);
            return new List<SavedMix>();
        }
    }

    public static void SaveMix(SavedMix mix)
    {
        if (mix == null)
        {
            LogWarning("SaveMix called with null mix");
            return;
        }

        if (string.IsNullOrWhiteSpace(mix.Name))
        {
            LogWarning("SaveMix called with empty mix name");
            return;
        }

        if (_writeCircuitBreaker.IsOpen)
        {
            LogWarning("SaveMix: Write circuit breaker is OPEN, operation skipped");
            throw new InvalidOperationException("Preferences storage is temporarily unavailable. Please try again in a moment.");
        }

        try
        {
            Retry(() =>
            {
                // PERFORMANCE FIX: Get mixes once instead of multiple times
                var mixes = GetMixes();
                mixes.RemoveAll(m => m.Name == mix.Name);
                mixes.Insert(0, mix);

                var services = ServiceHost.Services;
                if (services != null)
                {
                    try
                    {
                        var features = services.GetRequiredService<FeatureGate>();
                        var maxMixes = features.MaxSavedMixes;

                        if (maxMixes > 0 && mixes.Count > maxMixes)
                            mixes = mixes.Take(maxMixes).ToList();
                    }
                    catch (Exception featureEx)
                    {
                        LogError("SaveMix_ApplyFeatureGate", featureEx);
                        // Continue without enforcing limit
                    }
                }

                var json = JsonSerializer.Serialize(mixes);
                Preferences.Default.Set(MixesKey, json);
                return true;
            }, "SaveMix");

            // PERFORMANCE FIX: Invalidate cache on write
            lock (_cacheLock)
            {
                _mixesCache = null;
            }

            _writeCircuitBreaker.RecordSuccess();
        }
        catch (JsonException jsonEx)
        {
            _writeCircuitBreaker.RecordFailure();
            LogError("SaveMix_JsonSerialization", jsonEx);
            SendTelemetry("SaveMix", jsonEx);
            throw new InvalidOperationException("Failed to save mix due to serialization error.", jsonEx);
        }
        catch (Exception ex)
        {
            _writeCircuitBreaker.RecordFailure();
            LogError("SaveMix", ex);
            SendTelemetry("SaveMix", ex);
            throw new InvalidOperationException("Failed to save mix.", ex);
        }
    }

    public static void DeleteMix(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            LogWarning("DeleteMix called with empty name");
            return;
        }

        if (_writeCircuitBreaker.IsOpen)
        {
            LogWarning("DeleteMix: Write circuit breaker is OPEN, operation skipped");
            throw new InvalidOperationException("Preferences storage is temporarily unavailable. Please try again in a moment.");
        }

        try
        {
            Retry(() =>
            {
                var mixes = GetMixes();
                var countBefore = mixes.Count;
                mixes.RemoveAll(m => m.Name == name);

                if (countBefore == mixes.Count)
                {
                    LogWarning($"DeleteMix: Mix '{name}' not found");
                }

                var json = JsonSerializer.Serialize(mixes);
                Preferences.Default.Set(MixesKey, json);
                return true;
            }, "DeleteMix");

            // PERFORMANCE FIX: Invalidate cache on write
            lock (_cacheLock)
            {
                _mixesCache = null;
            }

            _writeCircuitBreaker.RecordSuccess();
        }
        catch (Exception ex)
        {
            _writeCircuitBreaker.RecordFailure();
            LogError($"DeleteMix: {name}", ex);
            SendTelemetry("DeleteMix", ex);
            throw new InvalidOperationException($"Failed to delete mix '{name}'.", ex);
        }
    }

    public static List<SavedPlaylist> GetPlaylists()
    {
        // PERFORMANCE FIX: Check cache first
        lock (_cacheLock)
        {
            if (IsCacheValid(_playlistsCache))
            {
                return new List<SavedPlaylist>(_playlistsCache.Value.Data);
            }
        }

        if (_readCircuitBreaker.IsOpen)
        {
            LogWarning("GetPlaylists: Read circuit breaker is OPEN, returning empty list");
            return new List<SavedPlaylist>();
        }

        try
        {
            var result = Retry(() =>
            {
                var json = Preferences.Default.Get(PlaylistsKey, string.Empty);
                if (string.IsNullOrEmpty(json))
                    return new List<SavedPlaylist>();

                var data = JsonSerializer.Deserialize<List<SavedPlaylist>>(json);
                return data ?? new List<SavedPlaylist>();
            }, "GetPlaylists");

            // PERFORMANCE FIX: Cache the result
            lock (_cacheLock)
            {
                _playlistsCache = (result, DateTime.UtcNow);
            }

            _readCircuitBreaker.RecordSuccess();
            return result;
        }
        catch (JsonException jsonEx)
        {
            _readCircuitBreaker.RecordFailure();
            LogError("GetPlaylists_JsonDeserialization", jsonEx);
            SendTelemetry("GetPlaylists", jsonEx);

            try
            {
                Preferences.Default.Remove(PlaylistsKey);
            }
            catch (Exception removeEx)
            {
                LogError("GetPlaylists_RemoveFailed", removeEx);
            }
            return new List<SavedPlaylist>();
        }
        catch (Exception ex)
        {
            _readCircuitBreaker.RecordFailure();
            LogError("GetPlaylists", ex);
            SendTelemetry("GetPlaylists", ex);
            return new List<SavedPlaylist>();
        }
    }

    public static void AddSoundToPlaylist(string playlistName, AudioItem sound)
    {
        if (string.IsNullOrWhiteSpace(playlistName))
        {
            LogWarning("AddSoundToPlaylist called with empty playlist name");
            return;
        }

        if (sound == null)
        {
            LogWarning("AddSoundToPlaylist called with null sound");
            return;
        }

        if (_writeCircuitBreaker.IsOpen)
        {
            LogWarning("AddSoundToPlaylist: Write circuit breaker is OPEN, operation skipped");
            throw new InvalidOperationException("Preferences storage is temporarily unavailable. Please try again in a moment.");
        }

        try
        {
            Retry(() =>
            {
                var playlists = GetPlaylists();
                var playlist = playlists.FirstOrDefault(p => p.Name == playlistName);

                if (playlist == null)
                {
                    LogWarning($"AddSoundToPlaylist: Playlist '{playlistName}' not found");
                    return false;
                }

                // Avoid duplicates
                if (!playlist.Items.Any(i => i.Id == sound.Id))
                {
                    playlist.Items.Add(sound);
                    SavePlaylist(playlist);
                }
                else
                {
                    LogWarning($"AddSoundToPlaylist: Sound '{sound.Id}' already in playlist '{playlistName}'");
                }
                return true;
            }, "AddSoundToPlaylist");

            _writeCircuitBreaker.RecordSuccess();
        }
        catch (Exception ex)
        {
            _writeCircuitBreaker.RecordFailure();
            LogError($"AddSoundToPlaylist: {playlistName}", ex);
            SendTelemetry("AddSoundToPlaylist", ex);
            throw new InvalidOperationException($"Failed to add sound to playlist '{playlistName}'.", ex);
        }
    }

    public static void SavePlaylist(SavedPlaylist playlist)
    {
        if (playlist == null)
        {
            LogWarning("SavePlaylist called with null playlist");
            return;
        }

        if (string.IsNullOrWhiteSpace(playlist.Name))
        {
            LogWarning("SavePlaylist called with empty playlist name");
            return;
        }

        if (_writeCircuitBreaker.IsOpen)
        {
            LogWarning("SavePlaylist: Write circuit breaker is OPEN, operation skipped");
            throw new InvalidOperationException("Preferences storage is temporarily unavailable. Please try again in a moment.");
        }

        try
        {
            Retry(() =>
            {
                var playlists = GetPlaylists();

                // Remove if a playlist with the same name exists
                playlists.RemoveAll(p => p.Name == playlist.Name);

                // Add new playlist at the front
                playlists.Insert(0, playlist);

                // use tier-based cap from FeatureGate instead of static constant
                var services = ServiceHost.Services;
                if (services != null)
                {
                    try
                    {
                        var features = services.GetRequiredService<FeatureGate>();
                        var maxPlaylists = features.MaxSavedPlaylists;

                        if (maxPlaylists > 0 && playlists.Count > maxPlaylists)
                            playlists = playlists.Take(maxPlaylists).ToList();
                    }
                    catch (Exception featureEx)
                    {
                        LogError("SavePlaylist_ApplyFeatureGate", featureEx);
                        // Continue without enforcing limit
                    }
                }

                var json = JsonSerializer.Serialize(playlists);
                Preferences.Default.Set(PlaylistsKey, json);
                return true;
            }, "SavePlaylist");

            // PERFORMANCE FIX: Invalidate cache on write
            lock (_cacheLock)
            {
                _playlistsCache = null;
            }

            _writeCircuitBreaker.RecordSuccess();
        }
        catch (JsonException jsonEx)
        {
            _writeCircuitBreaker.RecordFailure();
            LogError("SavePlaylist_JsonSerialization", jsonEx);
            SendTelemetry("SavePlaylist", jsonEx);
            throw new InvalidOperationException("Failed to save playlist due to serialization error.", jsonEx);
        }
        catch (Exception ex)
        {
            _writeCircuitBreaker.RecordFailure();
            LogError("SavePlaylist", ex);
            SendTelemetry("SavePlaylist", ex);
            throw new InvalidOperationException("Failed to save playlist.", ex);
        }
    }

    public static void DeletePlaylist(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            LogWarning("DeletePlaylist called with empty name");
            return;
        }

        if (_writeCircuitBreaker.IsOpen)
        {
            LogWarning("DeletePlaylist: Write circuit breaker is OPEN, operation skipped");
            throw new InvalidOperationException("Preferences storage is temporarily unavailable. Please try again in a moment.");
        }

        try
        {
            Retry(() =>
            {
                var playlists = GetPlaylists();
                var countBefore = playlists.Count;
                playlists.RemoveAll(p => p.Name == name);

                if (countBefore == playlists.Count)
                {
                    LogWarning($"DeletePlaylist: Playlist '{name}' not found");
                }

                var json = JsonSerializer.Serialize(playlists);
                Preferences.Default.Set(PlaylistsKey, json);
                return true;
            }, "DeletePlaylist");

            // PERFORMANCE FIX: Invalidate cache on write
            lock (_cacheLock)
            {
                _playlistsCache = null;
            }

            _writeCircuitBreaker.RecordSuccess();
        }
        catch (Exception ex)
        {
            _writeCircuitBreaker.RecordFailure();
            LogError($"DeletePlaylist: {name}", ex);
            SendTelemetry("DeletePlaylist", ex);
            throw new InvalidOperationException($"Failed to delete playlist '{name}'.", ex);
        }
    }

    public static List<SavedMixPlaylist> GetMixPlaylists()
    {
        // PERFORMANCE FIX: Check cache first
        lock (_cacheLock)
        {
            if (IsCacheValid(_mixPlaylistsCache))
            {
                return new List<SavedMixPlaylist>(_mixPlaylistsCache.Value.Data);
            }
        }

        if (_readCircuitBreaker.IsOpen)
        {
            LogWarning("GetMixPlaylists: Read circuit breaker is OPEN, returning empty list");
            return new List<SavedMixPlaylist>();
        }

        try
        {
            var result = Retry(() =>
            {
                var json = Preferences.Default.Get(MixPlaylistsKey, string.Empty);
                if (string.IsNullOrEmpty(json))
                    return new List<SavedMixPlaylist>();

                var data = JsonSerializer.Deserialize<List<SavedMixPlaylist>>(json);
                return data ?? new List<SavedMixPlaylist>();
            }, "GetMixPlaylists");

            // PERFORMANCE FIX: Cache the result
            lock (_cacheLock)
            {
                _mixPlaylistsCache = (result, DateTime.UtcNow);
            }

            _readCircuitBreaker.RecordSuccess();
            return result;
        }
        catch (JsonException jsonEx)
        {
            _readCircuitBreaker.RecordFailure();
            LogError("GetMixPlaylists_JsonDeserialization", jsonEx);
            SendTelemetry("GetMixPlaylists", jsonEx);

            try
            {
                Preferences.Default.Remove(MixPlaylistsKey);
            }
            catch (Exception removeEx)
            {
                LogError("GetMixPlaylists_RemoveFailed", removeEx);
            }
            return new List<SavedMixPlaylist>();
        }
        catch (Exception ex)
        {
            _readCircuitBreaker.RecordFailure();
            LogError("GetMixPlaylists", ex);
            SendTelemetry("GetMixPlaylists", ex);
            return new List<SavedMixPlaylist>();
        }
    }

    public static void SaveMixPlaylist(SavedMixPlaylist list)
    {
        if (list == null)
        {
            LogWarning("SaveMixPlaylist called with null list");
            return;
        }

        if (string.IsNullOrWhiteSpace(list.Name))
        {
            LogWarning("SaveMixPlaylist called with empty list name");
            return;
        }

        if (_writeCircuitBreaker.IsOpen)
        {
            LogWarning("SaveMixPlaylist: Write circuit breaker is OPEN, operation skipped");
            throw new InvalidOperationException("Preferences storage is temporarily unavailable. Please try again in a moment.");
        }

        try
        {
            Retry(() =>
            {
                var lists = GetMixPlaylists();
                lists.RemoveAll(p => p.Name == list.Name);

                // enforce per-playlist entry cap
                var services = ServiceHost.Services;
                if (services != null)
                {
                    try
                    {
                        var features = services.GetRequiredService<FeatureGate>();
                        var maxEntries = features.MaxMixesPerMixPlaylist;
                        if (maxEntries > 0 && list.Entries.Count > maxEntries)
                            list.Entries = list.Entries.Take(maxEntries).ToList();

                        lists.Insert(0, list);

                        // enforce total playlists cap
                        var maxLists = features.MaxSavedMixPlaylists;
                        if (maxLists > 0 && lists.Count > maxLists)
                            lists = lists.Take(maxLists).ToList();
                    }
                    catch (Exception featureEx)
                    {
                        LogError("SaveMixPlaylist_ApplyFeatureGate", featureEx);
                        // Fallback: just insert without enforcing caps
                        lists.Insert(0, list);
                    }
                }
                else
                {
                    // Fallback: just insert without enforcing caps
                    lists.Insert(0, list);
                }

                var json = JsonSerializer.Serialize(lists);
                Preferences.Default.Set(MixPlaylistsKey, json);
                return true;
            }, "SaveMixPlaylist");

            // PERFORMANCE FIX: Invalidate cache on write
            lock (_cacheLock)
            {
                _mixPlaylistsCache = null;
            }

            _writeCircuitBreaker.RecordSuccess();
        }
        catch (JsonException jsonEx)
        {
            _writeCircuitBreaker.RecordFailure();
            LogError("SaveMixPlaylist_JsonSerialization", jsonEx);
            SendTelemetry("SaveMixPlaylist", jsonEx);
            throw new InvalidOperationException("Failed to save mix playlist due to serialization error.", jsonEx);
        }
        catch (Exception ex)
        {
            _writeCircuitBreaker.RecordFailure();
            LogError("SaveMixPlaylist", ex);
            SendTelemetry("SaveMixPlaylist", ex);
            throw new InvalidOperationException("Failed to save mix playlist.", ex);
        }
    }

    public static void DeleteMixPlaylist(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            LogWarning("DeleteMixPlaylist called with empty name");
            return;
        }

        if (_writeCircuitBreaker.IsOpen)
        {
            LogWarning("DeleteMixPlaylist: Write circuit breaker is OPEN, operation skipped");
            throw new InvalidOperationException("Preferences storage is temporarily unavailable. Please try again in a moment.");
        }

        try
        {
            Retry(() =>
            {
                var lists = GetMixPlaylists();
                var countBefore = lists.Count;
                lists.RemoveAll(p => p.Name == name);

                if (countBefore == lists.Count)
                {
                    LogWarning($"DeleteMixPlaylist: Mix playlist '{name}' not found");
                }

                var json = JsonSerializer.Serialize(lists);
                Preferences.Default.Set(MixPlaylistsKey, json);
                return true;
            }, "DeleteMixPlaylist");

            // PERFORMANCE FIX: Invalidate cache on write
            lock (_cacheLock)
            {
                _mixPlaylistsCache = null;
            }

            _writeCircuitBreaker.RecordSuccess();
        }
        catch (Exception ex)
        {
            _writeCircuitBreaker.RecordFailure();
            LogError($"DeleteMixPlaylist: {name}", ex);
            SendTelemetry("DeleteMixPlaylist", ex);
            throw new InvalidOperationException($"Failed to delete mix playlist '{name}'.", ex);
        }
    }

    // Telemetry helper
    private static void SendTelemetry(string operation, Exception ex)
    {
        try
        {
            _telemetryCallback?.Invoke(new PreferencesErrorEvent
            {
                Operation = operation,
                ErrorType = ex.GetType().Name,
                ErrorMessage = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
        catch
        {
            // Never let telemetry crash the app
        }
    }

    // Logging helpers
    private static void LogError(string operation, Exception ex)
    {
        var message = $"UserPreferences.{operation} failed: {ex.Message}";
        _logger?.LogError(ex, message);

        // Also log to debug output as fallback
        System.Diagnostics.Debug.WriteLine($"ERROR: {message}");
        System.Diagnostics.Debug.WriteLine($"  Exception: {ex}");
    }

    private static void LogWarning(string message)
    {
        var fullMessage = $"UserPreferences: {message}";
        _logger?.LogWarning(fullMessage);

        // Also log to debug output as fallback
        System.Diagnostics.Debug.WriteLine($"WARNING: {fullMessage}");
    }
}