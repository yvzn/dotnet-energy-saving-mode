using Microsoft.Extensions.Hosting;

namespace EnergySavingMode.Services;

internal class EventTrigger(ITimeline timeline, TimeProvider timeProvider) : BackgroundService, IEnergySavingModeEvents
{
    public event EventHandler? Enabled;
    public event EventHandler? Disabled;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        var now = timeProvider.GetLocalNow().LocalDateTime;
Console.WriteLine($"now: {now:dd/MM:HH:mm:ss:fff K}");
        var nextEvent = timeline.GetNextEventOccurences(now).FirstOrDefault();

        if (nextEvent is null)
        {
            return;
        }
Console.WriteLine($"next: {nextEvent.DateTime:dd/MM:HH:mm:ss:fff} type: {nextEvent.Type}");

        var isEnergySavingModeEnabled = nextEvent.Type == EventType.End;
        if (isEnergySavingModeEnabled)
        {
            Enabled?.Invoke(this, new());
        }
        else
        {
            Disabled?.Invoke(this, new());
        }

        var isStopping = false;
        stoppingToken.Register(() => isStopping = true);

        while (nextEvent is not null && !isStopping)
        {
            now = timeProvider.GetLocalNow().LocalDateTime;
Console.WriteLine($"now: {now:dd/MM:HH:mm:ss:fff K}");

            if (nextEvent.DateTime > now) {
Console.WriteLine($"wait: {nextEvent.DateTime:dd/MM:HH:mm:ss:fff} for: {(nextEvent.DateTime - now).TotalMilliseconds} ms");
                using var periodicTimer = new PeriodicTimer(nextEvent.DateTime - now);
                await periodicTimer.WaitForNextTickAsync(stoppingToken);
            }

            now = timeProvider.GetLocalNow().LocalDateTime;
Console.WriteLine($"now: {now:dd/MM:HH:mm:ss:fff K}");
            nextEvent = timeline.GetNextEventOccurences(now).FirstOrDefault();

            if (nextEvent is null)
            {
                continue;
            }
Console.WriteLine($"next: {nextEvent.DateTime:dd/MM:HH:mm:ss:fff} type: {nextEvent.Type}");

            isEnergySavingModeEnabled = nextEvent.Type == EventType.End;
            if (isEnergySavingModeEnabled)
            {
                Enabled?.Invoke(this, new());
            }
            else
            {
                Disabled?.Invoke(this, new());
            }

        }
    }
}
