namespace AmbientSleeper.Services;

public interface IMinimizedUiService
{
    // Show or update the minimized playback UI (platform specific)
    void ShowOrUpdate(string title, string message, bool isPlaying);

    // Hide/stop the minimized playback UI
    void Hide();
}
