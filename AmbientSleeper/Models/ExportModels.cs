using AmbientSleeper.Services;

namespace AmbientSleeper.Models;

public enum ExportScope
{
    Personal,   // for your own devices
    Shareable   // for other Premium/Pro+ users
}

public class MixExportPackage
{
    public string Version { get; set; } = "1.0";
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public ExportScope Scope { get; set; }
    public string TierRequirement { get; set; } = "Standard+";

    public List<SavedMix> Mixes { get; set; } = new();
    public List<SavedPlaylist> Playlists { get; set; } = new();
}