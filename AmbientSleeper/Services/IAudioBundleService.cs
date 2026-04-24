using System.Collections.Generic;
using AmbientSleeper.Models;

namespace AmbientSleeper.Services;

public interface IAudioBundleService
{
    // Returns all bundles available for the current region
    IEnumerable<Bundle> GetAllBundles();

    // Returns all audio files available for the current tier (including device files if allowed)
    IEnumerable<AudioFile> GetAvailableFilesForTier(SubscriptionTier tier, IEnumerable<AudioFile>? deviceFiles = null);

    // Returns all bundles unlocked for the current tier
    IEnumerable<Bundle> GetUnlockedBundlesForTier(SubscriptionTier tier);

    // Returns the file cap for the current tier
    int GetMaxFilesForTier(SubscriptionTier tier);

    // Returns the bundle cap for the current tier
    int GetMaxBundlesForTier(SubscriptionTier tier);

    // Returns true if the file is unlocked for the given tier
    bool IsFileUnlockedForTier(SubscriptionTier tier, string fileId);

    // Returns true if the bundle is unlocked for the given tier
    bool IsBundleUnlockedForTier(SubscriptionTier tier, string bundleId);
}
