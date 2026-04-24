using AmbientSleeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmbientSleeper.Services
{
    public interface IMixAudioService
    {
        Task PlayMixAsync(IEnumerable<MixSoundItem> items);
        Task FadeOutAsync(TimeSpan duration);
        void StopAll();
        bool IsPlaying { get; }
    }

}
