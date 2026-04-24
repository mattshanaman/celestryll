namespace BadlyDefined.Services;

/// <summary>
/// Interface for ad services (banner, interstitial, rewarded)
/// </summary>
public interface IAdService
{
    /// <summary>
    /// Show an interstitial ad (full-screen)
    /// </summary>
    void ShowInterstitialAd();

    /// <summary>
    /// Show a banner ad at bottom of screen
    /// </summary>
    void ShowBannerAd();

    /// <summary>
    /// Hide banner ad
    /// </summary>
    void HideBannerAd();

    /// <summary>
    /// Show a rewarded ad (user watches for reward)
    /// </summary>
    /// <param name="onRewardEarned">Callback when user completes ad</param>
    void ShowRewardedAd(Action onRewardEarned);

    /// <summary>
    /// Check if rewarded ad is available
    /// </summary>
    bool IsRewardedAdAvailable();
}
