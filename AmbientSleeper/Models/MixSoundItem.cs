using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmbientSleeper.Models
{
    public class MixSoundItem
    {
        public string FileName { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public bool IsEnabled { get; set; }
        public double Volume { get; set; } = 1.0;
    }

}
