using AmbientSleeper.Models;

namespace AmbientSleeper.Services;

public class FeatureGate
{
    private readonly ISubscriptionService _subscription;
    private readonly IAdRewardManager? _adRewardManager;

    public FeatureGate(ISubscriptionService subscription, IAdRewardManager? adRewardManager = null)
    {
        _subscription = subscription;
        _adRewardManager = adRewardManager;
        _subscription.TierChanged += (_, __) => Changed?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? Changed;

    public SubscriptionTier Tier => _subscription.CurrentTier;

    public int AllowedBundles => Tier switch
    {
        SubscriptionTier.Free => 1,
        SubscriptionTier.Standard => 2,
        SubscriptionTier.Premium => 4,
        SubscriptionTier.ProPlus => int.MaxValue,
        _ => 1
    };

    public int AllowedLanguages => Tier switch
    {
        SubscriptionTier.ProPlus => int.MaxValue,
        _ => 1
    };

    public TimeSpan MaxSessionLength()
    {
        // Free tier: 15 minutes base, +45 minutes if ad extension active
        if (Tier == SubscriptionTier.Free)
        {
            var baseTime = TimeSpan.FromMinutes(15);
            if (_adRewardManager?.HasSessionExtension == true)
            {
                return baseTime + _adRewardManager.RemainingExtensionTime;
            }
            return baseTime;
        }
        
        return Tier == SubscriptionTier.ProPlus ? TimeSpan.MaxValue
            : Tier == SubscriptionTier.Premium ? TimeSpan.FromHours(8)
            : Tier == SubscriptionTier.Standard ? TimeSpan.FromHours(2)
            : TimeSpan.FromMinutes(15);
    }

    public int MaxOverlaySounds => Tier switch
    {
        SubscriptionTier.Free => 2, // Basic overlay
        SubscriptionTier.Standard => _subscription.IsLifetime ? int.MaxValue : 5, // lifetime = full mix mode, // Enhanced overlay
        SubscriptionTier.Premium => 8, // Advanced overlay
        SubscriptionTier.ProPlus => 16, // Pro overlay
        _ => 1
    };

    public int MaxSavedMixes => Tier switch
    {
        SubscriptionTier.Free => 1,
        SubscriptionTier.Standard => 3,
        SubscriptionTier.Premium => 10,
        SubscriptionTier.ProPlus => 1000,
        _ => 1
    };

    public int MaxSavedPlaylists => Tier switch
    {
        SubscriptionTier.Free => 0,
        SubscriptionTier.Standard => 5,
        SubscriptionTier.Premium => 20,
        SubscriptionTier.ProPlus => 1000,
        _ => 0
    };

    public int MaxFadeSeconds => Tier switch
    {
        SubscriptionTier.Free => 4,
        SubscriptionTier.Standard => 15,
        SubscriptionTier.Premium => 30,
        SubscriptionTier.ProPlus => 30,
        _ => 4
    };

    public bool PlaylistEnabled => Tier is SubscriptionTier.Standard or SubscriptionTier.Premium or SubscriptionTier.ProPlus;
    public bool ExtendedSessionsEnabled => Tier is not SubscriptionTier.Free;
    public bool CustomEqEnabled => Tier is SubscriptionTier.Premium or SubscriptionTier.ProPlus;
    public bool AlarmIntegrationEnabled => Tier is SubscriptionTier.Standard or SubscriptionTier.Premium or SubscriptionTier.ProPlus;
    public bool AdvancedEditorEnabled => Tier is SubscriptionTier.ProPlus;
    public bool GlobalAccessEnabled => Tier is SubscriptionTier.ProPlus;

    // NEW: export gates
    public bool ExportEnabled => Tier is not SubscriptionTier.Free;
    public bool ShareExportEnabled => Tier is SubscriptionTier.Premium or SubscriptionTier.ProPlus;

    // NEW: Mix Playlists
    public bool MixPlaylistEnabled => Tier is SubscriptionTier.Premium or SubscriptionTier.ProPlus;
    public int MaxSavedMixPlaylists => Tier switch
    {
        SubscriptionTier.Premium => 10,
        SubscriptionTier.ProPlus => 200,
        _ => 0
    };
    public int MaxMixesPerMixPlaylist => Tier switch
    {
        SubscriptionTier.Premium => 5,
        SubscriptionTier.ProPlus => 50,
        _ => 0
    };
    public bool MixPlaylistTransitionsEnabled => Tier is SubscriptionTier.ProPlus;
    
    public int MaxMixTransitionSeconds => Tier switch
    {
        SubscriptionTier.Premium => 10,
        SubscriptionTier.ProPlus => 30,
        _ => 0
    };
    
    // NEW: Ad-related features
    public bool ShouldShowAds => Tier == SubscriptionTier.Free;
    
    /// <summary>
    /// Check if a sound bundle is accessible (either always available or unlocked via ad)
    /// </summary>
    public bool IsSoundBundleAccessible(string bundleId)
    {
        // Paid tiers have access to all
        if (Tier != SubscriptionTier.Free)
            return true;
        
        // Free tier: check if unlocked via ad
        return _adRewardManager?.IsSoundUnlocked(bundleId) == true;
    }
}