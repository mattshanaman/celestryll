namespace Pemdas.Services
{
    public class AdService : IAdService
    {
        private bool _isInitialized;
        private readonly object _initLock = new();
        private const bool AllowRewardSimulation = false;

        public void Initialize()
        {
            lock (_initLock)
            {
                if (_isInitialized)
                    return;

                try
                {
                    // Plugin.MauiMTAdmob will be integrated after package restore
                    // CrossMauiMTAdmob.Current.Init();
                    _isInitialized = true;
                    System.Diagnostics.Debug.WriteLine("AdService initialized successfully (placeholder)");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ad initialization failed: {ex.Message}");
                    _isInitialized = false;
                }
            }
        }

        public void ShowInterstitialAd()
        {
            if (!_isInitialized)
            {
                System.Diagnostics.Debug.WriteLine("Cannot show interstitial ad: AdService not initialized");
                return;
            }

            try
            {
                var adUnitId = GetInterstitialAdUnitId();
                if (string.IsNullOrEmpty(adUnitId))
                {
                    System.Diagnostics.Debug.WriteLine("No ad unit ID configured for this platform");
                    return;
                }

                // Plugin.MauiMTAdmob will be integrated after package restore
                // CrossMauiMTAdmob.Current.LoadInterstitial(adUnitId);
                System.Diagnostics.Debug.WriteLine("Interstitial ad requested (placeholder)");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to show interstitial ad: {ex.Message}");
            }
        }

        public void ShowRewardedAd(Action? onRewarded)
        {
            if (!_isInitialized)
            {
                System.Diagnostics.Debug.WriteLine("Cannot show rewarded ad: AdService not initialized");
                return;
            }

            if (onRewarded == null)
            {
                System.Diagnostics.Debug.WriteLine("Warning: ShowRewardedAd called with null callback");
            }

            try
            {
                var adUnitId = GetRewardedAdUnitId();
                if (string.IsNullOrEmpty(adUnitId))
                {
                    System.Diagnostics.Debug.WriteLine("No rewarded ad unit ID configured for this platform");
                    return;
                }

                // Plugin.MauiMTAdmob will be integrated after package restore
                System.Diagnostics.Debug.WriteLine("Rewarded ad requested (placeholder)");

                if (AllowRewardSimulation)
                {
                    OnRewardedVideoCompleted(onRewarded);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Reward not granted: rewarded ad provider not integrated yet.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to show rewarded ad: {ex.Message}");
            }
        }

        private void OnRewardedVideoCompleted(Action? onRewarded)
        {
            try
            {
                onRewarded?.Invoke();
                System.Diagnostics.Debug.WriteLine("Rewarded ad completed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in rewarded ad callback: {ex.Message}");
            }
        }

        public void ShowBannerAd()
        {
            if (!_isInitialized)
            {
                System.Diagnostics.Debug.WriteLine("Cannot show banner ad: AdService not initialized");
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine("Banner ad display not yet implemented");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to show banner ad: {ex.Message}");
            }
        }

        public void HideBannerAd()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Banner ad hide not yet implemented");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to hide banner ad: {ex.Message}");
            }
        }

        private string GetInterstitialAdUnitId()
        {
#if ANDROID
            return "ca-app-pub-3940256099942544/1033173712";
#elif IOS
            return "ca-app-pub-3940256099942544/4411468910";
#else
            return string.Empty;
#endif
        }

        private string GetRewardedAdUnitId()
        {
#if ANDROID
            return "ca-app-pub-3940256099942544/5224354917";
#elif IOS
            return "ca-app-pub-3940256099942544/1712485313";
#else
            return string.Empty;
#endif
        }
    }
}