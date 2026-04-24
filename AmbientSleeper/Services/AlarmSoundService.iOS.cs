#if IOS
using AVFoundation;
using AudioToolbox;
using Foundation;
using Microsoft.Maui.Storage; // ADDED for FileSystem

namespace AmbientSleeper.Services;

public class AlarmSoundService : IAlarmSoundService
{
    private AVAudioPlayer? _player;

    // Expose Apple system sounds 1020..1036
    private static readonly (int Id, string Name)[] SystemSounds =
        Enumerable.Range(1020, 17)
                    .Select(id => (Id: id, Name: $"Apple Sound {id}"))
                    .ToArray();

    public async Task<string> PickSystemSoundAsync()
    {
        var options = SystemSounds.Select(s => s.Name).ToArray();

        var selected = await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var page = Application.Current?.MainPage;
            if (page == null) return string.Empty;
            var res = await page.DisplayActionSheet(
                "Select Alarm Sound",
                "Cancel",
                null,
                options);
            return res ?? string.Empty;
        });

        if (string.IsNullOrEmpty(selected) || selected == "Cancel")
            return string.Empty;

        var match = SystemSounds.FirstOrDefault(s => s.Name == selected);
        if (match.Id == 0)
            return string.Empty;

        // Return a token recognized by PlayAlarm
        return $"sys:{match.Id}";
    }

    public void PlayAlarm(string uriOrName, float volume = 1.0f, bool loop = false)
    {
        StopAlarm();

        // Use a default system alert if not set or explicitly "default"
        if (string.IsNullOrWhiteSpace(uriOrName) ||
            string.Equals(uriOrName, "default", StringComparison.OrdinalIgnoreCase))
        {
            new SystemSound(1005).PlayAlertSound(); // Default fallback
            return;
        }

        // Handle system sound token "sys:<id>"
        if (uriOrName.StartsWith("sys:", StringComparison.OrdinalIgnoreCase))
        {
            if (uint.TryParse(uriOrName.AsSpan(4), out var systemId))
            {
                new SystemSound(systemId).PlayAlertSound();
                return;
            }
        }

        try
        {
            var resolved = ResolvePath(uriOrName);
            if (string.IsNullOrWhiteSpace(resolved) || !File.Exists(resolved))
            {
                new SystemSound(1005).PlayAlertSound();
                return;
            }
            var url = NSUrl.FromFilename(resolved);
            var p = AVAudioPlayer.FromUrl(url);
            if (p == null)
            {
                new SystemSound(1005).PlayAlertSound();
                return;
            }

            _player = p;
            p.Volume = Math.Clamp(volume, 0f, 1f);
            p.NumberOfLoops = loop ? -1 : 0;
            p.PrepareToPlay();
            p.Play();
        }
        catch
        {
            // Fallback to default alert if playback fails
            new SystemSound(1005).PlayAlertSound();
        }
    }

    public void StopAlarm()
    {
        var p = _player;
        if (p == null) return;

        try 
        {
            if (p.Playing) p.Stop();
            p.Dispose();
        } catch {}
        finally
        {
            _player = null;
        }
    }

    private static string ResolvePath(string name)
    {
        try
        {
            var localPath = Path.Combine(FileSystem.AppDataDirectory, name);
            if (File.Exists(localPath)) return localPath;

            using var s = FileSystem.OpenAppPackageFileAsync(name).Result;
            var dir = Path.GetDirectoryName(localPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            using var fs = File.Create(localPath);
            s.CopyTo(fs);
            return localPath;
        }
        catch
        {
            return string.Empty;
        }
    }
}
#endif