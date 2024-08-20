using Microsoft.Extensions.Hosting;

namespace EnergySavingMode.Services;

internal class EventTrigger(CompactTimeline timeline, TimeProvider timeProvider) : BackgroundService, IEnergySavingModeEvents
{
    public event EventHandler? Enabled;
    public event EventHandler? Disabled;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        var now = timeProvider.GetUtcNow().LocalDateTime;
        var nextEvent = timeline.GetNextEvents(now).FirstOrDefault();

        if (nextEvent is null)
        {
            return;
        }

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
            if (nextEvent.DateTime > now) {
                using var periodicTimer = new PeriodicTimer(nextEvent.DateTime - now);
                await periodicTimer.WaitForNextTickAsync(stoppingToken);
            }
            
            now = timeProvider.GetUtcNow().LocalDateTime;
            nextEvent = timeline.GetNextEvents(now).FirstOrDefault();

            if (nextEvent is null)
            {
                continue;
            }

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
