using AmbientSleeper.Services;
using AmbientSleeper.ViewModels;
using AmbientSleeper.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using Plugin.Maui.Audio;
using Plugin.Maui.AppRating;

namespace AmbientSleeper
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<AmbientSleeper.App>()
                .UseMauiCommunityToolkit()
                .UseLocalNotification()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            // Services
            builder.Services.AddSingleton<IAudioService, AudioService>();
            var appRating = AppRating.Default;
            if (appRating != null)
            {
                builder.Services.AddSingleton<IAppRating>(appRating);
            }

#if ANDROID
            builder.Services.AddSingleton<IAlarmSoundService, AmbientSleeper.Platforms.Android.Services.AlarmSoundService>();
#endif
#if IOS
            builder.Services.AddSingleton<IAlarmSoundService, AmbientSleeper.Platforms.iOS.Services.AlarmSoundService>();
#endif
            builder.Services.AddSingleton<IPlaybackOrchestrator, PlaybackOrchestrator>();
            builder.Services.AddSingleton<ISubscriptionService, SubscriptionService>();
            builder.Services.AddSingleton<IEqService, EqService>();
            
            // Ad services for Free tier
            builder.Services.AddSingleton<IAdRewardManager, AdRewardManager>();
            builder.Services.AddSingleton<IAdvertisingService, AdvertisingService>();
            
            builder.Services.AddSingleton<FeatureGate>(sp => 
                new FeatureGate(
                    sp.GetRequiredService<ISubscriptionService>(),
                    sp.GetRequiredService<IAdRewardManager>()));

            // Error handling and notification services
            builder.Services.AddSingleton<IUserNotificationService, UserNotificationService>();
            builder.Services.AddSingleton<IErrorReportingService, ErrorReportingService>();

            // NEW: export/import
            builder.Services.AddSingleton<IExportService, ExportService>();
            builder.Services.AddSingleton<IAudioBundleService, AudioBundleService>();

            // VMs
            builder.Services.AddSingleton<TimerViewModel>();
            builder.Services.AddSingleton<PlaybackViewModel>(sp =>
            {
                var timer = sp.GetRequiredService<TimerViewModel>();
                var vm = new PlaybackViewModel(
                    sp.GetRequiredService<IPlaybackOrchestrator>(),
                    sp.GetRequiredService<IAlarmSoundService>(),
                    timer,
                    sp.GetRequiredService<FeatureGate>());
                timer.Playback = vm;
                return vm;
            });
            // Register LibraryViewModel with all dependencies
            builder.Services.AddSingleton<LibraryViewModel>(sp =>
                new LibraryViewModel(
                    sp.GetRequiredService<PlaybackViewModel>(),
                    sp.GetRequiredService<FeatureGate>(),
                    sp.GetRequiredService<IAudioBundleService>()));
            builder.Services.AddSingleton<PlaybackSettingsViewModel>();
            builder.Services.AddSingleton<ReviewService>();

#if ANDROID
            builder.Services.AddSingleton<IMinimizedUiService, MinimizedUiService>();
#else
            builder.Services.AddSingleton<IMinimizedUiService, DummyMinimizedUiService>();
#endif

            // Views (optional; Shell can still construct, but DI helps reuse)
            builder.Services.AddSingleton<PlaybackPage>();
            builder.Services.AddSingleton<LibraryPage>();
            builder.Services.AddSingleton<TimerPage>();
            builder.Services.AddSingleton<PlaybackSettingsPage>();
            builder.Services.AddSingleton<SettingsPage>();
            builder.Services.AddSingleton<UpgradePage>();
            builder.Services.AddSingleton<EqPage>();

            var app = builder.Build();

            // Initialize ServiceHost
            ServiceHost.Init(app.Services);

            // Initialize logging for static classes
            var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
            UserPreferences.SetLogger(loggerFactory.CreateLogger("UserPreferences"));
            AudioService.SetLogger(loggerFactory.CreateLogger<AudioService>());

            // Set up telemetry for UserPreferences
            var errorReporting = app.Services.GetRequiredService<IErrorReportingService>();
            var notificationService = app.Services.GetRequiredService<IUserNotificationService>();
            
            UserPreferences.SetTelemetryCallback(errorEvent =>
            {
                // Record error for reporting
                errorReporting.RecordError(new ErrorReport
                {
                    Operation = errorEvent.Operation,
                    ErrorType = errorEvent.ErrorType,
                    ErrorMessage = errorEvent.ErrorMessage ?? "Unknown error",
                    Timestamp = errorEvent.Timestamp,
                    Context = new Dictionary<string, string>
                    {
                        ["Source"] = "UserPreferences"
                    }
                });

                // Show toast for recoverable errors (read operations)
                if (errorEvent.Operation.StartsWith("Get", StringComparison.Ordinal))
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await notificationService.ShowToastAsync(
                            "Some settings could not be loaded. Using defaults.",
                            NotificationType.Warning);
                    });
                }
                // Show error dialog for critical write failures
                else if (errorEvent.Operation.StartsWith("Save", StringComparison.Ordinal) || 
                         errorEvent.Operation.StartsWith("Delete", StringComparison.Ordinal))
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await notificationService.ShowToastAsync(
                            $"Failed to save changes. Please try again.",
                            NotificationType.Error);
                    });
                }
            });

            // Run health check on startup (off UI thread)
            _ = Task.Run(async () =>
            {
                // Give the app time to fully initialize
                await Task.Delay(2000);

                var startupLogger = loggerFactory.CreateLogger("Startup");

                // Warm cache in the background to avoid startup UI stalls
                try
                {
                    await UserPreferences.WarmupCacheAsync();
                }
                catch (Exception warmupEx)
                {
                    startupLogger.LogWarning(warmupEx, "Cache warmup failed, continuing anyway");
                }

                var (isHealthy, issues) = UserPreferences.CheckHealth();

                if (!isHealthy)
                {
                    startupLogger.LogWarning("Preferences health check failed with {IssueCount} issues", issues.Count);

                    // Marshal only UI work to the main thread.
                    if (issues.Any(i => i.Contains("not accessible", StringComparison.OrdinalIgnoreCase)))
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await notificationService.ShowErrorDialogAsync(
                                "Storage Warning",
                                "Some app settings may not save correctly. Please check your device storage permissions.");
                        });
                    }
                }
            });

            return app;
        }
    }
}

public class DummyMinimizedUiService : IMinimizedUiService
{
    public void ShowOrUpdate(string title, string message, bool isPlaying) { }
    public void Hide() { }
}

public static class ServiceHost
{
    public static IServiceProvider? Services { get; private set; }

    public static void Init(IServiceProvider services) => Services = services;
}
