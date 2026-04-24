using Plugin.Maui.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmbientSleeper.Models;

namespace AmbientSleeper.Services
{
    public class MixAudioService : IMixAudioService
    {
        private readonly IAudioManager _audioManager;
        private readonly Dictionary<string, IAudioPlayer> _players = new();

        public bool IsPlaying => _players.Values.Any(p => p.IsPlaying);

        public MixAudioService(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        public async Task PlayMixAsync(IEnumerable<MixSoundItem> items)
        {
            StopAll();
            _players.Clear();

            foreach (var item in items.Where(i => i.IsEnabled))
            {
                var stream = await FileSystem.OpenAppPackageFileAsync(item.FileName);
                var player = _audioManager.CreatePlayer(stream);
                player.Volume = item.Volume;
                player.Loop = true;
                player.Play();
                _players[item.FileName] = player;
            }
        }

        public async Task FadeOutAsync(TimeSpan duration)
        {
            const int steps = 20;
            var interval = duration / steps;

            for (int i = 0; i < steps; i++)
            {
                foreach (var player in _players.Values)
                {
                    player.Volume = Math.Max(0, player.Volume - (1.0 / steps));
                }
                await Task.Delay(interval);
            }

            StopAll();
        }


        public void StopAll()
        {
            foreach (var player in _players.Values)
            {
                player.Stop();
                player.Dispose();
            }
            _players.Clear();
        }
    }

}
