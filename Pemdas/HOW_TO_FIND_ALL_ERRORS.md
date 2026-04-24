# ?? How to Find and Capture ALL Error Output

## Problem
Cannot find error output or need to capture all errors for troubleshooting.

---

## ? Method 1: Visual Studio Debug Output Window (PRIMARY)

### Step 1: Open Output Window
```
View ? Output (or Ctrl+Alt+O)
```

### Step 2: Select Correct Output Source
In the "Show output from:" dropdown, select:
- **"Debug"** - Shows Debug.WriteLine output (our diagnostics)
- **"Build"** - Shows compilation errors
- **"Package Manager"** - Shows NuGet errors

### Step 3: Filter for Our Messages
Look for these emoji markers in the output:
```
?? = Diagnostic info
?? = Target/lookup
? = Success
? = Error/failure
?? = Warning
?? = Retry/recovery
?? = Statistics
?? = Date-related
?? = Startup
```

### Step 4: Copy ALL Debug Output
1. Right-click in Output window
2. Select "Select All" (Ctrl+A)
3. Copy (Ctrl+C)
4. Paste into a text file

---

## ? Method 2: Check Build Errors

### In Visual Studio:
```
View ? Error List (or Ctrl+\, E)
```

You'll see three tabs:
- **Errors** (red) - Must fix to build
- **Warnings** (yellow) - Should review
- **Messages** (blue) - Informational

### Export Error List:
1. Right-click in Error List
2. "Copy All"
3. Paste into text file

---

## ? Method 3: Check Application Logs (Runtime)

### Windows:
```
Location: %LOCALAPPDATA%\Packages\{YourAppId}\LocalState\
Look for: *.log files
```

### Android:
```
Tool: Android Logcat
View ? Other Windows ? Device Log (Android)

Or use ADB:
adb logcat *:E
```

### iOS:
```
Tool: iOS Console
Devices ? iOS Device ? Console
```

---

## ? Method 4: Add Comprehensive Logging

Let me add a logging service to capture EVERYTHING:

### Create LoggingService.cs

```csharp
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
            return "No logs found.";
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
```

---

## ? Method 5: Add Error Viewer Page

### Create ErrorViewerPage.xaml

```xaml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="Pemdas.Pages.ErrorViewerPage"
             Title="Debug Logs">
    
    <Grid RowDefinitions="Auto,*,Auto" Padding="10">
        <Label Grid.Row="0" 
               Text="Debug Output (tap to copy)"
               FontAttributes="Bold"
               Margin="0,0,0,10"/>
        
        <ScrollView Grid.Row="1">
            <Label x:Name="LogsLabel" 
                   Text="{Binding LogsText}"
                   FontFamily="Courier New"
                   FontSize="10"
                   LineBreakMode="WordWrap">
                <Label.GestureRecognizers>
                    <TapGestureRecognizer Command="{Binding CopyLogsCommand}"/>
                </Label.GestureRecognizers>
            </Label>
        </ScrollView>
        
        <Grid Grid.Row="2" ColumnDefinitions="*,*,*" ColumnSpacing="10" Margin="0,10,0,0">
            <Button Grid.Column="0" 
                    Text="Refresh" 
                    Command="{Binding RefreshLogsCommand}"/>
            <Button Grid.Column="1" 
                    Text="Copy" 
                    Command="{Binding CopyLogsCommand}"/>
            <Button Grid.Column="2" 
                    Text="Clear" 
                    Command="{Binding ClearLogsCommand}"/>
        </Grid>
    </Grid>
</ContentPage>
```

### Create ErrorViewerViewModel.cs

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pemdas.Services;

namespace Pemdas.ViewModels;

public partial class ErrorViewerViewModel : BaseViewModel
{
    [ObservableProperty]
    private string logsText = "Loading logs...";

    public ErrorViewerViewModel()
    {
        Title = "Debug Logs";
        _ = LoadLogs();
    }

    [RelayCommand]
    private async Task LoadLogs()
    {
        IsBusy = true;
        try
        {
            LogsText = await LoggingService.GetAllLogs();
            if (string.IsNullOrWhiteSpace(LogsText))
            {
                LogsText = "No logs available yet. Use the app to generate logs.";
            }
        }
        catch (Exception ex)
        {
            LogsText = $"Error loading logs: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RefreshLogs()
    {
        await LoadLogs();
        await Application.Current.MainPage.DisplayAlert(
            "Refreshed", 
            "Logs reloaded", 
            "OK");
    }

    [RelayCommand]
    private async Task CopyLogs()
    {
        try
        {
            await Clipboard.SetTextAsync(LogsText);
            await Application.Current.MainPage.DisplayAlert(
                "Copied", 
                "Logs copied to clipboard", 
                "OK");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Error", 
                $"Failed to copy: {ex.Message}", 
                "OK");
        }
    }

    [RelayCommand]
    private async Task ClearLogs()
    {
        var confirm = await Application.Current.MainPage.DisplayAlert(
            "Clear Logs",
            "Delete all log entries?",
            "Yes",
            "No");

        if (confirm)
        {
            await LoggingService.ClearLogs();
            await LoadLogs();
        }
    }

    [RelayCommand]
    private async Task ShareLogs()
    {
        try
        {
            var logPath = LoggingService.GetLogFilePath();
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Share Debug Logs",
                File = new ShareFile(logPath)
            });
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                $"Failed to share: {ex.Message}",
                "OK");
        }
    }
}
```

---

## ? Method 6: Quick Diagnostic Command

### Add to ProfilePage or GamePage

Add this button temporarily:

```xaml
<Button Text="?? View Debug Logs"
        Command="{Binding ViewLogsCommand}"
        BackgroundColor="Orange"/>
```

Add to ViewModel:

```csharp
[RelayCommand]
private async Task ViewLogs()
{
    await Shell.Current.GoToAsync("errorviewer");
}
```

Register route in AppShell.xaml.cs:

```csharp
Routing.RegisterRoute("errorviewer", typeof(ErrorViewerPage));
```

---

## ? Method 7: Capture Startup Errors

### Update App.xaml.cs

```csharp
public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Capture unhandled exceptions
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

        MainPage = new AppShell();
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            var error = $"UNHANDLED EXCEPTION:\n{ex.Message}\n{ex.StackTrace}";
            System.Diagnostics.Debug.WriteLine($"??? {error}");
            _ = LoggingService.LogError(error, ex);
        }
    }

    private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        var error = $"UNOBSERVED TASK EXCEPTION:\n{e.Exception.Message}\n{e.Exception.StackTrace}";
        System.Diagnostics.Debug.WriteLine($"??? {error}");
        _ = LoggingService.LogError(error, e.Exception);
        e.SetObserved();
    }
}
```

---

## ? Method 8: Get Database Diagnostics

### Add to ProfilePage (Temporary Debug Button)

```xaml
<Button Text="?? Database Info"
        Command="{Binding ShowDatabaseInfoCommand}"
        BackgroundColor="Blue"
        TextColor="White"/>
```

```csharp
[RelayCommand]
private async Task ShowDatabaseInfo()
{
    var integrity = await _databaseService.VerifyDatabaseIntegrity();
    
    // This triggers the diagnostic output in Debug window
    await Application.Current.MainPage.DisplayAlert(
        "Database Check",
        "Check Debug Output window for detailed diagnostics",
        "OK");
}
```

---

## ?? Quick Checklist: Where to Find Errors

Use this checklist to find ALL errors:

### Before Running:
- [ ] Check Error List (Ctrl+\, E) for build errors
- [ ] Check Output window ? Build for compilation issues
- [ ] Clear all previous output (right-click ? Clear All)

### While Running:
- [ ] Output window ? Debug (watch for ? emoji)
- [ ] Output window ? Package Manager (for NuGet issues)
- [ ] Device Log (for Android logcat)
- [ ] iOS Console (for iOS logs)

### After Error Occurs:
- [ ] Copy full Debug output (Ctrl+A, Ctrl+C)
- [ ] Check Error List for runtime errors
- [ ] Check log file at: `FileSystem.AppDataDirectory/pemdas_debug.log`
- [ ] Look for exception stack traces
- [ ] Note exact error message text

---

## ?? What to Look For

### Critical Errors (Must Fix):
```
? PUZZLE IS NULL
? CRITICAL: No puzzles for today
? Error initializing game
? Exception: [anything]
? UNHANDLED EXCEPTION
```

### Important Warnings:
```
?? Incomplete database
?? Today is OUTSIDE the puzzle date range
?? No puzzles found
?? Database integrity check failed
```

### Expected Messages (OK):
```
? Generated 43800 total puzzles
? Found puzzle: Easy - SolveIt
? Database Check: 43800 puzzles found
?? Today's puzzles: 8/8 available
```

---

## ?? Quick Action: Get Full Diagnostic Now

Run this in Immediate Window (Debug ? Windows ? Immediate):

```csharp
await Task.Run(async () => {
    var db = new DatabaseService();
    await db.Init();
    var result = await db.VerifyDatabaseIntegrity();
    return result;
});
```

Or add this temporary method to GameViewModel:

```csharp
[RelayCommand]
private async Task DumpFullDiagnostics()
{
    var sb = new StringBuilder();
    sb.AppendLine("=== FULL DIAGNOSTIC DUMP ===");
    sb.AppendLine($"Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
    sb.AppendLine();
    
    // Database
    await _databaseService.VerifyDatabaseIntegrity();
    
    // Current puzzle
    sb.AppendLine($"Current Puzzle: {_currentPuzzle?.Difficulty} - {_currentPuzzle?.Mode}");
    sb.AppendLine($"Has Error: {HasError}");
    sb.AppendLine($"Error Message: {ErrorMessage}");
    sb.AppendLine($"Is Busy: {IsBusy}");
    
    // User progress
    var progress = await _databaseService.GetUserProgress();
    sb.AppendLine($"Streak: {progress?.CurrentStreak}");
    sb.AppendLine($"Preferred Difficulty: {progress?.PreferredDifficultySlot}");
    
    System.Diagnostics.Debug.WriteLine(sb.ToString());
    
    await Application.Current.MainPage.DisplayAlert(
        "Diagnostics Complete",
        "Check Debug Output window for full report",
        "OK");
}
```

---

## ?? Pro Tips

### Tip 1: Use Breakpoints
Set breakpoints in:
- `GameViewModel.InitializeAsync()` line 206
- `DatabaseService.GetTodaysPuzzle()` line 104
- `DatabaseService.EnsurePuzzlesExistAsync()` line 95

### Tip 2: Watch Window
Add these to Watch window during debugging:
- `puzzle`
- `_currentPuzzle`
- `HasError`
- `ErrorMessage`

### Tip 3: Conditional Breakpoint
Set condition: `puzzle == null` on the line that checks puzzle

### Tip 4: Log Everything
Replace all `System.Diagnostics.Debug.WriteLine` with `LoggingService.LogInfo`

---

## ?? How to Share Errors With Me

1. **Copy Debug Output:**
   - View ? Output ? Debug
   - Ctrl+A, Ctrl+C
   - Paste into text file

2. **Copy Error List:**
   - View ? Error List
   - Right-click ? Copy All
   - Paste into text file

3. **Get Log File:**
   - Find: `FileSystem.AppDataDirectory/pemdas_debug.log`
   - Copy contents

4. **Share:**
   - Paste all three into your next message
   - Include what you were doing when error occurred

---

## ?? Expected vs. Actual

When you share output, include:

**What I Expected:**
```
? Database Check: 43800 puzzles found
? Found puzzle: Easy - SolveIt
```

**What I Actually Got:**
```
[Paste your actual error output here]
```

---

**Ready to find those errors!** ??

Please run the app, copy the full Debug output, and share it with me!
