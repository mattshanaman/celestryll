using Android.App;
using Android.Content.PM;
using Android.OS;

namespace Pemdas
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Future: Initialize Ad Service when AdMob packages are added
            // var adService = Handler.MauiContext?.Services.GetService<IAdService>();
            // adService?.Initialize();
        }
    }
}
