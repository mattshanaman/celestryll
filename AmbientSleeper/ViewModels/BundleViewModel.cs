using System.Collections.ObjectModel;
using AmbientSleeper.Models;
using AmbientSleeper.Services;

namespace AmbientSleeper.ViewModels;

public class BundleFileViewModel
{
    public AudioFile File { get; set; }
    public bool IsUnlocked { get; set; }
    
    // Provide an AudioItem for compatibility with the Add to Mix/Playlist commands
    public AudioItem AsAudioItem => new AudioItem
    {
        Title = File.Name,
        BundledFileName = File.Metadata ?? string.Empty,
        SourceType = AudioSourceType.Bundled,
   Volume = 0.8 // Default volume
    };
    
    public BundleFileViewModel(AudioFile file, bool isUnlocked)
    {
        File = file ?? throw new ArgumentNullException(nameof(file));
  IsUnlocked = isUnlocked;
    }
}

public class BundleViewModel
{
    public Bundle Bundle { get; set; }
    public ObservableCollection<BundleFileViewModel> Files { get; set; }
    public bool IsUnlocked { get; set; }
    
    public BundleViewModel(Bundle bundle, bool isUnlocked, ObservableCollection<BundleFileViewModel> files)
{
        Bundle = bundle ?? throw new ArgumentNullException(nameof(bundle));
      IsUnlocked = isUnlocked;
      Files = files ?? new ObservableCollection<BundleFileViewModel>();
    }
}
