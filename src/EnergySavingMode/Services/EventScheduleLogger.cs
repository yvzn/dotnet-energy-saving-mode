using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EnergySavingMode.Services;

internal class EventScheduleLogger(ITimeline timeline, TimeProvider timeProvider, ILogger<EventScheduleLogger> logger) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await Task.Yield();

		var now = timeProvider.GetLocalNow().LocalDateTime;
		logger.LogInformation("EnergySavingMode is starting... Local server time is {EnergySavingMode_LocalDateTime}", now.ToString("O"));

		var nextEvents = timeline.GetNextEventOccurences(now).Take(5);
		var eventSchedule = nextEvents
			.Select(e => string.Format("{0:O} => {1}", e.DateTime, e.Type == EventType.End ? "Disabled" : "Enabled"));

		logger.LogInformation("Next scheduled EnergySavingMode events are: {NewLine}{EnergySavingMode_EventSchedule}",
			Environment.NewLine, string.Join(Environment.NewLine, eventSchedule));
	}
}
