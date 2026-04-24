using AVFoundation;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmbientSleeper.Services;

namespace AmbientSleeper.Platforms.iOS.Services
{
    public class AlarmSoundService : IAlarmSoundService
    {
        private AVAudioPlayer? _player;

        public Task<string> PickSystemSoundAsync()
        {
            // Use bundled alarm.mp3 or return empty
            return Task.FromResult("alarm.mp3");
        }

        public void PlayAlarm(string uri, float volume = 1.0f, bool loop = false)
        {
            var url = NSUrl.FromFilename(uri);
            var player = AVAudioPlayer.FromUrl(url);
            if (player == null) return;

            player.Volume = volume;
            player.Play();
        }

        public void StopAlarm()
        {
            if (_player == null) return;
            try
            {
                if (_player.Playing)
                    _player.Stop();
            }
            catch
            {
                //eventually add some logging here
            }
            finally
            {
                _player.Dispose();
                _player = null;
            }
            
        }
    }

}
