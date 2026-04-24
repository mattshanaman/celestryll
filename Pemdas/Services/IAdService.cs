namespace Pemdas.Services
{
    public interface IAdService
    {
        void Initialize();
        void ShowInterstitialAd();
        void ShowRewardedAd(Action onRewarded);
        void ShowBannerAd();
        void HideBannerAd();
    }
}