using System.Text;

namespace Pemdas.Services;

public class LoggingService
{
    private static readonly string LogFilePath = Path.Combine(
        FileSystem.AppDataDirectory, 
        "pemdas_debug.log");
    
    private static readonly SemaphoreSlim LogLock = new(1, 1);

    public static async Task LogInfo(string message)
    {
        await LogMessage("INFO", message);
    }

    public static async Task LogError(string message, Exception? ex = null)
    {
        var fullMessage = ex != null 
            ? $"{message}\nException: {ex.Message}\nStack: {ex.StackTrace}"
            : message;
        await LogMessage("ERROR", fullMessage);
    }

    public static async Task LogWarning(string message)
    {
        await LogMessage("WARN", message);
    }

    public static async Task LogDebug(string message)
    {
        await LogMessage("DEBUG", message);
    }

    private static async Task LogMessage(string level, string message)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var logEntry = $"[{timestamp}] [{level}] {message}\n";

        // Write to Debug output
        System.Diagnostics.Debug.WriteLine(logEntry);

        // Write to file
        await LogLock.WaitAsync();
        try
        {
            await File.AppendAllTextAsync(LogFilePath, logEntry);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to write log: {ex.Message}");
        }
        finally
        {
            LogLock.Release();
        }
    }

    public static async Task<string> GetAllLogs()
    {
        await LogLock.WaitAsync();
        try
        {
            if (File.Exists(LogFilePath))
            {
                return await File.ReadAllTextAsync(LogFilePath);
            }
            return "No logs found. Use the app to generate logs.";
        }
        finally
        {
            LogLock.Release();
        }
    }

    public static async Task ClearLogs()
    {
        await LogLock.WaitAsync();
        try
        {
            if (File.Exists(LogFilePath))
            {
                File.Delete(LogFilePath);
            }
        }
        finally
        {
            LogLock.Release();
        }
    }

    public static string GetLogFilePath() => LogFilePath;
}
