using Microsoft.Extensions.Options;
using System.Linq;

namespace EnergySavingMode.Services;

internal class Timeline(IOptions<Options.Configuration> options)
{
	private readonly Options.Configuration configuration = options.Value;

	public IEnumerable<TimelineEvent> Next(DateTime dateTime)
	{
		var maxIterations = 50;
		var dayOfWeek = dateTime.DayOfWeek;
		var timeOnly = TimeOnly.FromDateTime(dateTime);

		for (var count = 0; count < maxIterations; ++count)
		{
			var timeRangesOfTheDay = configuration.Schedule[dayOfWeek];
			foreach (var timeRange in timeRangesOfTheDay)
			{
				if (timeOnly <= timeRange.Start)
				{
					yield return new(dayOfWeek, timeRange.Start, TimelineEventType.Start);
				}

				if (timeOnly <= timeRange.End)
				{
					yield return new(dayOfWeek, timeRange.End, TimelineEventType.End);
				}
			}

			dayOfWeek = (DayOfWeek)(((int)dayOfWeek + 1) % 7);
		}
	}
}

public record TimelineEvent(DayOfWeek Day, TimeOnly Time, TimelineEventType Type);

public enum TimelineEventType { End = 0, Start = 1 };
