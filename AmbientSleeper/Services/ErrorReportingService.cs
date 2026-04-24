using System.Text;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace AmbientSleeper.Services;

public class ErrorReport
{
    public string ErrorId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Operation { get; set; } = string.Empty;
    public string ErrorType { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public Dictionary<string, string> Context { get; set; } = new();
}

public interface IErrorReportingService
{
    /// <summary>
    /// Records an error for later reporting
    /// </summary>
    void RecordError(ErrorReport error);
    
    /// <summary>
    /// Gets all recorded errors
    /// </summary>
    List<ErrorReport> GetErrors();
    
    /// <summary>
    /// Generates a shareable error report
    /// </summary>
    Task<string> GenerateReportAsync();
    
    /// <summary>
    /// Shares error report via system share sheet
    /// </summary>
    Task ShareReportAsync();
    
    /// <summary>
    /// Clears all recorded errors
    /// </summary>
    void ClearErrors();
}

public class ErrorReportingService : IErrorReportingService
{
    private readonly List<ErrorReport> _errors = new();
    private readonly object _lock = new();
    private const int MaxErrors = 100;

    public void RecordError(ErrorReport error)
    {
        if (error == null) return;

        lock (_lock)
        {
            _errors.Add(error);
            
            // Keep only the most recent errors
            if (_errors.Count > MaxErrors)
            {
                _errors.RemoveRange(0, _errors.Count - MaxErrors);
            }
        }
    }

    public List<ErrorReport> GetErrors()
    {
        lock (_lock)
        {
            return new List<ErrorReport>(_errors);
        }
    }

    public async Task<string> GenerateReportAsync()
    {
        var report = new StringBuilder();
        report.AppendLine("=== Ambient Sleeper Error Report ===");
        report.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        report.AppendLine($"Device: {DeviceInfo.Model} ({DeviceInfo.Platform} {DeviceInfo.VersionString})");
        report.AppendLine($"App Version: {AppInfo.VersionString}");
        report.AppendLine();

        List<ErrorReport> errors;
        lock (_lock)
        {
            errors = new List<ErrorReport>(_errors);
        }

        if (errors.Count == 0)
        {
            report.AppendLine("No errors recorded.");
        }
        else
        {
            report.AppendLine($"Total Errors: {errors.Count}");
            report.AppendLine();

            foreach (var error in errors.OrderByDescending(e => e.Timestamp))
            {
                report.AppendLine("---");
                report.AppendLine($"Error ID: {error.ErrorId}");
                report.AppendLine($"Timestamp: {error.Timestamp:yyyy-MM-dd HH:mm:ss} UTC");
                report.AppendLine($"Operation: {error.Operation}");
                report.AppendLine($"Type: {error.ErrorType}");
                report.AppendLine($"Message: {error.ErrorMessage}");

                if (!string.IsNullOrEmpty(error.StackTrace))
                {
                    report.AppendLine("Stack Trace:");
                    report.AppendLine(error.StackTrace);
                }

                if (error.Context.Count > 0)
                {
                    report.AppendLine("Context:");
                    foreach (var kvp in error.Context)
                    {
                        report.AppendLine($"  {kvp.Key}: {kvp.Value}");
                    }
                }

                report.AppendLine();
            }
        }

        // Add health check info
        var (isHealthy, issues) = UserPreferences.CheckHealth();
        report.AppendLine("=== System Health Check ===");
        report.AppendLine($"Status: {(isHealthy ? "HEALTHY" : "ISSUES DETECTED")}");
        
        if (issues.Count > 0)
        {
            report.AppendLine("Issues:");
            foreach (var issue in issues)
            {
                report.AppendLine($"  - {issue}");
            }
        }

        return report.ToString();
    }

    public async Task ShareReportAsync()
    {
        try
        {
            var reportText = await GenerateReportAsync();
            var fileName = $"AmbientSleeper_ErrorReport_{DateTime.UtcNow:yyyyMMdd_HHmmss}.txt";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            await File.WriteAllTextAsync(filePath, reportText);

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Share Error Report",
                File = new ShareFile(filePath)
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to share error report: {ex.Message}");
        }
    }

    public void ClearErrors()
    {
        lock (_lock)
        {
            _errors.Clear();
        }
    }
}
