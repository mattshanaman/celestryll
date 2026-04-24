using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmbientSleeper.Models;

public class AudioItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public AudioSourceType SourceType { get; set; } = AudioSourceType.Bundled;

    // For Bundled: FileName under Resources/Raw (e.g., "rain.mp3")
    public string? BundledFileName { get; set; }

    // For Device: absolute file path or content URI (Android). We'll normalize to a local cache path.
    public string? DevicePath { get; set; }

    // Default volume for Mix Mode (0.0–1.0)
    public double Volume { get; set; } = 1.0;
}
