using Microsoft.Extensions.Hosting;

namespace EnergySavingMode.Services;

internal class EventTrigger(ITimeline timeline, TimeProvider timeProvider, EventBroadcast eventBroadcast) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await Task.Yield();

		var now = timeProvider.GetLocalNow().LocalDateTime;
		var nextEvent = timeline.GetNextEventOccurences(now).FirstOrDefault();

		if (nextEvent is null)
		{
			return;
		}

		await eventBroadcast.SendAsync(isEnergySavingModeEnabled: nextEvent.Type == EventType.End, stoppingToken);

		var isStopping = false;
		stoppingToken.Register(() => isStopping = true);

		while (nextEvent is not null && !isStopping)
		{
			now = timeProvider.GetLocalNow().LocalDateTime;

			if (nextEvent.DateTime > now)
			{
				using var periodicTimer = new PeriodicTimer(nextEvent.DateTime - now);
				await periodicTimer.WaitForNextTickAsync(stoppingToken);
			}

			now = timeProvider.GetLocalNow().LocalDateTime;
			nextEvent = timeline.GetNextEventOccurences(now).FirstOrDefault();

			if (nextEvent is null)
			{
				continue;
			}

			await eventBroadcast.SendAsync(isEnergySavingModeEnabled: nextEvent.Type == EventType.End, stoppingToken);
		}
	}
}
