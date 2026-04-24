using AmbientSleeper.Services;
using A_Media = global::Android.Media;
using DroidApp = global::Android.App;
using DroidNet = global::Android.Net;

namespace AmbientSleeper.Platforms.Android.Services
{
    public class AlarmSoundService : IAlarmSoundService
    {
        private A_Media.MediaPlayer? _player;

        public bool IsPlaying => _player?.IsPlaying == true;

        public async Task<string> PickSystemSoundAsync()
        {
            var uri = A_Media.RingtoneManager.GetDefaultUri(A_Media.RingtoneType.Alarm)
                      ?? A_Media.RingtoneManager.GetDefaultUri(A_Media.RingtoneType.Notification);
            return await Task.FromResult(uri?.ToString() ?? string.Empty);
        }

        public void PlayAlarm(string uri, float volume = 1.0f, bool loop = false)
        {
            StopAlarm();

            if (string.IsNullOrWhiteSpace(uri)) return;

            // Ensure we have a valid Context and parsed Android Uri
            var ctx = DroidApp.Application.Context;
            if (ctx == null) return;

            var androidUri = DroidNet.Uri.Parse(uri);
            if (androidUri == null) return;

            var player = new A_Media.MediaPlayer();

            // Build audio attributes with null checks
            var attrBuilder = new A_Media.AudioAttributes.Builder();
            var builderWithUsage = attrBuilder.SetUsage(A_Media.AudioUsageKind.Alarm);
            if (builderWithUsage == null) return;
            
            var builderWithContent = builderWithUsage.SetContentType(A_Media.AudioContentType.Music);
            if (builderWithContent == null) return;
            
            var attrs = builderWithContent.Build();            
            if (attrs != null)
            {
                player.SetAudioAttributes(attrs);
            }

            player.SetDataSource(ctx, androidUri);
            player.Looping = loop;
            player.Prepared += (s, e) =>
            {
                var v = Math.Clamp(volume, 0f, 1f);
                player.SetVolume(v, v);
                player.Start();
            };
            player.Completion += (s, e) =>
            {
                if (!loop) StopAlarm();
            };
            _player = player;
            player.PrepareAsync();
        }

        public void StopAlarm()
        {
            try
            {
                if (_player != null)
                {
                    if (_player.IsPlaying) _player.Stop();
                    _player.Reset();
                    _player.Release();
                    _player.Dispose();
                }
            }
            finally
            {
                _player = null;
            }
        }

        private void SetVolumeInternal(float volume)
        {
            var v = Math.Clamp(volume, 0f, 1f);
            _player?.SetVolume(v, v);
        }
    }
}
