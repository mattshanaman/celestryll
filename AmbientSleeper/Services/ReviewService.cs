using Plugin.Maui.AppRating;

namespace AmbientSleeper.Services;

public class ReviewService
{
    private readonly IAppRating _rating;

    public ReviewService(IAppRating rating)
    {
        _rating = rating;
    }

    public async Task PromptAsync()
    {
#if ANDROID
        await _rating.PerformInAppRateAsync(isTestOrDebugMode: false);
#else
        await _rating.PerformInAppRateAsync();
#endif
    }
}