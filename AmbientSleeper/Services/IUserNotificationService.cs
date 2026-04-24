namespace AmbientSleeper.Services;

public enum NotificationType
{
    Info,
    Warning,
    Error,
    Success
}

public interface IUserNotificationService
{
    /// <summary>
    /// Shows a toast notification to the user
    /// </summary>
    Task ShowToastAsync(string message, NotificationType type = NotificationType.Info, int durationMs = 3000);
    
    /// <summary>
    /// Shows a persistent error message that requires user acknowledgment
    /// </summary>
    Task ShowErrorDialogAsync(string title, string message, string buttonText = "OK");
    
    /// <summary>
    /// Shows a warning message with optional action
    /// </summary>
    Task<bool> ShowWarningAsync(string title, string message, string acceptText = "OK", string? cancelText = null);
}
