using BadlyDefined.Services;
using System.Diagnostics;

namespace BadlyDefined.Platforms;

/// <summary>
/// Mock ad service for testing (replace with real implementation later)
/// </summary>
public class MockAdService : IAdService
{
    public void ShowInterstitialAd()
    {
        Debug.WriteLine("📺 [MOCK] Interstitial ad shown");
        // TODO: Implement real ad service (AdMob, etc.)
    }

    public void ShowBannerAd()
    {
        Debug.WriteLine("📺 [MOCK] Banner ad shown");
    }

    public void HideBannerAd()
    {
        Debug.WriteLine("📺 [MOCK] Banner ad hidden");
    }

    public void ShowRewardedAd(Action onRewardEarned)
    {
        Debug.WriteLine("📺 [MOCK] Rewarded ad shown");
        // Simulate ad completion
        onRewardEarned?.Invoke();
    }

    public bool IsRewardedAdAvailable()
    {
        return true; // Mock always available
    }
}
