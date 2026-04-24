using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Microsoft.Maui;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace AmbientSleeper;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, Exported = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation |
                           ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize,
    ResizeableActivity = true,
    ScreenOrientation = ScreenOrientation.Unspecified)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Ensure volume buttons control media stream
        VolumeControlStream = global::Android.Media.Stream.Music;

        // ✅ Start the playback notification service
        var intent = new global::Android.Content.Intent(global::Android.App.Application.Context, typeof(AmbientSleeper.PlaybackNotificationService));
        global::Android.App.Application.Context.StartForegroundService(intent);
    }

    internal static int GetPlaybackIcon()
    {
        return global::Android.Resource.Drawable.IcMediaPlay;
    }
}