using Microsoft.Extensions.Options;

namespace EnergySavingMode.Services;

internal class Timeline(IOptions<Options.Configuration> options)
{
	private readonly Options.Configuration configuration = options.Value;

	public IEnumerable<SeriesEvent> GetNextEventSeries(DateTime startingDateTime)
	{
		var dayOfWeek = startingDateTime.DayOfWeek;
		var startingTime = TimeOnly.FromDateTime(startingDateTime);

		var maxDaysToConsider = 30;
		var daysElapsed = 0;

		var timeRangesOfTheDay = configuration.Schedule[dayOfWeek];
		foreach (var timeRange in timeRangesOfTheDay)
		{
			if (startingTime <= timeRange.Start)
			{
				yield return new(dayOfWeek, timeRange.Start, EventType.Start, daysElapsed);
			}

			if (startingTime <= timeRange.End)
			{
				yield return new(dayOfWeek, timeRange.End, EventType.End, daysElapsed);
			}
		}

		daysElapsed = 1;
		dayOfWeek = dayOfWeek.Next();

		for (; daysElapsed < maxDaysToConsider; ++daysElapsed)
		{
			timeRangesOfTheDay = configuration.Schedule[dayOfWeek];

			foreach (var timeRange in timeRangesOfTheDay)
			{
				yield return new(dayOfWeek, timeRange.Start, EventType.Start, daysElapsed);
				yield return new(dayOfWeek, timeRange.End, EventType.End, daysElapsed);
			}

			dayOfWeek = dayOfWeek.Next();
		}
	}

	internal IEnumerable<EventOccurence> GetNextEventOccurences(DateTime startingDateTime)
	{
		var startingDate = DateOnly.FromDateTime(startingDateTime);
		foreach (var seriesEvent in GetNextEventSeries(startingDateTime))
		{
			var occurenceDate = startingDate.AddDays(seriesEvent.daysElapsed);
			yield return new(occurenceDate.ToDateTime(seriesEvent.Time, DateTimeKind.Local), seriesEvent.Type);
		}
	}
}

internal record SeriesEvent(DayOfWeek Day, TimeOnly Time, EventType Type, int daysElapsed);

internal record EventOccurence(DateTime DateTime, EventType Type);

internal enum EventType { End = 0, Start = 1 };

internal static class DayOfWeekExtensions
{
	public static DayOfWeek Next(this DayOfWeek dayOfWeek)
	{
		return (DayOfWeek)(((int)dayOfWeek + 1) % 7);
	}
}
