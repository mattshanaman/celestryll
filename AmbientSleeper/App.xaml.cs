using AmbientSleeper.Services;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;


namespace AmbientSleeper
{
    public partial class App : Application
    {
        private IAdvertisingService? _adService;
        private IAdRewardManager? _rewardManager;
        private bool _isFirstLaunch = true;

        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override async void OnStart()
        {
            base.OnStart();
            
            // Get ad services
            _adService = ServiceHost.Services?.GetService<IAdvertisingService>();
            _rewardManager = ServiceHost.Services?.GetService<IAdRewardManager>();
            
            // Initialize ads for Free tier
            if (_adService is not null && _adService.ShouldShowAds)
            {
                await _adService.InitializeAsync();
                
                // Show app open interstitial if not first launch
                if (!_isFirstLaunch)
                {
                    await _adService.ShowInterstitialAsync("AppOpen");
                }
                _isFirstLaunch = false;
                
                // Preload ads for better UX
                _ = Task.Run(async () => await _adService.PreloadAdsAsync());
            }
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            
            // Note: Don't clear session unlocks on sleep, only on true app termination
            System.Diagnostics.Debug.WriteLine("[App] Sleeping - keeping ad rewards active");
        }

        protected override void OnResume()
        {
            base.OnResume();
            
            System.Diagnostics.Debug.WriteLine("[App] Resuming");
            
            // Reload ads if needed
            if (_adService is not null && _adService.ShouldShowAds)
            {
                _ = Task.Run(async () => await _adService.PreloadAdsAsync());
            }
        }
    }
}