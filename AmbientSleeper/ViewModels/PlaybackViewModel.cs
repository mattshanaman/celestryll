using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AmbientSleeper.Models;
using AmbientSleeper.Services;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel; // MainThread
using Microsoft.Maui.Storage;
using AmbientSleeper.Resources.Strings;
using Microsoft.Extensions.DependencyInjection;

namespace AmbientSleeper.ViewModels;

public partial class PlaybackViewModel : ObservableObject
{
    private readonly IPlaybackOrchestrator _orchestrator;
    private readonly IAlarmSoundService _alarm;
    private readonly TimerViewModel _timer;
    private readonly FeatureGate _features;
    private readonly IMinimizedUiService _minimizedUi;

    private CancellationTokenSource? _sessionCts;

    public ObservableCollection<AudioItem> MixSelection { get; } = new();
    public ObservableCollection<AudioItem> PlaylistQueue { get; } = new();
    public ObservableCollection<SavedPlaylist> Playlists { get; } = new();
    public ObservableCollection<SavedMix> Mixes { get; } = new();

    // NEW: Mix Playlist
    public ObservableCollection<MixPlaylistEntry> MixPlaylistQueue { get; } = new();
    public ObservableCollection<SavedMixPlaylist> MixPlaylists { get; } = new();

    [ObservableProperty] private bool isPlaying;
    [ObservableProperty] private bool playlistLoopEnabled = true;
    [ObservableProperty] private bool mixPlaylistLoopEnabled = true;

    [ObservableProperty] private bool alarmEnabled = true;
    [ObservableProperty] private string? selectedAlarm; // Android: URI; iOS: bundled alarm name
    [ObservableProperty] private int selectedTabIndex = 0;

    [ObservableProperty] private bool isEqEnabled;
    [ObservableProperty] private bool isAdvancedEqEnabled;

    // Expose feature flags for binding
    [ObservableProperty] private bool isPlaylistEnabled;
    [ObservableProperty] private bool isAlarmIntegrationEnabled;
    [ObservableProperty] private int maxOverlaySounds;
    [ObservableProperty] private int maxSavedMixes;

    [ObservableProperty] private bool isMixPlaylistEnabled;
    [ObservableProperty] private bool isMixPlaylistTransitionsEnabled;
    [ObservableProperty] private int maxSavedMixPlaylists;
    [ObservableProperty] private int maxMixesPerMixPlaylist;

    // Bindable fade-out seconds for Stop All button text
    [ObservableProperty] private int fadeOutSeconds;

    // computed flag for Save Mix button enablement
    public bool CanSaveMix
        => MixSelection.Count > 0 && UserPreferences.GetMixes().Count < MaxSavedMixes;

    // expose tier cap so UI can react if needed
    public int MaxSavedPlaylists => _features.MaxSavedPlaylists; 

    // computed flag for enabling Save Current Playlist button
    public bool CanSavePlaylist                                          
        => IsPlaylistEnabled                                             
           && PlaylistQueue.Count > 0                                   
           && UserPreferences.GetPlaylists().Count < MaxSavedPlaylists;  

    // computed flag for enabling Save Mix Playlist button
    public bool CanSaveMixPlaylist
        => IsMixPlaylistEnabled
           && MixPlaylistQueue.Count > 0
           && UserPreferences.GetMixPlaylists().Count < MaxSavedMixPlaylists;


    public PlaybackViewModel(IPlaybackOrchestrator orchestrator, IAlarmSoundService alarm,
                            TimerViewModel timer, FeatureGate features)
    {
        _orchestrator = orchestrator;
        _alarm = alarm;
        _timer = timer;
        _features = features;
        
        // Guard against null service provider
        var services = ServiceHost.Services;
        if (services == null)
            throw new InvalidOperationException("ServiceHost not initialized");
        _minimizedUi = services.GetRequiredService<IMinimizedUiService>();

        // Load persisted alarm enabled state and last alarm
        AlarmEnabled = UserPreferences.AlarmEnabledPref;
        selectedAlarm = UserPreferences.LastAlarm;

        // Initialize fade-out label using persisted value, clamped by tier
        FadeOutSeconds = Math.Clamp(UserPreferences.FadeOutSeconds, 0, _features.MaxFadeSeconds);

        ApplyFeatureGate();
        _features.Changed += (_, __) =>
        {
            ApplyFeatureGate();
            FadeOutSeconds = Math.Clamp(UserPreferences.FadeOutSeconds, 0, _features.MaxFadeSeconds);
            OnPropertyChanged(nameof(MaxSavedPlaylists));
            OnPropertyChanged(nameof(CanSaveMix));
            OnPropertyChanged(nameof(CanSaveMixPlaylist));
            _sessionCts?.Cancel();
        };

        MixSelection.CollectionChanged += (_, __) => OnPropertyChanged(nameof(CanSaveMix));
        MixPlaylistQueue.CollectionChanged += (_, __) => OnPropertyChanged(nameof(CanSaveMixPlaylist));

        RefreshPlaylists();
        RefreshMixes();
        RefreshMixPlaylists();

        _features.Changed += (_, __) => _sessionCts?.Cancel();
    }

    private void ApplyFeatureGate()
    {
        IsPlaylistEnabled = _features.PlaylistEnabled;
        IsAlarmIntegrationEnabled = _features.AlarmIntegrationEnabled;
        MaxOverlaySounds = _features.MaxOverlaySounds;
        MaxSavedMixes = _features.MaxSavedMixes;

        IsEqEnabled = _features.CustomEqEnabled;           // Premium/Pro+
        IsAdvancedEqEnabled = _features.AdvancedEditorEnabled; // Pro+

        // Mix playlists
        IsMixPlaylistEnabled = _features.MixPlaylistEnabled;
        IsMixPlaylistTransitionsEnabled = _features.MixPlaylistTransitionsEnabled;
        MaxSavedMixPlaylists = _features.MaxSavedMixPlaylists;
        MaxMixesPerMixPlaylist = _features.MaxMixesPerMixPlaylist;

        OnPropertyChanged(nameof(IsPlaylistEnabled));
        OnPropertyChanged(nameof(IsAlarmIntegrationEnabled));
        OnPropertyChanged(nameof(MaxOverlaySounds));
        OnPropertyChanged(nameof(MaxSavedMixes));
        OnPropertyChanged(nameof(IsEqEnabled));
        OnPropertyChanged(nameof(IsAdvancedEqEnabled));

        OnPropertyChanged(nameof(CanSaveMix));
        OnPropertyChanged(nameof(MaxSavedPlaylists));
        OnPropertyChanged(nameof(CanSavePlaylist));

        OnPropertyChanged(nameof(IsMixPlaylistEnabled));
        OnPropertyChanged(nameof(IsMixPlaylistTransitionsEnabled));
        OnPropertyChanged(nameof(MaxSavedMixPlaylists));
        OnPropertyChanged(nameof(MaxMixesPerMixPlaylist));
        OnPropertyChanged(nameof(CanSaveMixPlaylist));
    }

    [RelayCommand]
    public void ChangeTab(string tabIndex)
    {
        if (int.TryParse(tabIndex, out var index))
        {
            SelectedTabIndex = index;
        }
    }

    // Library hooks


    public void AddToMix(AudioItem item)
    {
        // Enforce overlay limit by tier
        if (MixSelection.Count >= MaxOverlaySounds)
            return;

        if (!MixSelection.Any(x => x.Id == item.Id))
        {
            // Load saved volume if available
            var savedVolumes = UserPreferences.GetMixVolumes();
            if (savedVolumes.TryGetValue(item.Id, out var savedVolume))
                item.Volume = savedVolume;
            MixSelection.Add(Clone(item));
        }
    }

    public void AddToPlaylist(AudioItem item)
    {
        if (!PlaylistQueue.Any(x => x.Id == item.Id))
            PlaylistQueue.Add(Clone(item));
    }

    // Mix Playlist ops
    public void AddMixToMixPlaylist(SavedMix mix)
    {
        if (!IsMixPlaylistEnabled) return;
        if (MixPlaylistQueue.Count >= MaxMixesPerMixPlaylist) return;
        MixPlaylistQueue.Add(new MixPlaylistEntry { Mix = mix });
    }

    [RelayCommand]
    public void RemoveFromMixPlaylist(MixPlaylistEntry entry) => _ = MixPlaylistQueue.Remove(entry);

    [RelayCommand]
    public void RemoveFromMix(AudioItem item) => _ = MixSelection.Remove(item);

    [RelayCommand]
    public void RemoveFromPlaylist(AudioItem item) => _ = PlaylistQueue.Remove(item);

    [RelayCommand]
    public async Task StartMixAsync()
    {
        if (MixSelection.Count == 0) return;
        await _orchestrator.StartMixAsync(MixSelection);
        IsPlaying = true;

        // Start timer if enabled
        if (AlarmEnabled && (UserPreferences.UseDuration || UserPreferences.StopAtTime != TimeSpan.Zero))
        {
            await _timer.StartTimerAsync();
        }

        _minimizedUi.ShowOrUpdate("Ambient Sleeper", "Mix", IsPlaying);
        StartSessionLimitWatchIfNeeded();
    }

    [RelayCommand]
    public async Task StopMixAsync()
    {
        var fade = TimeSpan.FromSeconds(UserPreferences.FadeOutSeconds);
        await _orchestrator.StopAllWithFadeAsync(fade);
        IsPlaying = false;
        _timer.CancelTimer();
        _sessionCts?.Cancel();
        _sessionCts = null;
        _minimizedUi.Hide();

        // Save all current volumes
        var volumes = MixSelection.ToDictionary(x => x.Id, x => x.Volume);
        UserPreferences.SetMixVolumes(volumes);
    }

    [RelayCommand]
    public async Task UpdateMixVolumeAsync(MixVolumeUpdate update)
    {
        update.Item.Volume = update.Volume;
        await _orchestrator.UpdateMixVolumeAsync(update.Item.Id, update.Volume);

        // Save updated volume
        var volumes = UserPreferences.GetMixVolumes();
        volumes[update.Item.Id] = update.Volume;
        UserPreferences.SetMixVolumes(volumes);
    }

    //Helper class for volume updates 
    public class MixVolumeUpdate
    {
        public required AudioItem Item { get; set; }
        public double Volume { get; set; }
    }

    private void RefreshMixes()
    {
        Mixes.Clear();
        foreach (var m in UserPreferences.GetMixes())
            Mixes.Add(m);

        // refresh save availability after list reload
        OnPropertyChanged(nameof(CanSaveMix));
    }

    [RelayCommand]
    public void SaveCurrentMix(string mixName)
    {
        if (string.IsNullOrWhiteSpace(mixName) || MixSelection.Count == 0)
            return;

        // Enforce max saved mixes per tier
        var existing = UserPreferences.GetMixes();
        if (existing.Count >= MaxSavedMixes)
            return;

        // Duplicate name check
        if (existing.Any(m => m.Name.Equals(mixName, StringComparison.OrdinalIgnoreCase)))
            return;

        var mix = new SavedMix
        {
            Name = mixName,
            Items = MixSelection.Select(Clone).ToList(),
            SavedAt = DateTime.UtcNow
        };
        UserPreferences.SaveMix(mix);
        RefreshMixes();
        
        OnPropertyChanged(nameof(CanSaveMix));
    }

    [RelayCommand]
    public void LoadMix(string mixName)
    {
        var mix = UserPreferences.GetMixes().FirstOrDefault(m => m.Name == mixName);
        if (mix != null)
        {
            MixSelection.Clear();
            foreach (var item in mix.Items)
                MixSelection.Add(Clone(item));
        }
    }

    [RelayCommand]
    public void DeleteMix(string mixName)
    {
        UserPreferences.DeleteMix(mixName);
        RefreshMixes();
        OnPropertyChanged(nameof(CanSaveMix));
    }

    [RelayCommand]
    public async Task StartPlaylistAsync()
    {
        // Feature gating: Standard+ only
        if (!IsPlaylistEnabled)
        {
            var shell = Shell.Current;
            if (shell != null)
            {
                await shell.DisplayAlert(AppResources.PlaylistLocked_Title,
                    AppResources.PlaylistLocked_Message,
                    AppResources.Ok);
            }
            return;
        }

        // Nothing to play
        if (PlaylistQueue.Count == 0)
        {
            var shell = Shell.Current;
            if (shell != null)
            {
                await shell.DisplayAlert(AppResources.EmptyPlaylist_Title,
                    AppResources.EmptyPlaylist_Message,
                    AppResources.Ok);
            }
            return;
        }

        await _orchestrator.StartPlaylistAsync(PlaylistQueue, PlaylistLoopEnabled);
        IsPlaying = true;

        // Start timer if enabled
        if (AlarmEnabled && (UserPreferences.UseDuration || UserPreferences.StopAtTime != TimeSpan.Zero))
        {
            await _timer.StartTimerAsync();
        }

        _minimizedUi.ShowOrUpdate("Ambient Sleeper", "Playlist", IsPlaying);
        StartSessionLimitWatchIfNeeded();
    }

    [RelayCommand]
    public async Task StopPlaylistAsync()
    {
        var fade = TimeSpan.FromSeconds(UserPreferences.FadeOutSeconds);
        await _orchestrator.StopAllWithFadeAsync(fade);
        IsPlaying = false;
        _timer.CancelTimer();
        _sessionCts?.Cancel();
        _sessionCts = null;
        _minimizedUi.Hide();
    }
    private void RefreshPlaylists()
    {
        Playlists.Clear();
        foreach (var p in UserPreferences.GetPlaylists())
            Playlists.Add(p);

        OnPropertyChanged(nameof(CanSavePlaylist));
    }

    [RelayCommand]
    public void SaveCurrentPlaylist(string playlistName)
    {
        // strict gate — no-op on Free
        if (!IsPlaylistEnabled) return;

        if (string.IsNullOrWhiteSpace(playlistName) || PlaylistQueue.Count == 0)
            return;

        // block if cap reached
        if (UserPreferences.GetPlaylists().Count >= MaxSavedPlaylists)  
            return;

        // duplicate name check
        if (UserPreferences.GetPlaylists().Any(p => p.Name.Equals(playlistName, StringComparison.OrdinalIgnoreCase)))
            return;

        var playlist = new SavedPlaylist
        {
            Name = playlistName,
            Items = PlaylistQueue.Select(Clone).ToList(),
            SavedAt = DateTime.UtcNow
        };
        UserPreferences.SavePlaylist(playlist);
        RefreshPlaylists();

        OnPropertyChanged(nameof(CanSavePlaylist));
    }

    [RelayCommand]
    public void LoadPlaylist(string playlistName)
    {
        var playlist = UserPreferences.GetPlaylists().FirstOrDefault(p => p.Name == playlistName);
        if (playlist != null)
        {
            PlaylistQueue.Clear();
            foreach (var item in playlist.Items)
                PlaylistQueue.Add(Clone(item));
        }
    }

    [RelayCommand]
    public void DeletePlaylist(string playlistName)
    {
        UserPreferences.DeletePlaylist(playlistName);
        RefreshPlaylists();

        OnPropertyChanged(nameof(CanSavePlaylist));
    }

    // Mix Playlist helpers
    private void RefreshMixPlaylists()
    {
        MixPlaylists.Clear();
        foreach (var p in UserPreferences.GetMixPlaylists())
            MixPlaylists.Add(p);
        OnPropertyChanged(nameof(CanSaveMixPlaylist));
    }

    [RelayCommand]
    public void SaveCurrentMixPlaylist(string name)
    {
        if (!IsMixPlaylistEnabled) return;
        if (string.IsNullOrWhiteSpace(name) || MixPlaylistQueue.Count == 0) return;
        if (UserPreferences.GetMixPlaylists().Count >= MaxSavedMixPlaylists) return;

        // duplicate name check
        if (UserPreferences.GetMixPlaylists().Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            return;

        var list = new SavedMixPlaylist
        {
            Name = name,
            Entries = MixPlaylistQueue.Select(e => new MixPlaylistEntry
            {
                Mix = e.Mix,
                DurationSeconds = e.DurationSeconds,
                TransitionSeconds = Math.Clamp(e.TransitionSeconds, 0, _features.MaxMixTransitionSeconds)
            }).ToList(),
            Loop = MixPlaylistLoopEnabled,
            SavedAt = DateTime.UtcNow
        };
        UserPreferences.SaveMixPlaylist(list);
        RefreshMixPlaylists();
        OnPropertyChanged(nameof(CanSaveMixPlaylist));
    }

    [RelayCommand]
    public void LoadMixPlaylist(string name)
    {
        var list = UserPreferences.GetMixPlaylists().FirstOrDefault(p => p.Name == name);
        if (list == null) return;
        MixPlaylistQueue.Clear();
        foreach (var e in list.Entries)
            MixPlaylistQueue.Add(new MixPlaylistEntry { Mix = e.Mix, DurationSeconds = e.DurationSeconds, TransitionSeconds = e.TransitionSeconds });
        MixPlaylistLoopEnabled = list.Loop;
    }

    [RelayCommand]
    public void DeleteMixPlaylist(string name)
    {
        UserPreferences.DeleteMixPlaylist(name);
        RefreshMixPlaylists();
        OnPropertyChanged(nameof(CanSaveMixPlaylist));
    }

    [RelayCommand]
    public async Task StartMixPlaylistAsync()
    {
        if (!IsMixPlaylistEnabled)
        {
            var title = AppResources.ResourceManager.GetString("MixPlaylist_Locked_Title") ?? "Mix Playlist locked";
            var message = AppResources.ResourceManager.GetString("MixPlaylist_Locked_Message") ?? "Upgrade to Premium or Pro+ to use Mix Playlists.";
            var shell = Shell.Current;
            if (shell != null)
            {
                await shell.DisplayAlert(title, message, AppResources.Ok);
            }
            return;
        }
        if (MixPlaylistQueue.Count == 0) return;

        IsPlaying = true;

        // Start timer if enabled (so alarm fires on expiry)
        if (AlarmEnabled && (UserPreferences.UseDuration || UserPreferences.StopAtTime != TimeSpan.Zero))
        {
            await _timer.StartTimerAsync();
        }

        _minimizedUi.ShowOrUpdate("Ambient Sleeper", "Mix Playlist", IsPlaying);
        StartSessionLimitWatchIfNeeded();

        _ = Task.Run(async () =>
        {
            try
            {
                do
                {
                    foreach (var entry in MixPlaylistQueue.ToList())
                    {
                        var items = entry.Mix.Items.Select(Clone).ToList();
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await _orchestrator.StartMixAsync(items);
                        });

                        var runSeconds = entry.DurationSeconds ?? (int)TimeSpan.FromMinutes(3).TotalSeconds; // default 3m per mix
                        try
                        {
                            await Task.Delay(TimeSpan.FromSeconds(runSeconds), _sessionCts?.Token ?? CancellationToken.None);
                        }
                        catch { }

                        var transition = IsMixPlaylistTransitionsEnabled
                            ? TimeSpan.FromSeconds(Math.Clamp(entry.TransitionSeconds, 0, _features.MaxMixTransitionSeconds))
                            : TimeSpan.Zero;
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await _orchestrator.StopAllWithFadeAsync(transition);
                        });

                        if (_sessionCts?.IsCancellationRequested == true) break;
                    }
                }
                while (MixPlaylistLoopEnabled && !(_sessionCts?.IsCancellationRequested ?? false));
            }
            finally
            {
                if (!MixPlaylistLoopEnabled)
                {
                    await MainThread.InvokeOnMainThreadAsync(async () => await StopAllAsync());
                }
            }
        });
    }

    [RelayCommand]
    public async Task StopMixPlaylistAsync()
    {
        var fade = TimeSpan.FromSeconds(UserPreferences.FadeOutSeconds);
        await _orchestrator.StopAllWithFadeAsync(fade);
        IsPlaying = false;
        _timer.CancelTimer();
        _sessionCts?.Cancel();
        _sessionCts = null;
        _minimizedUi.Hide();
    }

    [RelayCommand]
    public async Task StopAllAsync()
    {
        var fade = TimeSpan.FromSeconds(UserPreferences.FadeOutSeconds);
        await _orchestrator.StopAllWithFadeAsync(fade);
        IsPlaying = false;
        _timer.CancelTimer();
        _sessionCts?.Cancel();
        _sessionCts = null;
        _minimizedUi.Hide();
    }

    [RelayCommand]
    public async Task PickAlarmAsync()
    {
        // Free can’t pick; show info and mark default
        if (!_features.AlarmIntegrationEnabled)
        {
            var shell = Shell.Current;
            if (shell != null)
            {
                await shell.DisplayAlert(AppResources.Alarm_Title,
                    AppResources.Alarm_Free_Message,
                    AppResources.Ok);
            }
            SelectedAlarm = "default"; // sentinel indicating default alarm
            UserPreferences.SetLastAlarm(SelectedAlarm);
            return;
        }

        SelectedAlarm = await _alarm.PickSystemSoundAsync();
        if (!string.IsNullOrEmpty(SelectedAlarm))
            UserPreferences.SetLastAlarm(SelectedAlarm);
    }

    public async Task OnTimerExpiredAsync()
    {
        var fade = TimeSpan.FromSeconds(UserPreferences.FadeOutSeconds);
        await _orchestrator.StopAllWithFadeAsync(fade);
        IsPlaying = false;

        var alarm = SelectedAlarm;
        if (_features.AlarmIntegrationEnabled && !string.IsNullOrWhiteSpace(alarm))
            _alarm.PlayAlarm(alarm);

        // Upgrade prompt only for Free users who hit the 15-minute cap
        var services = ServiceHost.Services;
        if (services == null) return;
        
        var features = services.GetRequiredService<FeatureGate>();
        var cap = features.MaxSessionLength();
        var planned = UserPreferences.Duration; // what the user set via the slider

        if (features.Tier == SubscriptionTier.Free
            && cap == TimeSpan.FromMinutes(15)
            && planned >= TimeSpan.FromMinutes(15))
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    var shell = Shell.Current;
                    if (shell != null)
                    {
                        await shell.GoToAsync(nameof(Views.UpgradePage));
                    }
                });
            }
            catch
            {
                // ignore navigation failures
            }
        }
    }

    private static AudioItem Clone(AudioItem s) => new()
    {
        Id = s.Id,
        Title = s.Title,
        SourceType = s.SourceType,
        BundledFileName = s.BundledFileName,
        DevicePath = s.DevicePath,
        Volume = s.Volume
    };

    [RelayCommand]
    public async Task PromptReviewAsync()
    {
        var services = ServiceHost.Services;
        if (services == null) return;
        
        var review = services.GetRequiredService<ReviewService>();
        await review.PromptAsync();
    }

    public void StopAlarmSound()
    {
        _alarm.StopAlarm();
    }

    private void StartSessionLimitWatchIfNeeded()
    {
        _sessionCts?.Cancel();
        var limit = _features.MaxSessionLength();
        if (limit <= TimeSpan.Zero) return; // unlimited

        var cts = new CancellationTokenSource();
        _sessionCts = cts;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(limit, cts.Token);
                if (!cts.IsCancellationRequested)
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await StopAllAsync();
                        // Optionally notify user here (toast/dialog)
                    });
                }
            }
            catch (TaskCanceledException) { /* ignore */ }
        });
    }

    // command to open EQ page
    [RelayCommand]
    public async Task OpenEqAsync()
    {
        if (!IsEqEnabled) return;
        var shell = Shell.Current;
        if (shell != null)
        {
            await shell.GoToAsync(nameof(AmbientSleeper.Views.EqPage));
        }
    }

    [RelayCommand]
    public async Task ExportAsync()
    {
        // Gate: Standard+ can export personal; Premium/Pro+ can choose shareable
        var scope = ExportScope.Personal;
        if (_features.ShareExportEnabled)
        {
            var shell = Shell.Current;
            if (shell == null) return;
            var choice = await shell.DisplayActionSheet(
                AppResources.ExportScope_Title, AppResources.Cancel, null, AppResources.ExportScope_Personal, AppResources.ExportScope_Shareable);
            if (string.IsNullOrEmpty(choice) || choice == AppResources.Cancel) return;
            scope = choice.StartsWith("Shareable", StringComparison.Ordinal) ? ExportScope.Shareable : ExportScope.Personal;
        }
        else if (!_features.ExportEnabled)
        {
            var shell = Shell.Current;
            if (shell != null)
            {
                await shell.DisplayAlert(AppResources.ExportLocked_Title,
                    AppResources.ExportLocked_Message, AppResources.Ok);
            }
            return;
        }

        var services = ServiceHost.Services;
        if (services == null) return;
        
        var exporter = services.GetRequiredService<IExportService>();
        try
        {
            // Localized choices for share mechanism
            var title = AppResources.ResourceManager.GetString("ExportShare_Title") ?? "Share export using";
            var viaSheet = AppResources.ResourceManager.GetString("ExportShare_ShareSheet") ?? "Share sheet";
            var viaEmail = AppResources.ResourceManager.GetString("ExportShare_Email") ?? "Email";

            var shell = Shell.Current;
            if (shell == null) return;
            var shareChoice = await shell.DisplayActionSheet(title, AppResources.Cancel, null, viaSheet, viaEmail);
            if (string.IsNullOrEmpty(shareChoice) || shareChoice == AppResources.Cancel) return;
            var shareByEmail = shareChoice == viaEmail;

            await exporter.ExportAsync(scope, shareByEmail);
            await shell.DisplayAlert(AppResources.ExportComplete_Title,
                AppResources.ExportComplete_Message, AppResources.Ok);
        }
        catch (Exception ex)
        {
            var shell = Shell.Current;
            if (shell != null)
            {
                await shell.DisplayAlert(AppResources.ExportFailed_Title, ex.Message, AppResources.Ok);
            }
        }
    }

    [RelayCommand]
    public async Task ImportAsync()
    {
        // Pick the export file
        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = AppResources.PickExport_Title,
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { "application/json", "text/json", "application/*" } },
                { DevicePlatform.iOS, new[] { "public.json", "public.data" } },
                { DevicePlatform.WinUI, new[] { ".json" } },
                { DevicePlatform.macOS, new[] { "json" } },
            })
        });

        if (result == null) return;

        var services = ServiceHost.Services;
        if (services == null) return;
        
        var exporter = services.GetRequiredService<IExportService>();
        try
        {
            var count = await exporter.ImportAsync(result.FullPath);
            RefreshMixes();
            RefreshPlaylists();
            var shell = Shell.Current;
            if (shell != null)
            {
                await shell.DisplayAlert(AppResources.ImportComplete_Title,
                    string.Format(AppResources.ImportComplete_MessageFormat, count), AppResources.Ok);
            }
        }
        catch (Exception ex)
        {
            var shell = Shell.Current;
            if (shell != null)
            {
                await shell.DisplayAlert(AppResources.ImportFailed_Title, ex.Message, AppResources.Ok);
            }
        }
    }
}
