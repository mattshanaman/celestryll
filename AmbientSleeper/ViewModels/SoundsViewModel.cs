using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AmbientSleeper.Models;
using AmbientSleeper.Services;

namespace AmbientSleeper.ViewModels;

public partial class SoundsViewModel : ObservableObject
{
    private readonly IAudioService _audio;
    private readonly ISettingsService _settings;

    public ObservableCollection<SoundItem> Sounds { get; } = new()
    {
        new SoundItem { FileName = "fan01.mp3",          DisplayName = "Fan — Low" },
        new SoundItem { FileName = "fan02.mp3",          DisplayName = "Fan — High" },
        new SoundItem { FileName = "water_stream01.mp3", DisplayName = "Running Water" },
        new SoundItem { FileName = "thundershower01.mp3",DisplayName = "Thundershower" },
    };

    [ObservableProperty] private SoundItem? selectedSound;
    partial void OnSelectedSoundChanged(SoundItem? value)
    {
        if (value is not null)
            _settings.LastSoundFile = value.FileName;
        OnPropertyChanged(nameof(NowPlayingText));
    }

    [ObservableProperty] private bool loopEnabled;
    partial void OnLoopEnabledChanged(bool value)
    {
        _settings.LoopEnabled = value;
        // If currently playing, update loop on the fly
        try
        {
            _audio.Stop();
            if (SelectedSound is not null)
            {
                _ = _audio.LoadAsync(SelectedSound.FileName);
                _ = _audio.PlayAsync(loop: value);
            }
        }
        catch { /* ignore */ }
        OnPropertyChanged(nameof(NowPlayingText));
        OnPropertyChanged(nameof(PlayPauseText));
    }

    public string NowPlayingText => _audio.IsPlaying && SelectedSound != null
        ? $"Now playing: {SelectedSound.DisplayName}"
        : "Not playing";

    public string PlayPauseText => _audio.IsPlaying ? "Pause" : "Play";

    public SoundsViewModel(IAudioService audio, ISettingsService settings)
    {
        _audio = audio;
        _settings = settings;

        // Restore preferences
        loopEnabled = _settings.LoopEnabled;

        var last = _settings.LastSoundFile;
        if (last is not null)
            SelectedSound = Sounds.FirstOrDefault(s => s.FileName == last) ?? Sounds.First();

        // Fallback default
        SelectedSound ??= Sounds.First();
    }

    [RelayCommand]
    private async Task PlayPauseAsync()
    {
        if (SelectedSound is null)
        {
            await Shell.Current.DisplayAlert("Select a sound", "Please choose a sound to play.", "OK");
            return;
        }

        if (_audio.IsPlaying)
        {
            _audio.Pause();
        }
        else
        {
            await _audio.LoadAsync(SelectedSound.FileName);
            await _audio.PlayAsync(loop: LoopEnabled);
        }
        OnPropertyChanged(nameof(PlayPauseText));
        OnPropertyChanged(nameof(NowPlayingText));
    }

    [RelayCommand]
    private void Stop()
    {
        _audio.Stop();
        OnPropertyChanged(nameof(PlayPauseText));
        OnPropertyChanged(nameof(NowPlayingText));
    }
}
