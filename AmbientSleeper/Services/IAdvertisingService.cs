namespace AmbientSleeper.Services;

/// <summary>
/// Service for managing advertisements in the Free tier
/// </summary>
public interface IAdvertisingService
{
    /// <summary>
    /// Whether ads should be shown (Free tier only)
    /// </summary>
    bool ShouldShowAds { get; }
    
    /// <summary>
    /// Whether ads are loaded and ready to display
    /// </summary>
    bool AreAdsReady { get; }
    
    /// <summary>
    /// Initialize the advertising SDK
    /// </summary>
    Task InitializeAsync();
    
    /// <summary>
    /// Show a banner ad on the specified page
    /// </summary>
    /// <param name="pageType">Type of page (Library, Settings, Timer, etc.)</param>
    void ShowBanner(string pageType);
    
    /// <summary>
    /// Hide any visible banner ads
    /// </summary>
    void HideBanner();
    
    /// <summary>
    /// Show an interstitial ad (full screen)
    /// </summary>
    /// <param name="trigger">What triggered the ad (AppOpen, SessionEnd)</param>
    Task ShowInterstitialAsync(string trigger);
    
    /// <summary>
    /// Show a rewarded ad and return whether user completed it
    /// </summary>
    /// <param name="rewardType">Type of reward (ExtendSession, UnlockSound)</param>
    Task<bool> ShowRewardedAdAsync(string rewardType);
    
    /// <summary>
    /// Check if a rewarded ad is available
    /// </summary>
    bool IsRewardedAdAvailable(string rewardType);
    
    /// <summary>
    /// Get remaining time before next interstitial can be shown
    /// </summary>
    TimeSpan GetInterstitialCooldown();
    
    /// <summary>
    /// Preload ads for better performance
    /// </summary>
    Task PreloadAdsAsync();
}

/// <summary>
/// Tracks ad-based rewards and unlocks for Free tier users
/// </summary>
public interface IAdRewardManager
{
    /// <summary>
    /// Check if user has an active session extension from watching an ad
    /// </summary>
    bool HasSessionExtension { get; }
    
    /// <summary>
    /// Get remaining time on session extension
    /// </summary>
    TimeSpan RemainingExtensionTime { get; }
    
    /// <summary>
    /// Grant session extension from rewarded ad
    /// </summary>
    /// <param name="duration">Duration to extend (typically 30-60 minutes)</param>
    void GrantSessionExtension(TimeSpan duration);
    
    /// <summary>
    /// Check if a specific sound bundle is unlocked via ad
    /// </summary>
    bool IsSoundUnlocked(string bundleId);
    
    /// <summary>
    /// Unlock a sound bundle for current session
    /// </summary>
    void UnlockSound(string bundleId);
    
    /// <summary>
    /// Clear session-based unlocks (called when app closes or session ends)
    /// </summary>
    void ClearSessionUnlocks();
    
    /// <summary>
    /// Get all currently unlocked bundle IDs
    /// </summary>
    IReadOnlyList<string> GetUnlockedBundles();
}
