namespace AmbientSleeper.Services;

public class TimerService : ITimerService
{
    private CancellationTokenSource? _ctsLinked;
    public bool IsScheduled => ScheduledForUtc.HasValue;
    public DateTime? ScheduledForUtc { get; private set; }

    public async Task ScheduleStopAsync(DateTime stopAtUtc, CancellationToken externalToken, Action onStop)
    {
        Cancel();
        ScheduledForUtc = stopAtUtc;
        var cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
        _ctsLinked = cts;

        try
        {
            var delay = stopAtUtc - DateTime.UtcNow;
            if (delay < TimeSpan.Zero) delay = TimeSpan.Zero;
            await Task.Delay(delay, cts.Token);
            if (!cts.IsCancellationRequested)
                onStop();
        }
        catch (TaskCanceledException) { /* ignore */ }
        finally
        {
            ScheduledForUtc = null;
            _ctsLinked?.Dispose();
            _ctsLinked = null;
        }
    }

    public void Cancel()
    {
        _ctsLinked?.Cancel();
        _ctsLinked?.Dispose();
        _ctsLinked = null;
        ScheduledForUtc = null;
    }
}