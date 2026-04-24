using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmbientSleeper.Models
{
    public class SavedMix
    {
        public string Name { get; set; } = string.Empty;
        public List<AudioItem> Items { get; set; } = new();
        public DateTime SavedAt { get; set; } = DateTime.UtcNow;
    }
}
