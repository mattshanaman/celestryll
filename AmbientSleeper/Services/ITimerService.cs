namespace AmbientSleeper.Services;

public interface ITimerService
{
    Task ScheduleStopAsync(DateTime stopAtUtc, CancellationToken cancellationToken, Action onStop);
    void Cancel();
    bool IsScheduled { get; }
    DateTime? ScheduledForUtc { get; }
}