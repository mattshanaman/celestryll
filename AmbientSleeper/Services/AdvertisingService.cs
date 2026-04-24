using AmbientSleeper.Models;

namespace AmbientSleeper.Services;

/// <summary>
/// Cross-platform advertising service implementation
/// Delegates to platform-specific implementations when available
/// </summary>
public partial class AdvertisingService : IAdvertisingService
{
    private readonly ISubscriptionService _subscription;
    private readonly IAdRewardManager _rewardManager;
    private DateTime? _lastInterstitialTime;
    private readonly TimeSpan _interstitialCooldown = TimeSpan.FromMinutes(5); // Min 5 min between interstitials
    private bool _isInitialized;
    
    public AdvertisingService(ISubscriptionService subscription, IAdRewardManager rewardManager)
    {
        _subscription = subscription;
        _rewardManager = rewardManager;
    }
    
    public bool ShouldShowAds => _subscription.CurrentTier == SubscriptionTier.Free;
    
    public bool AreAdsReady { get; private set; }
    
    public async Task InitializeAsync()
    {
        if (_isInitialized)
            return;
        
        if (!ShouldShowAds)
        {
            System.Diagnostics.Debug.WriteLine("[Ads] Paid tier - ads disabled");
            return;
        }
        
        System.Diagnostics.Debug.WriteLine("[Ads] Initializing advertising SDK...");
        
        try
        {
            InitializePlatformAds();
            _isInitialized = true;
            AreAdsReady = true;
            System.Diagnostics.Debug.WriteLine("[Ads] Initialization complete");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Ads] Initialization failed: {ex.Message}");
            AreAdsReady = false;
        }
    }
    
    public void ShowBanner(string pageType)
    {
        if (!ShouldShowAds || !AreAdsReady)
            return;
        
        // Don't show banners on Playback page
        if (pageType.Equals("Playback", StringComparison.OrdinalIgnoreCase))
        {
            HideBanner();
            return;
        }
        
        System.Diagnostics.Debug.WriteLine($"[Ads] Showing banner on {pageType}");
        ShowPlatformBanner(pageType);
    }
    
    public void HideBanner()
    {
        if (!AreAdsReady)
            return;
        
        System.Diagnostics.Debug.WriteLine("[Ads] Hiding banner");
        HidePlatformBanner();
    }
    
    public async Task ShowInterstitialAsync(string trigger)
    {
        if (!ShouldShowAds || !AreAdsReady)
            return;
        
        // Check cooldown
        if (_lastInterstitialTime.HasValue)
        {
            var timeSince = DateTime.UtcNow - _lastInterstitialTime.Value;
            if (timeSince < _interstitialCooldown)
            {
                System.Diagnostics.Debug.WriteLine($"[Ads] Interstitial on cooldown ({timeSince.TotalSeconds:F0}s / {_interstitialCooldown.TotalSeconds}s)");
                return;
            }
        }
        
        System.Diagnostics.Debug.WriteLine($"[Ads] Showing interstitial: {trigger}");
        
        try
        {
            ShowPlatformInterstitial(trigger);
            _lastInterstitialTime = DateTime.UtcNow;
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Ads] Interstitial failed: {ex.Message}");
        }
    }
    
    public async Task<bool> ShowRewardedAdAsync(string rewardType)
    {
        if (!ShouldShowAds || !AreAdsReady)
            return false;
        
        if (!IsRewardedAdAvailable(rewardType))
        {
            System.Diagnostics.Debug.WriteLine($"[Ads] Rewarded ad not available: {rewardType}");
            return false;
        }
        
        System.Diagnostics.Debug.WriteLine($"[Ads] Showing rewarded ad: {rewardType}");
        
        try
        {
            var completed = await ShowPlatformRewardedAdAsync(rewardType);
            
            if (completed)
            {
                // Grant reward
                GrantReward(rewardType);
                System.Diagnostics.Debug.WriteLine($"[Ads] Reward granted: {rewardType}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[Ads] User did not complete ad");
            }
            
            return completed;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Ads] Rewarded ad failed: {ex.Message}");
            return false;
        }
    }
    
    public bool IsRewardedAdAvailable(string rewardType)
    {
        if (!ShouldShowAds || !AreAdsReady)
            return false;
        
        return CheckPlatformRewardedAdAvailableInternal(rewardType);
    }
    
    public TimeSpan GetInterstitialCooldown()
    {
        if (!_lastInterstitialTime.HasValue)
            return TimeSpan.Zero;
        
        var elapsed = DateTime.UtcNow - _lastInterstitialTime.Value;
        var remaining = _interstitialCooldown - elapsed;
        
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }
    
    public async Task PreloadAdsAsync()
    {
        if (!ShouldShowAds || !AreAdsReady)
            return;
        
        System.Diagnostics.Debug.WriteLine("[Ads] Preloading ads...");
        
        try
        {
            PreloadPlatformAds();
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Ads] Preload failed: {ex.Message}");
        }
    }
    
    private void GrantReward(string rewardType)
    {
        switch (rewardType.ToLowerInvariant())
        {
            case "extendsession":
                _rewardManager.GrantSessionExtension(TimeSpan.FromMinutes(45));
                break;
            
            case "unlocksound":
                // Platform implementation should provide bundle ID
                // For now, this is a placeholder
                System.Diagnostics.Debug.WriteLine("[Ads] Sound unlock reward - requires bundle ID");
                break;
            
            default:
                System.Diagnostics.Debug.WriteLine($"[Ads] Unknown reward type: {rewardType}");
                break;
        }
    }
    
    // Platform-specific partial methods (implemented in platform projects)
    partial void InitializePlatformAds();
    partial void ShowPlatformBanner(string pageType);
    partial void HidePlatformBanner();
    partial void ShowPlatformInterstitial(string trigger);
    partial void PreloadPlatformAds();
    
    // Platform-specific async methods with default implementations
    protected virtual Task<bool> ShowPlatformRewardedAdAsync(string rewardType)
    {
        // Default implementation - override in platform-specific code
        System.Diagnostics.Debug.WriteLine("[Ads] ShowPlatformRewardedAdAsync not implemented on this platform");
        return Task.FromResult(false);
    }
    
    protected virtual bool CheckPlatformRewardedAdAvailableInternal(string rewardType)
    {
        // Default implementation - override in platform-specific code
        return false;
    }
}
