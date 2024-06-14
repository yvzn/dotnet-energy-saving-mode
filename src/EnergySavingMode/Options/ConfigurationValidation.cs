using Microsoft.Extensions.Options;
using System.Globalization;

namespace EnergySavingMode.Options;

internal class ConfigurationValidation : IValidateOptions<Configuration>
{
	public ValidateOptionsResult Validate(string? name, Configuration options)
	{
		var failures = new List<string>();

		failures.AddRange(Validate(options.Schedule[DayOfWeek.Monday]));
		failures.AddRange(Validate(options.Schedule[DayOfWeek.Tuesday]));
		failures.AddRange(Validate(options.Schedule[DayOfWeek.Wednesday]));
		failures.AddRange(Validate(options.Schedule[DayOfWeek.Thursday]));
		failures.AddRange(Validate(options.Schedule[DayOfWeek.Friday]));
		failures.AddRange(Validate(options.Schedule[DayOfWeek.Saturday]));
		failures.AddRange(Validate(options.Schedule[DayOfWeek.Sunday]));

		if (failures.Count > 0)
		{
			return ValidateOptionsResult.Fail(failures);
		}

		return ValidateOptionsResult.Success;
	}

	private IEnumerable<string> Validate(ICollection<TimeRange> timeRanges)
		=> timeRanges
			.Select(ValidateStartBeforeEnd)
			.Union(ValidateOverlaps(timeRanges))
			.OfType<string>();

	private string? ValidateStartBeforeEnd(TimeRange timeRange)
	{
		var (startTime, endTime) = timeRange;

		if (startTime >= endTime)
		{
			return $"{timeRange} has a start time after its end time";
		}

		return default;
	}

	private IEnumerable<string> ValidateOverlaps(ICollection<TimeRange> timeRanges)
	{
		TimeRange? previous = default;

		var enumerator = timeRanges.GetEnumerator();
		while(enumerator.MoveNext()) {
			var current = enumerator.Current;

			if (previous is null)
			{
				previous = current;
				continue;
			}

			if (current.Start < previous.End)
			{
				yield return $"{current} overlaps ${previous}";
			}
			previous = current;
		}
	}
}

