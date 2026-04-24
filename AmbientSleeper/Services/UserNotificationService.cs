using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace AmbientSleeper.Services;

public class UserNotificationService : IUserNotificationService
{
    public async Task ShowToastAsync(string message, NotificationType type = NotificationType.Info, int durationMs = 3000)
    {
        try
        {
            var toast = Toast.Make(message, ToastDuration.Long, 14);
            await toast.Show();
        }
        catch (Exception ex)
        {
            // Fallback to debug output
            System.Diagnostics.Debug.WriteLine($"Toast failed: {message}, Error: {ex.Message}");
        }
    }

    public async Task ShowErrorDialogAsync(string title, string message, string buttonText = "OK")
    {
        try
        {
            var shell = Shell.Current;
            if (shell != null)
            {
                await shell.DisplayAlert(title, message, buttonText);
            }
            else
            {
                // Fallback if Shell is not available
                System.Diagnostics.Debug.WriteLine($"Error Dialog: {title} - {message}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error dialog failed: {ex.Message}");
        }
    }

    public async Task<bool> ShowWarningAsync(string title, string message, string acceptText = "OK", string? cancelText = null)
    {
        try
        {
            var shell = Shell.Current;
            if (shell != null)
            {
                if (string.IsNullOrEmpty(cancelText))
                {
                    await shell.DisplayAlert(title, message, acceptText);
                    return true;
                }
                else
                {
                    return await shell.DisplayAlert(title, message, acceptText, cancelText);
                }
            }
            else
            {
                // Fallback if Shell is not available
                System.Diagnostics.Debug.WriteLine($"Warning Dialog: {title} - {message}");
                return true;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Warning dialog failed: {ex.Message}");
            return false;
        }
    }
}
