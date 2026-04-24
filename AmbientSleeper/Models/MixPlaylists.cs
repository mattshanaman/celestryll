using System.Text.Json.Serialization;

namespace AmbientSleeper.Models;

public class MixPlaylistEntry
{
    public SavedMix Mix { get; set; } = new();
    // Pro+: optional per-entry duration in seconds; if null, use default
    public int? DurationSeconds { get; set; }
    // Pro+: crossfade/transition length 0-30 seconds
    public int TransitionSeconds { get; set; } = 0;
}

public class SavedMixPlaylist
{
    public string Name { get; set; } = string.Empty;
    public List<MixPlaylistEntry> Entries { get; set; } = new();
    public bool Loop { get; set; } = true;
    public DateTime SavedAt { get; set; } = DateTime.UtcNow;
}
