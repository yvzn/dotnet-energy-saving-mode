namespace EnergySavingMode.Options;

public class Configuration
{
	public IDictionary<DayOfWeek, ICollection<TimeRange>> Schedule { get; } = new Dictionary<DayOfWeek, ICollection<TimeRange>>
	{
		{ DayOfWeek.Sunday, Array.Empty<TimeRange>() },
		{ DayOfWeek.Monday, Array.Empty<TimeRange>() },
		{ DayOfWeek.Tuesday, Array.Empty<TimeRange>() },
		{ DayOfWeek.Wednesday, Array.Empty<TimeRange>() },
		{ DayOfWeek.Thursday, Array.Empty<TimeRange>() },
		{ DayOfWeek.Friday, Array.Empty<TimeRange>() },
		{ DayOfWeek.Saturday, Array.Empty<TimeRange>() },
	};
}

public record TimeRange(TimeOnly Start, TimeOnly End);
