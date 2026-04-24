using Foundation;
using AVFoundation;
using UIKit;

namespace AmbientSleeper;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        // Enable background audio playback
        var session = AVAudioSession.SharedInstance();
        session.SetCategory(AVAudioSessionCategory.Playback, AVAudioSessionCategoryOptions.MixWithOthers, out _);
        session.SetActive(true, out _);

        return base.FinishedLaunching(application, launchOptions);
    }
}