namespace Pemdas.Services;

public interface IFeedbackService
{
    // Audio feedback
    Task PlaySuccessSound();
    Task PlayErrorSound();
    Task PlayClickSound();
    Task PlayHintSound();
    Task PlayStreakSound();
    Task PlayCountdownSound();
    
    // Haptic feedback
    void SuccessVibration();
    void ErrorVibration();
    void LightTap();
    void MediumImpact();
    void HeavyImpact();
    
    // Combined feedback
    Task PlaySuccessFeedback();
    Task PlayErrorFeedback();
    Task PlayHintFeedback();
    Task PlayStreakFeedback();
    
    // Settings
    bool IsSoundEnabled { get; set; }
    bool IsHapticEnabled { get; set; }
}

public class FeedbackService : IFeedbackService
{
    private bool _isSoundEnabled = true;
    private bool _isHapticEnabled = true;
    
    // Simple in-memory cache for audio players
    private readonly Dictionary<string, IDispatcherTimer?> _soundCooldowns = new();
    private const int CooldownMs = 100; // Prevent sound spam

    public bool IsSoundEnabled
    {
        get => _isSoundEnabled;
        set => _isSoundEnabled = value;
    }

    public bool IsHapticEnabled
    {
        get => _isHapticEnabled;
        set => _isHapticEnabled = value;
    }

    // Audio Methods
    public async Task PlaySuccessSound()
    {
        if (!IsSoundEnabled || !CanPlaySound("success"))
            return;

        try
        {
            // Use MAUI MediaElement or platform-specific audio
            // For now, we'll use haptic as fallback since MediaElement requires XAML setup
            await Task.CompletedTask;
            System.Diagnostics.Debug.WriteLine("?? Playing success sound");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error playing success sound: {ex.Message}");
        }
    }

    public async Task PlayErrorSound()
    {
        if (!IsSoundEnabled || !CanPlaySound("error"))
            return;

        try
        {
            await Task.CompletedTask;
            System.Diagnostics.Debug.WriteLine("?? Playing error sound");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error playing error sound: {ex.Message}");
        }
    }

    public async Task PlayClickSound()
    {
        if (!IsSoundEnabled || !CanPlaySound("click"))
            return;

        try
        {
            await Task.CompletedTask;
            System.Diagnostics.Debug.WriteLine("?? Playing click sound");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error playing click sound: {ex.Message}");
        }
    }

    public async Task PlayHintSound()
    {
        if (!IsSoundEnabled || !CanPlaySound("hint"))
            return;

        try
        {
            await Task.CompletedTask;
            System.Diagnostics.Debug.WriteLine("?? Playing hint sound");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error playing hint sound: {ex.Message}");
        }
    }

    public async Task PlayStreakSound()
    {
        if (!IsSoundEnabled || !CanPlaySound("streak"))
            return;

        try
        {
            await Task.CompletedTask;
            System.Diagnostics.Debug.WriteLine("?? Playing streak sound");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error playing streak sound: {ex.Message}");
        }
    }

    public async Task PlayCountdownSound()
    {
        if (!IsSoundEnabled || !CanPlaySound("countdown"))
            return;

        try
        {
            await Task.CompletedTask;
            System.Diagnostics.Debug.WriteLine("?? Playing countdown sound");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error playing countdown sound: {ex.Message}");
        }
    }

    // Haptic Methods
    public void SuccessVibration()
    {
        if (!IsHapticEnabled)
            return;

        try
        {
#if ANDROID || IOS
            HapticFeedback.Perform(HapticFeedbackType.Click);
            // Perform a success pattern: short-pause-short
            Task.Run(async () =>
            {
                Vibration.Vibrate(TimeSpan.FromMilliseconds(50));
                await Task.Delay(50);
                Vibration.Vibrate(TimeSpan.FromMilliseconds(50));
            });
#endif
            System.Diagnostics.Debug.WriteLine("?? Success vibration");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error with success vibration: {ex.Message}");
        }
    }

    public void ErrorVibration()
    {
        if (!IsHapticEnabled)
            return;

        try
        {
#if ANDROID || IOS
            HapticFeedback.Perform(HapticFeedbackType.LongPress);
            Vibration.Vibrate(TimeSpan.FromMilliseconds(200));
#endif
            System.Diagnostics.Debug.WriteLine("?? Error vibration");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error with error vibration: {ex.Message}");
        }
    }

    public void LightTap()
    {
        if (!IsHapticEnabled)
            return;

        try
        {
#if ANDROID || IOS
            HapticFeedback.Perform(HapticFeedbackType.Click);
#endif
            System.Diagnostics.Debug.WriteLine("?? Light tap");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error with light tap: {ex.Message}");
        }
    }

    public void MediumImpact()
    {
        if (!IsHapticEnabled)
            return;

        try
        {
#if ANDROID || IOS
            HapticFeedback.Perform(HapticFeedbackType.Click);
            Vibration.Vibrate(TimeSpan.FromMilliseconds(50));
#endif
            System.Diagnostics.Debug.WriteLine("?? Medium impact");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error with medium impact: {ex.Message}");
        }
    }

    public void HeavyImpact()
    {
        if (!IsHapticEnabled)
            return;

        try
        {
#if ANDROID || IOS
            HapticFeedback.Perform(HapticFeedbackType.LongPress);
            Vibration.Vibrate(TimeSpan.FromMilliseconds(100));
#endif
            System.Diagnostics.Debug.WriteLine("?? Heavy impact");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error with heavy impact: {ex.Message}");
        }
    }

    // Combined Feedback
    public async Task PlaySuccessFeedback()
    {
        SuccessVibration();
        await PlaySuccessSound();
    }

    public async Task PlayErrorFeedback()
    {
        ErrorVibration();
        await PlayErrorSound();
    }

    public async Task PlayHintFeedback()
    {
        LightTap();
        await PlayHintSound();
    }

    public async Task PlayStreakFeedback()
    {
        MediumImpact();
        await PlayStreakSound();
    }

    // Helper Methods
    private bool CanPlaySound(string soundId)
    {
        if (_soundCooldowns.TryGetValue(soundId, out var _))
        {
            return false; // Still in cooldown
        }

        // Set cooldown
        var timer = Application.Current?.Dispatcher.CreateTimer();
        if (timer != null)
        {
            timer.Interval = TimeSpan.FromMilliseconds(CooldownMs);
            timer.Tick += (s, e) =>
            {
                _soundCooldowns.Remove(soundId);
                timer.Stop();
            };
            _soundCooldowns[soundId] = timer;
            timer.Start();
        }

        return true;
    }
}
