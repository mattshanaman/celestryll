using Microsoft.Maui.Storage;

namespace AmbientSleeper.Services;

public class SettingsService : ISettingsService
{
    private const string Prefix = "ambient_sleeper_";

    public string? LastSoundFile
    {
        get => Preferences.Get(Prefix + nameof(LastSoundFile), null);
        set { if (value is null) Preferences.Remove(Prefix + nameof(LastSoundFile)); else Preferences.Set(Prefix + nameof(LastSoundFile), value); }
    }

    public bool LoopEnabled
    {
        get => Preferences.Get(Prefix + nameof(LoopEnabled), true);
        set => Preferences.Set(Prefix + nameof(LoopEnabled), value);
    }

    public bool UseDuration
    {
        get => Preferences.Get(Prefix + nameof(UseDuration), true);
        set => Preferences.Set(Prefix + nameof(UseDuration), value);
    }

    public TimeSpan Duration
    {
        get => TimeSpan.FromSeconds(Preferences.Get(Prefix + nameof(Duration), (long)TimeSpan.FromHours(2).TotalSeconds));
        set => Preferences.Set(Prefix + nameof(Duration), (long)value.TotalSeconds);
    }

    public TimeSpan StopAtTime
    {
        get => TimeSpan.FromSeconds(Preferences.Get(Prefix + nameof(StopAtTime), (long)new TimeSpan(23, 0, 0).TotalSeconds));
        set => Preferences.Set(Prefix + nameof(StopAtTime), (long)value.TotalSeconds);
    }
}