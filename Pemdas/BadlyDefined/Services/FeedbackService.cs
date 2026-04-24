using BadlyDefined.Services;

namespace BadlyDefined.Services;

/// <summary>
/// Cross-platform feedback service using MAUI Essentials
/// </summary>
public class FeedbackService : IFeedbackService
{
    public async Task PlaySuccessFeedback()
    {
        try
        {
#if ANDROID || IOS
            HapticFeedback.Perform(HapticFeedbackType.Click);
#endif
            await Task.CompletedTask;
        }
        catch
        {
            // Ignore feedback errors
        }
    }

    public async Task PlayErrorFeedback()
    {
        try
        {
#if ANDROID || IOS
            HapticFeedback.Perform(HapticFeedbackType.LongPress);
#endif
            await Task.CompletedTask;
        }
        catch
        {
            // Ignore feedback errors
        }
    }

    public async Task LightTap()
    {
        try
        {
#if ANDROID || IOS
            HapticFeedback.Perform(HapticFeedbackType.Click);
#endif
            await Task.CompletedTask;
        }
        catch
        {
            // Ignore feedback errors
        }
    }

    public async Task MediumImpact()
    {
        try
        {
#if ANDROID || IOS
            HapticFeedback.Perform(HapticFeedbackType.Click);
#endif
            await Task.CompletedTask;
        }
        catch
        {
            // Ignore feedback errors
        }
    }

    public async Task HeavyImpact()
    {
        try
        {
#if ANDROID || IOS
            HapticFeedback.Perform(HapticFeedbackType.LongPress);
#endif
            await Task.CompletedTask;
        }
        catch
        {
            // Ignore feedback errors
        }
    }
}
