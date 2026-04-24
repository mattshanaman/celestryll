using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AmbientSleeper.Models;
using AmbientSleeper.Services;
using System.Linq;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using AmbientSleeper.Resources.Strings;

namespace AmbientSleeper.ViewModels;

public partial class LibraryViewModel : ObservableObject
{
    public ObservableCollection<AudioItem> DeviceAudio { get; } = new();
    public ObservableCollection<SavedPlaylist> Playlists { get; } = new();
    public ObservableCollection<AudioItem> LibrarySounds { get; } = new();

    // Bundles for UI (includes the "Built-in" bundle)
    public ObservableCollection<BundleViewModel> Bundles { get; } = new();

    // Mix Playlists for UI (Library tab)
    public ObservableCollection<SavedMixPlaylist> MixPlaylists { get; } = new();

    [ObservableProperty]
    private SavedPlaylist? selectedPlaylist;

    private readonly PlaybackViewModel _playback;
    private readonly FeatureGate _features;
    private readonly IAudioBundleService _bundleService;

    // flag for enabling Add-to-Mix UI
    public bool CanAddMoreToMix => _playback.MixSelection.Count < _features.MaxOverlaySounds;

    // bindable property for gating playlist actions (Free=false)
    public bool IsPlaylistEnabled => _features.PlaylistEnabled;

    // gate Mix Playlist features by tier
    public bool IsMixPlaylistEnabled => _features.MixPlaylistEnabled;

    // Updated tab index: 0 = Bundles (including Built-in), 1 = Your audio, 2 = Mix Playlist
    [ObservableProperty] private int selectedTabIndex = 0;

    // command to change tabs (bound from header buttons)
    [RelayCommand]
    public void ChangeTab(string tabIndex)
    {
      if (int.TryParse(tabIndex, out var index))
   SelectedTabIndex = index;
    }

    public LibraryViewModel(PlaybackViewModel playback, FeatureGate features, IAudioBundleService bundleService)
    {
        _playback = playback ?? throw new ArgumentNullException(nameof(playback));
      _features = features ?? throw new ArgumentNullException(nameof(features));
        _bundleService = bundleService ?? throw new ArgumentNullException(nameof(bundleService));

        // update availability when mix changes or tier changes
     _playback.MixSelection.CollectionChanged += (_, __) => OnPropertyChanged(nameof(CanAddMoreToMix));

        // notify when tier changes so bindings update
      _features.Changed += (_, __) =>
        {
 OnPropertyChanged(nameof(CanAddMoreToMix));
     OnPropertyChanged(nameof(IsPlaylistEnabled));
            OnPropertyChanged(nameof(IsMixPlaylistEnabled));
          RefreshBundles(); // Refresh bundles when tier changes
        };

        RefreshBundles();
 RefreshPlaylists();
     RefreshMixPlaylists();
    }

    public void RefreshBundles()
    {
        try
        {
            var tier = _features.Tier;
            var allBundles = _bundleService.GetAllBundles();
            
            // Pre-cache available files to avoid repeated LINQ queries (O(n²) → O(n))
            var availableFilesLookup = _bundleService.GetAvailableFilesForTier(tier)
                .ToDictionary(f => f.Id);
            
            // Build new collection off the UI thread to reduce jank
            var newBundles = new List<BundleViewModel>();
            
            foreach (var bundle in allBundles)
            {
                var isBundleUnlocked = _bundleService.IsBundleUnlockedForTier(tier, bundle.Id);
                var fileVMs = new ObservableCollection<BundleFileViewModel>();
                
                foreach (var bf in bundle.Files.OrderBy(f => f.Order))
                {
                    // Use dictionary lookup instead of FirstOrDefault for O(1) performance
                    if (availableFilesLookup.TryGetValue(bf.FileId, out var file))
                    {
                        bool isUnlocked = _bundleService.IsFileUnlockedForTier(tier, bf.FileId);
                        fileVMs.Add(new BundleFileViewModel(file, isUnlocked));
                    }
                }
                
                newBundles.Add(new BundleViewModel(bundle, isBundleUnlocked, fileVMs));
            }
            
            // Single batch update to collection (reduces CollectionChanged events)
            Bundles.Clear();
            foreach (var bundle in newBundles)
            {
                Bundles.Add(bundle);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error refreshing bundles: {ex.Message}");
            // Bundles will remain empty or partially populated
        }
    }

    public void RefreshPlaylists()
    {
        try
        {
            // Batch the collection update
            var playlists = UserPreferences.GetPlaylists();
            Playlists.Clear();
            foreach (var p in playlists)
            {
                Playlists.Add(p);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error refreshing playlists: {ex.Message}");
        }
    }

    public void RefreshMixPlaylists()
    {
   try
        {
   MixPlaylists.Clear();
   foreach (var p in UserPreferences.GetMixPlaylists())
   MixPlaylists.Add(p);
    }
        catch (Exception ex)
        {
     System.Diagnostics.Debug.WriteLine($"Error refreshing mix playlists: {ex.Message}");
        }
    }

    [RelayCommand]
    public void AddSoundToPlaylist((AudioItem sound, string playlistName) param)
    {
        try
        {
     // strict gate — no-op on Free
            if (!_features.PlaylistEnabled) return;
    if (param.sound == null || string.IsNullOrWhiteSpace(param.playlistName)) return;

    UserPreferences.AddSoundToPlaylist(param.playlistName, param.sound);
    RefreshPlaylists();
 }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error adding sound to playlist: {ex.Message}");
   }
    }

    [RelayCommand]
    public async Task ImportDeviceAudioAsync()
    {
      try
        {
  var audioFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
   {
   { DevicePlatform.iOS, new[] { "public.audio" } },
    { DevicePlatform.Android, new[] { "audio/*" } },
     { DevicePlatform.WinUI, new[] { ".mp3", ".wav", ".m4a" } },
                { DevicePlatform.macOS, new[] { "mp3", "wav", "m4a" } }
          });

    var result = await FilePicker.PickAsync(new PickOptions
       {
              FileTypes = audioFileType,
           PickerTitle = AppResources.PickAudio_Title
            });
       
     if (result == null) return;

      await using var src = await result.OpenReadAsync();
          var cachePath = Path.Combine(FileSystem.CacheDirectory, $"{Guid.NewGuid()}_{result.FileName}");
       await using (var fs = File.Create(cachePath))
                await src.CopyToAsync(fs);

   var item = new AudioItem
            {
    Title = Path.GetFileNameWithoutExtension(result.FileName),
  SourceType = AudioSourceType.Device,
 DevicePath = cachePath,
       Volume = 1.0
            };
      DeviceAudio.Add(item);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error importing device audio: {ex.Message}");
  // Could show user notification here
        }
    }

    [RelayCommand]
    public void AddToMix(AudioItem item)
    {
        try
  {
            if (item == null) return;
      _playback.AddToMix(item);
        }
        catch (Exception ex)
        {
       System.Diagnostics.Debug.WriteLine($"Error adding to mix: {ex.Message}");
        }
    }

    [RelayCommand]
    public void AddToPlaylist(AudioItem item)
    {
        try
     {
// strict gate — no-op on Free
  if (!_features.PlaylistEnabled) return;
         if (item == null) return;
   
         _playback.AddToPlaylist(item);
        }
        catch (Exception ex)
     {
            System.Diagnostics.Debug.WriteLine($"Error adding to playlist: {ex.Message}");
        }
    }

    [RelayCommand]
    public void SaveMixPlaylistFromLibrary(string name)
    {
        try
    {
       if (!IsMixPlaylistEnabled) return;
      if (string.IsNullOrWhiteSpace(name)) return;

      // duplicate name check
         if (UserPreferences.GetMixPlaylists().Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
         {
                // optional: could surface a UI alert here
       return;
  }

       var features = _features;
            var entries = _playback.MixPlaylistQueue.Select(e => new MixPlaylistEntry
            {
    Mix = e.Mix,
             DurationSeconds = e.DurationSeconds,
    TransitionSeconds = Math.Clamp(e.TransitionSeconds, 0, features.MaxMixTransitionSeconds)
      }).ToList();

          var list = new SavedMixPlaylist
            {
  Name = name,
Entries = entries,
         Loop = _playback.MixPlaylistLoopEnabled,
          SavedAt = DateTime.UtcNow
  };
       UserPreferences.SaveMixPlaylist(list);
            RefreshMixPlaylists();
        }
 catch (Exception ex)
        {
   System.Diagnostics.Debug.WriteLine($"Error saving mix playlist: {ex.Message}");
        }
    }

    [RelayCommand]
    public void DeleteMixPlaylist(string name)
    {
        try
 {
          if (string.IsNullOrWhiteSpace(name)) return;
            
     UserPreferences.DeleteMixPlaylist(name);
 RefreshMixPlaylists();
   }
        catch (Exception ex)
   {
            System.Diagnostics.Debug.WriteLine($"Error deleting mix playlist: {ex.Message}");
 }
 }

    public async Task<string?> PickUserAudioAsync()
    {
        try
        {
     var audioFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
  {
                { DevicePlatform.iOS, new[] { "public.audio" } },
         { DevicePlatform.Android, new[] { "audio/*" } },
   { DevicePlatform.WinUI, new[] { ".mp3", ".wav", ".m4a" } },
         { DevicePlatform.macOS, new[] { "mp3", "wav", "m4a" } }
            });

            var options = new PickOptions
       {
      PickerTitle = AppResources.PickAudio_TitleAlt,
       FileTypes = audioFileType
      };

  var result = await FilePicker.PickAsync(options);
     return result?.FullPath;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error picking user audio: {ex.Message}");
            return null;
        }
    }
}
