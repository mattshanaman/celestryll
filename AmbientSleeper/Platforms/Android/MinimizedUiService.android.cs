using AmbientSleeper.Services;

namespace AmbientSleeper;

// Simplified Android implementation to avoid platform Notification API issues in this build.
// Can be expanded later to show a real ongoing notification.
public class MinimizedUiService : IMinimizedUiService
{
    public void ShowOrUpdate(string title, string message, bool isPlaying)
    {
        // No-op for now
    }

    public void Hide()
    {
        // No-op for now
    }
}
