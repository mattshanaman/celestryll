using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmbientSleeper.Models;

public class PlaylistEntry
{
    public AudioItem Item { get; set; } = default!;
    public int Order { get; set; } // 0-based
}
