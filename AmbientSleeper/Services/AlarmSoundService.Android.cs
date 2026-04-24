#if ANDROID
using Android.Media;

namespace AmbientSleeper.Services;

public class AlarmSoundService : IAlarmSoundService
{
    private MediaPlayer? _player;

    public async Task<string> PickSystemSoundAsync()
    {
        // Simple: choose default alarm (fallback to notification)
        var uri = RingtoneManager.GetDefaultUri(RingtoneType.Alarm)
                  ?? RingtoneManager.GetDefaultUri(RingtoneType.Notification);

        return await Task.FromResult(uri?.ToString() ?? string.Empty);
    }

    public void PlayAlarm(string uriOrName, float volume = 1.0f, bool loop = false)
    {
        StopAlarm();

        // Support sentinel "default" and empty/null -> use device default alarm/notification
        Android.Net.Uri? soundUri = null;
        if (string.IsNullOrWhiteSpace(uriOrName) ||
            string.Equals(uriOrName, "default", StringComparison.OrdinalIgnoreCase))
        {
            soundUri = RingtoneManager.GetDefaultUri(RingtoneType.Alarm)
                       ?? RingtoneManager.GetDefaultUri(RingtoneType.Notification);
        }
        else
        {
            soundUri = Android.Net.Uri.Parse(uriOrName);
        }

        if (soundUri is null)
            return;

        _player = new MediaPlayer();
        _player.SetAudioAttributes(new AudioAttributes.Builder()?
            .SetUsage(AudioUsageKind.Alarm)?
            .SetContentType(AudioContentType.Music)?
            .Build());
        _player.Looping = loop;
        _player.SetDataSource(Android.App.Application.Context, soundUri);

        _player.Prepared += (s, e) =>
        {
            var v = Math.Clamp(volume, 0f, 1f);
            _player?.SetVolume(v, v);
            _player?.Start();
        };

        // Prepare asynchronously to avoid blocking the UI thread
        _player.PrepareAsync();
    }

    public void StopAlarm()
    {
        if (_player == null) return;

        try
        {
            if (_player.IsPlaying) _player.Stop();

            _player.Reset();
            _player.Release();
            _player.Dispose();

        }
        catch
        {
            /* ignore */
        }
        finally
        {
            _player = null;
        }
    }
}
#endif