using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AmbientSleeper.Models;
using AmbientSleeper.Services;

namespace AmbientSleeper.ViewModels;

public class MainViewModel
{
    public PlaybackViewModel Playback { get; }
    public TimerViewModel Timer { get; }

    public MainViewModel(PlaybackViewModel playback, TimerViewModel timer)
    {
        Playback = playback;
        Timer = timer;
    }
}

