using System.Diagnostics;
using System.Text;

namespace BadlyDefined.Services;

/// <summary>
/// Centralized error logging and handling service with detailed diagnostics
/// </summary>
public class ErrorLoggingService
{
    private readonly string _logFilePath;
    private readonly SemaphoreSlim _writeLock = new(1, 1);
    private const int MaxLogFileSizeBytes = 5 * 1024 * 1024; // 5MB

    public ErrorLoggingService()
    {
        _logFilePath = Path.Combine(FileSystem.AppDataDirectory, "error_log.txt");
    }

    /// <summary>
    /// Log an error with full context and stack trace
    /// </summary>
    public async Task LogErrorAsync(Exception ex, string context, Dictionary<string, object>? additionalData = null)
    {
        try
        {
            var logEntry = BuildLogEntry(ex, context, additionalData);
            
            // Debug output
            Debug.WriteLine($"❌ ERROR [{context}]: {ex.Message}");
            
            // Write to file
            await WriteToLogFileAsync(logEntry);
        }
        catch (Exception logEx)
        {
            // Fallback if logging fails
            Debug.WriteLine($"❌ CRITICAL: Failed to log error: {logEx.Message}");
        }
    }

    /// <summary>
    /// Log a warning message
    /// </summary>
    public async Task LogWarningAsync(string message, string context, Dictionary<string, object>? additionalData = null)
    {
        try
        {
            var logEntry = BuildWarningEntry(message, context, additionalData);
            Debug.WriteLine($"⚠️ WARNING [{context}]: {message}");
            await WriteToLogFileAsync(logEntry);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Failed to log warning: {ex.Message}");
        }
    }

    /// <summary>
    /// Log informational message
    /// </summary>
    public async Task LogInfoAsync(string message, string context)
    {
        try
        {
            var logEntry = $"[INFO] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} [{context}] {message}\n";
            Debug.WriteLine($"ℹ️ INFO [{context}]: {message}");
            await WriteToLogFileAsync(logEntry);
        }
        catch
        {
            // Silent fail for info logs
        }
    }

    /// <summary>
    /// Get user-friendly error message based on exception type
    /// </summary>
    public string GetUserFriendlyMessage(Exception ex)
    {
        return ex switch
        {
            InvalidOperationException => "An operation could not be completed. Please try again.",
            UnauthorizedAccessException => "Access denied. Please check your permissions.",
            System.IO.IOException => "File operation failed. Please ensure the app has proper permissions.",
            SQLite.SQLiteException => "Database error occurred. If this persists, try restarting the app.",
            ArgumentException => "Invalid input provided. Please check your entry and try again.",
            TimeoutException => "Operation timed out. Please try again.",
            _ => "An unexpected error occurred. Please try again or restart the app."
        };
    }

    /// <summary>
    /// Get recent log entries for diagnostics
    /// </summary>
    public async Task<string> GetRecentLogsAsync(int lineCount = 50)
    {
        await _writeLock.WaitAsync();
        try
        {
            if (!File.Exists(_logFilePath))
                return "No log entries found.";

            var lines = await File.ReadAllLinesAsync(_logFilePath);
            var recentLines = lines.TakeLast(lineCount);
            return string.Join("\n", recentLines);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Failed to read logs: {ex.Message}");
            return $"Error reading logs: {ex.Message}";
        }
        finally
        {
            _writeLock.Release();
        }
    }

    /// <summary>
    /// Clear old log entries
    /// </summary>
    public async Task ClearOldLogsAsync()
    {
        await _writeLock.WaitAsync();
        try
        {
            if (File.Exists(_logFilePath))
            {
                var fileInfo = new FileInfo(_logFilePath);
                if (fileInfo.Length > MaxLogFileSizeBytes)
                {
                    // Keep only last 1000 lines
                    var lines = await File.ReadAllLinesAsync(_logFilePath);
                    var recentLines = lines.TakeLast(1000);
                    await File.WriteAllLinesAsync(_logFilePath, recentLines);
                    Debug.WriteLine("🧹 Old logs cleared");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Failed to clear logs: {ex.Message}");
        }
        finally
        {
            _writeLock.Release();
        }
    }

    private string BuildLogEntry(Exception ex, string context, Dictionary<string, object>? additionalData)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[ERROR] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} UTC");
        sb.AppendLine($"Context: {context}");
        sb.AppendLine($"Exception Type: {ex.GetType().FullName}");
        sb.AppendLine($"Message: {ex.Message}");
        
        if (ex.InnerException != null)
        {
            sb.AppendLine($"Inner Exception: {ex.InnerException.Message}");
        }
        
        sb.AppendLine($"Stack Trace: {ex.StackTrace}");
        
        if (additionalData != null && additionalData.Count > 0)
        {
            sb.AppendLine("Additional Data:");
            foreach (var kvp in additionalData)
            {
                sb.AppendLine($"  {kvp.Key}: {kvp.Value}");
            }
        }
        
        sb.AppendLine($"Device: {DeviceInfo.Platform} {DeviceInfo.VersionString}");
        sb.AppendLine($"App Version: {AppInfo.VersionString}");
        sb.AppendLine(new string('-', 80));
        
        return sb.ToString();
    }

    private string BuildWarningEntry(string message, string context, Dictionary<string, object>? additionalData)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[WARNING] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} [{context}]");
        sb.AppendLine($"Message: {message}");
        
        if (additionalData != null && additionalData.Count > 0)
        {
            sb.AppendLine("Additional Data:");
            foreach (var kvp in additionalData)
            {
                sb.AppendLine($"  {kvp.Key}: {kvp.Value}");
            }
        }
        
        sb.AppendLine(new string('-', 40));
        return sb.ToString();
    }

    private async Task WriteToLogFileAsync(string entry)
    {
        await _writeLock.WaitAsync();
        try
        {
            await File.AppendAllTextAsync(_logFilePath, entry);
        }
        finally
        {
            _writeLock.Release();
        }
    }
}
