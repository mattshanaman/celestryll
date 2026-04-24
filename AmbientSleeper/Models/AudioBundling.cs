using System;
using System.Collections.Generic;

namespace AmbientSleeper.Models
{
    public class AudioFile
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string? Metadata { get; set; }
        public double DurationSeconds { get; set; }
        // Add more fields as needed (e.g., region, language, etc.)
    }

    public class Bundle
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Artwork { get; set; }
        public List<BundleFile> Files { get; set; } = new();
    }

    public class BundleFile
    {
        public string BundleId { get; set; } = string.Empty;
        public string FileId { get; set; } = string.Empty;
        public int Order { get; set; }
    }

    public class Tier
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int MaxBundles { get; set; }
        public int MaxFiles { get; set; }
    }

    public class TierAccess
    {
        public string TierId { get; set; } = string.Empty;
        public string BundleId { get; set; } = string.Empty;
        public string FileId { get; set; } = string.Empty;
        public string AccessType { get; set; } = "full"; // e.g., "full", "partial", "teaser"
    }
}
