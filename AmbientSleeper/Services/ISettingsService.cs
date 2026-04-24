using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmbientSleeper.Services
{
    public interface ISettingsService
    {
        string? LastSoundFile { get; set; }
        bool LoopEnabled { get; set; }
        bool UseDuration { get; set; }
        TimeSpan Duration { get; set; }
        TimeSpan StopAtTime { get; set; }
    }
}
