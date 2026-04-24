using AmbientSleeper.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AmbientSleeper.ViewModels;

public partial class PlaybackSettingsViewModel : ObservableObject
{
    private readonly FeatureGate _features;
    private readonly PlaybackViewModel _playback;

    public PlaybackSettingsViewModel(FeatureGate features, PlaybackViewModel playback)
    {
        _features = features;
        _playback = playback;

        // initialize from persisted state
        FadeOutSeconds = UserPreferences.FadeOutSeconds;
        AlarmEnabled = _playback.AlarmEnabled;
        SelectedAlarm = _playback.SelectedAlarm;
    }

    public int MaxFadeSeconds => _features.MaxFadeSeconds;

    [ObservableProperty]
    private int fadeOutSeconds;

    partial void OnFadeOutSecondsChanged(int value)
    {
        var clamped = Math.Clamp(value, 0, MaxFadeSeconds);
        if (clamped != value)
        {
            FadeOutSeconds = clamped;
            return;
        }
        UserPreferences.FadeOutSeconds = clamped;
    }

    [ObservableProperty]
    private bool alarmEnabled;

    partial void OnAlarmEnabledChanged(bool value)
    {
        _playback.AlarmEnabled = value;
        UserPreferences.AlarmEnabledPref = value;
    }

    [ObservableProperty]
    private string? selectedAlarm;

    public bool IsAlarmIntegrationEnabled => _features.AlarmIntegrationEnabled;

    [RelayCommand]
    public async Task PickAlarmAsync()
    {
        if (!IsAlarmIntegrationEnabled) return;
        await _playback.PickAlarmAsync();
        SelectedAlarm = _playback.SelectedAlarm;
    }
}