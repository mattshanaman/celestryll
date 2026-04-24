using System.Collections.Generic;
using System.Linq;
using AmbientSleeper.Models;

namespace AmbientSleeper.Services;

public class AudioBundleService : IAudioBundleService
{
    // In a real app, these would come from a database or remote config
    private readonly List<Bundle> _bundles;
    private readonly List<AudioFile> _files;
    private readonly List<BundleFile> _bundleFiles;
    private readonly List<Tier> _tiers;
    private readonly List<TierAccess> _tierAccess;

    public AudioBundleService()
    {
        _bundles = new List<Bundle>();
        _files = new List<AudioFile>();
        _bundleFiles = new List<BundleFile>();
        _tiers = new List<Tier>();
        _tierAccess = new List<TierAccess>();

        InitializeBuiltInBundle();
        InitializeTiers();
    }

    private void InitializeBuiltInBundle()
    {
        try
        {
            // Create the Built-in bundle
            var builtInBundle = new Bundle
            {
                Id = "bundle_builtin",
                Name = "Built-in",
                Description = "Core ambient sounds included with the app",
                Artwork = null
            };

            // Create audio files for the Built-in bundle
            var industrialFan = new AudioFile
            {
                Id = "file_industrial_fan",
                Name = "Industrial Fan",
                Metadata = "large-industrial-fan-running-constantly-in-warehouse-environment-339216.mp3",
                DurationSeconds = 0 // Duration can be calculated when loaded
            };

            var boxFan = new AudioFile
            {
                Id = "file_box_fan",
                Name = "Box Fan",
                Metadata = "relaxing-boxfan-359880.mp3",
                DurationSeconds = 0
            };

            var runningWater = new AudioFile
            {
                Id = "file_running_water",
                Name = "Running Water",
                Metadata = "running-stream-water-sound-239612.mp3",
                DurationSeconds = 0
            };

            var thundershower = new AudioFile
            {
                Id = "file_thundershower",
                Name = "Thundershower",
                Metadata = "thunderstorm-rain-2-19350.mp3",
                DurationSeconds = 0
            };

            var calmingRain = new AudioFile
            {
                Id = "file_calming_rain",
                Name = "Calming Rain",
                Metadata = "calming-rain-loop-398653.mp3",
                DurationSeconds = 0
            };

            var whiteNoise = new AudioFile
            {
                Id = "file_white_noise",
                Name = "White Noise",
                Metadata = "white-noise-171891.mp3",
                DurationSeconds = 0
            };

            // Add files to collection
            _files.AddRange(new[] { industrialFan, boxFan, runningWater, thundershower, calmingRain, whiteNoise });

            // Link files to the Built-in bundle
            _bundleFiles.Add(new BundleFile { BundleId = builtInBundle.Id, FileId = industrialFan.Id, Order = 1 });
            _bundleFiles.Add(new BundleFile { BundleId = builtInBundle.Id, FileId = boxFan.Id, Order = 2 });
            _bundleFiles.Add(new BundleFile { BundleId = builtInBundle.Id, FileId = runningWater.Id, Order = 3 });
            _bundleFiles.Add(new BundleFile { BundleId = builtInBundle.Id, FileId = thundershower.Id, Order = 4 });
            _bundleFiles.Add(new BundleFile { BundleId = builtInBundle.Id, FileId = calmingRain.Id, Order = 5 });
            _bundleFiles.Add(new BundleFile { BundleId = builtInBundle.Id, FileId = whiteNoise.Id, Order = 6 });

            // Add bundle to collection
            _bundles.Add(builtInBundle);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing Built-in bundle: {ex.Message}");
            // Continue without Built-in bundle if initialization fails
        }
    }

    private void InitializeTiers()
    {
        try
        {
            // Define tiers
            _tiers.Add(new Tier { Id = "Free", Name = "Free", MaxBundles = 1, MaxFiles = 6 });
            _tiers.Add(new Tier { Id = "Standard", Name = "Standard", MaxBundles = 2, MaxFiles = 20 });
            _tiers.Add(new Tier { Id = "Premium", Name = "Premium", MaxBundles = 5, MaxFiles = 50 });
            _tiers.Add(new Tier { Id = "ProPlus", Name = "Pro+", MaxBundles = 999, MaxFiles = 999 });

            // Grant access to Built-in bundle for all tiers
            var builtInBundleId = "bundle_builtin";
            foreach (var tier in _tiers)
            {
                _tierAccess.Add(new TierAccess
                {
                    TierId = tier.Id,
                    BundleId = builtInBundleId,
                    FileId = string.Empty,
                    AccessType = "full"
                });

                // Also grant file-level access
                foreach (var file in _files.Where(f => f.Id.StartsWith("file_")))
                {
                    _tierAccess.Add(new TierAccess
                    {
                        TierId = tier.Id,
                        BundleId = string.Empty,
                        FileId = file.Id,
                        AccessType = "full"
                    });
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing tiers: {ex.Message}");
            // Continue with empty tiers if initialization fails
        }
    }

    public IEnumerable<Bundle> GetAllBundles()
    {
        try
        {
            return _bundles ?? Enumerable.Empty<Bundle>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting all bundles: {ex.Message}");
            return Enumerable.Empty<Bundle>();
        }
    }

    public IEnumerable<Bundle> GetUnlockedBundlesForTier(SubscriptionTier tier)
    {
        try
        {
            var tierId = tier.ToString();
            var unlockedBundleIds = _tierAccess
                .Where(a => a.TierId == tierId && !string.IsNullOrEmpty(a.BundleId))
                .Select(a => a.BundleId)
                .Distinct()
                .ToList();

            return _bundles?.Where(b => unlockedBundleIds.Contains(b.Id)) ?? Enumerable.Empty<Bundle>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting unlocked bundles for tier {tier}: {ex.Message}");
            return Enumerable.Empty<Bundle>();
        }
    }

    public IEnumerable<AudioFile> GetAvailableFilesForTier(SubscriptionTier tier, IEnumerable<AudioFile>? deviceFiles = null)
    {
        try
        {
            var tierId = tier.ToString();
            var unlockedFileIds = _tierAccess
                .Where(a => a.TierId == tierId && !string.IsNullOrEmpty(a.FileId))
                .Select(a => a.FileId)
                .Distinct()
                .ToList();

            var bundleFileIds = _bundleFiles
                .Where(bf => unlockedFileIds.Contains(bf.FileId))
                .Select(bf => bf.FileId)
                .ToList();

            var files = _files
                .Where(f => unlockedFileIds.Contains(f.Id) || bundleFileIds.Contains(f.Id))
                .ToList();

            if (deviceFiles != null)
            {
                files.AddRange(deviceFiles);
            }

            return files.DistinctBy(f => f.Id);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting available files for tier {tier}: {ex.Message}");
            return deviceFiles ?? Enumerable.Empty<AudioFile>();
        }
    }

    public int GetMaxFilesForTier(SubscriptionTier tier)
    {
        try
        {
            var t = _tiers?.FirstOrDefault(x => x.Id == tier.ToString());
            return t?.MaxFiles ?? 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting max files for tier {tier}: {ex.Message}");
            return 0;
        }
    }

    public int GetMaxBundlesForTier(SubscriptionTier tier)
    {
        try
        {
            var t = _tiers?.FirstOrDefault(x => x.Id == tier.ToString());
            return t?.MaxBundles ?? 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting max bundles for tier {tier}: {ex.Message}");
            return 0;
        }
    }

    public bool IsFileUnlockedForTier(SubscriptionTier tier, string fileId)
    {
        try
        {
            if (string.IsNullOrEmpty(fileId))
                return false;

            var tierId = tier.ToString();
            return _tierAccess?.Any(a => a.TierId == tierId && a.FileId == fileId) ?? false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error checking file unlock status for {fileId}, tier {tier}: {ex.Message}");
            return false;
        }
    }

    public bool IsBundleUnlockedForTier(SubscriptionTier tier, string bundleId)
    {
        try
        {
            if (string.IsNullOrEmpty(bundleId))
                return false;

            var tierId = tier.ToString();
            return _tierAccess?.Any(a => a.TierId == tierId && a.BundleId == bundleId) ?? false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error checking bundle unlock status for {bundleId}, tier {tier}: {ex.Message}");
            return false;
        }
    }
}
