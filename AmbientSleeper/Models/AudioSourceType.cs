using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmbientSleeper.Models;

public enum AudioSourceType
{
    Bundled,  // from Resources/Raw
    Device    // user-picked file (copied to cache or direct path)
}

