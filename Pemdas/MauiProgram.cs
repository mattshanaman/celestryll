using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Pemdas.Pages;
using Pemdas.Services;
using Pemdas.ViewModels;
using System.Globalization;

namespace Pemdas
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentUICulture;

            // Register services
            builder.Services.AddSingleton<ErrorLoggingService>();
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<ExpressionEvaluator>();
            builder.Services.AddSingleton<IAdService, AdService>();
            builder.Services.AddSingleton<ISubscriptionService, SubscriptionService>();
            builder.Services.AddSingleton<ILocalizationService, LocalizationService>();
            builder.Services.AddSingleton<IFeedbackService, FeedbackService>();
            builder.Services.AddSingleton<GameService>();

            // Register ViewModels - GameViewModel as Singleton so test mode works correctly
            builder.Services.AddSingleton<GameViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<TestModeViewModel>();
            builder.Services.AddTransient<PastGamesViewModel>();
            
            // Register Pages
            builder.Services.AddTransient<GamePage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<TestModePage>();
            builder.Services.AddTransient<PastGamesPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
