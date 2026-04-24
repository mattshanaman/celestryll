using Android.App;
using Android.OS;
using AndroidX.Core.App;
using Android.Content;
using Android.Content.PM;

namespace AmbientSleeper
{
    [Service(
        Exported = false,
        ForegroundServiceType = ForegroundService.TypeMediaPlayback
    )]
    public class PlaybackNotificationService : Service
    {
        public override IBinder? OnBind(Intent? intent) => null;

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            const string CHANNEL_ID = "media_playback_channel";

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new global::Android.App.NotificationChannel(
                        CHANNEL_ID,
                        new global::Java.Lang.String("Media Playback"),
                        global::Android.App.NotificationImportance.Low)
                    {
                        Description = "Playback controls and status"
                    };

                var manager = (global::Android.App.NotificationManager?)GetSystemService(NotificationService);
                manager?.CreateNotificationChannel(channel);
            }

            // Ensure a valid icon
            var icon = MainActivity.GetPlaybackIcon();
            if (icon == 0)
            {
                icon = global::Android.Resource.Drawable.IcMediaPlay;
            }

            var builder = new NotificationCompat.Builder(this, CHANNEL_ID);
            var builderWithTitle = builder.SetContentTitle(new global::Java.Lang.String("Ambient Sound Playing"));
            if (builderWithTitle == null) return StartCommandResult.NotSticky;
            
            var builderWithIcon = builderWithTitle.SetSmallIcon(icon);
            if (builderWithIcon == null) return StartCommandResult.NotSticky;
            
            var builderComplete = builderWithIcon.SetOngoing(true);
            if (builderComplete == null) return StartCommandResult.NotSticky;

            var notification = builderComplete.Build();
            if (notification == null) return StartCommandResult.NotSticky;

            StartForeground(1, notification, ForegroundService.TypeMediaPlayback);

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            StopForeground(true);
            base.OnDestroy();
        }
    }

}
