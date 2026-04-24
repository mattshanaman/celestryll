using BadlyDefined.Pages;
using BadlyDefined.Services;
using BadlyDefined.ViewModels;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace BadlyDefined;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        System.Diagnostics.Debug.WriteLine("🔧 MauiProgram.CreateMauiApp started");

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
        builder.Logging.SetMinimumLevel(LogLevel.Trace);
        System.Diagnostics.Debug.WriteLine("✅ Debug logging enabled");
#endif

        // Services
        System.Diagnostics.Debug.WriteLine("🔧 Registering services...");
        builder.Services.AddSingleton<ErrorLoggingService>();
        System.Diagnostics.Debug.WriteLine("✅ ErrorLoggingService registered");

        builder.Services.AddSingleton<DatabaseService>();
        System.Diagnostics.Debug.WriteLine("✅ DatabaseService registered");

        builder.Services.AddSingleton<GameService>();
        System.Diagnostics.Debug.WriteLine("✅ GameService registered");

        builder.Services.AddSingleton<IAdService, MockAdService>();
        builder.Services.AddSingleton<ISubscriptionService, MockSubscriptionService>();
        builder.Services.AddSingleton<IFeedbackService, FeedbackService>();
        System.Diagnostics.Debug.WriteLine("✅ Mock services registered");

        // ViewModels
        builder.Services.AddTransient<GameViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<TestModeViewModel>();
        System.Diagnostics.Debug.WriteLine("✅ ViewModels registered");

        // Pages
        builder.Services.AddTransient<GamePage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<TestModePage>();
        System.Diagnostics.Debug.WriteLine("✅ Pages registered");

        System.Diagnostics.Debug.WriteLine("🎉 MauiProgram configuration complete");

        var app = builder.Build();
        System.Diagnostics.Debug.WriteLine("✅ MauiApp built successfully");
        return app;
    }
}
