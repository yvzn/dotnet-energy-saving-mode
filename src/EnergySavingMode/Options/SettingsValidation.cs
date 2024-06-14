using Microsoft.Extensions.Options;
using System.Globalization;

namespace EnergySavingMode.Options;

internal class SettingsValidation : IValidateOptions<Settings>
{
	public ValidateOptionsResult Validate(string? name, Settings options)
	{
		var failures = new List<string>();

		failures.AddRange(Validate(options.Enabled.Mon));
		failures.AddRange(Validate(options.Enabled.Tue));
		failures.AddRange(Validate(options.Enabled.Wed));
		failures.AddRange(Validate(options.Enabled.Thu));
		failures.AddRange(Validate(options.Enabled.Fri));
		failures.AddRange(Validate(options.Enabled.Sat));
		failures.AddRange(Validate(options.Enabled.Sun));

		if (failures.Count > 0)
		{
			return ValidateOptionsResult.Fail(failures);
		}

		return ValidateOptionsResult.Success;
	}

	private static IEnumerable<string> Validate(ICollection<string> schedule)
		=> schedule.Select(Validate).OfType<string>();

	private static string? Validate(string entry)
	{
		var maybeTimeRange = entry.Split('-');
		if (maybeTimeRange.Length < 2)
		{
			return $"{entry} is not a valid range of time";
		}

		foreach (var maybeTime in maybeTimeRange)
		{
			if (!IsValidTime(maybeTime))
			{
				return $"{entry} contains an invalid time value {maybeTime}";
			}
		}

		return default;
	}

	private static bool IsValidTime(string maybeTime)
		=> TimeSpan.TryParse(maybeTime, CultureInfo.CurrentCulture, out _);
}
