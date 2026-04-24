using AmbientSleeper.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.LocalNotification;
using AmbientSleeper.Resources.Strings;

namespace AmbientSleeper.ViewModels;

public partial class TimerViewModel : ObservableObject
{
    public PlaybackViewModel? Playback { get; set; } // Property injection; set in DI wiring

    private CancellationTokenSource? _cts;

    [ObservableProperty] private bool useDuration = true;
    [ObservableProperty] private bool useAtTime = false;

    [ObservableProperty] private TimeSpan duration = TimeSpan.FromHours(1);
    [ObservableProperty] private TimeSpan stopAtTime = new(23, 0, 0);

    [ObservableProperty] private TimeSpan remaining;
    [ObservableProperty] private bool isRunning;

    partial void OnUseDurationChanged(bool value)
    {
        if (value) UseAtTime = false;
        UserPreferences.SetUseDuration(value);
    }

    partial void OnUseAtTimeChanged(bool value)
    {
        if (value) UseDuration = false;
        // UseDuration preference is the inverse of UseAtTime
        UserPreferences.SetUseDuration(!value);
    }

    partial void OnDurationChanged(TimeSpan value)
    {
        UserPreferences.SetDuration(value);
        if (!IsRunning && UseDuration) Remaining = value;
    }
    partial void OnStopAtTimeChanged(TimeSpan value) => UserPreferences.SetStopAtTime(value);
    

    public TimerViewModel()
    {
        remaining = duration;

        useDuration = UserPreferences.UseDuration;
        duration = UserPreferences.Duration;
        stopAtTime = UserPreferences.StopAtTime;
    }

    [RelayCommand]
    public async Task StartTimerAsync()
    {
        if (IsRunning) return;
        IsRunning = true;
        _cts = new();
        var token = _cts.Token;

        DateTime endUtc;
        if (UseDuration)
        {
            endUtc = DateTime.UtcNow + Duration;
        }
        else
        {
            var nowLocal = DateTime.Now;
            var todayTarget = new DateTime(nowLocal.Year, nowLocal.Month, nowLocal.Day,
                                           StopAtTime.Hours, StopAtTime.Minutes, 0);
            if (todayTarget <= nowLocal) todayTarget = todayTarget.AddDays(1);
            endUtc = todayTarget.ToUniversalTime();
        }

        try
        {
            while (!token.IsCancellationRequested)
            {
                var left = endUtc - DateTime.UtcNow;
                if (left <= TimeSpan.Zero) break;
                MainThread.BeginInvokeOnMainThread(() => Remaining = left);
                await Task.Delay(250, token);
            }
        }
        catch (TaskCanceledException) { }

        IsRunning = false;

        if (!(_cts?.IsCancellationRequested ?? true))
        {
            Remaining = TimeSpan.Zero;
            await OnTimerExpiredAsync();
        }
        else
        {
            // Reset Remaining when canceled (for duration mode)
            if (UseDuration) Remaining = Duration;
        }
    }

    [RelayCommand]
    public void CancelTimer()
    {
        _cts?.Cancel();
        IsRunning = false;
        Remaining = Duration;
    }

    [RelayCommand]
    public void Add5Min() => Duration += TimeSpan.FromMinutes(5);

    [RelayCommand]
    public void Sub5Min()
    {
        if (Duration > TimeSpan.FromMinutes(5))
            Duration -= TimeSpan.FromMinutes(5);
    }

    [RelayCommand]
    public void StopAlarm()
    {
        Playback?.StopAlarmSound();
    }

    // Called when the countdown reaches zero
    private async Task OnTimerExpiredAsync()
    {
        // 1) Stop playback with fade + play optional alarm
        if (Playback != null)
        {
            await Playback.OnTimerExpiredAsync();
        }
        
        // 2) Fire a local notification so the user sees completion (even if app is backgrounded)
        try
        {
            // Request permission if needed (no-op on Android if already granted)
            await LocalNotificationCenter.Current.RequestNotificationPermission();

            var request = new NotificationRequest
            {
                NotificationId = 2001,
                Title = AppResources.Notification_TimerComplete_Title,
                Description = AppResources.Notification_TimerComplete_Description,
                Schedule = new NotificationRequestSchedule
                {
                    // Deliver immediately with a tiny delay for reliability
                    NotifyTime = DateTime.Now.AddSeconds(1)
                },
                CategoryType = NotificationCategoryType.Reminder
            };

            await LocalNotificationCenter.Current.Show(request);
        }
        catch
        {
            // Best effort; if notifications fail, do nothing
        }
    }
}
