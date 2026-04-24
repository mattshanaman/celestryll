using AmbientSleeper.Services;

namespace AmbientSleeper.ViewModels;

public partial class TimerViewModel
{
    // Lazy-resolve FeatureGate to avoid changing the existing constructor
    private FeatureGate? _features;
    private FeatureGate? Features
    {
        get
        {
            if (_features != null) return _features;
            var services = ServiceHost.Services;
            if (services == null) return null;
            _features = services.GetRequiredService<FeatureGate>();
            return _features;
        }
    }

    // Cache last value to avoid repeated Preference writes while user slides
    private double _selectedMinutesBacking;

    // Max minutes allowed by current tier (UI uses this for Slider.Maximum)
    public double MaxDurationMinutes
    {
        get
        {
            // Pro+ uses MaxValue; show a reasonable large bound in the UI
            var features = Features;
            if (features == null) return 1440; // Fallback: 24h cap
            
            var max = features.MaxSessionLength();
            return max == TimeSpan.MaxValue
                ? 1440 // 24h cap for UI
                : Math.Max(1, max.TotalMinutes);
        }
    }

    // 2-way bound to the slider
    public double SelectedMinutes
    {
        get => _selectedMinutesBacking == 0
            ? Math.Round(UserPreferences.Duration.TotalMinutes)
            : _selectedMinutesBacking;
        set
        {
            var clamped = Math.Clamp(value, 1, MaxDurationMinutes);
            if (Math.Abs(_selectedMinutesBacking - clamped) < double.Epsilon)
                return;

            _selectedMinutesBacking = clamped;
            UserPreferences.SetDuration(TimeSpan.FromMinutes(clamped));
            OnPropertyChanged(nameof(SelectedMinutes));
        }
    }

    // Call once before showing the timer UI (or from your existing ctor if you prefer)
    public void EnsureFeatureSubscriptions()
    {
        if (_features != null)
            return;

        var services = ServiceHost.Services;
        if (services == null) return;

        _features = services.GetRequiredService<FeatureGate>();
        _features.Changed += (_, __) =>
        {
            // Notify UI to update the slider cap
            OnPropertyChanged(nameof(MaxDurationMinutes));

            // Clamp current selection if it exceeds the new cap
            if (SelectedMinutes > MaxDurationMinutes)
            {
                SelectedMinutes = MaxDurationMinutes;
            }
        };
    }
}