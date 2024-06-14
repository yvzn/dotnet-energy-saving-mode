using Microsoft.Extensions.Options;
using System.Globalization;

namespace EnergySavingMode.Options;

internal class ConfigureFromAppSettings(IOptions<Settings> settings)
	: IConfigureOptions<Configuration>
{
	public void Configure(Configuration options)
	{
		var enabledByDayOfWeek = settings.Value.Enabled;

		options.Schedule[DayOfWeek.Sunday] = Convert(enabledByDayOfWeek.Sun);
		options.Schedule[DayOfWeek.Monday] = Convert(enabledByDayOfWeek.Mon);
		options.Schedule[DayOfWeek.Tuesday] = Convert(enabledByDayOfWeek.Tue);
		options.Schedule[DayOfWeek.Wednesday] = Convert(enabledByDayOfWeek.Wed);
		options.Schedule[DayOfWeek.Thursday] = Convert(enabledByDayOfWeek.Thu);
		options.Schedule[DayOfWeek.Friday] = Convert(enabledByDayOfWeek.Fri);
		options.Schedule[DayOfWeek.Saturday] = Convert(enabledByDayOfWeek.Sat);
	}

	private static List<TimeRange> Convert(ICollection<string> daySchedule)
		=> daySchedule.Select(Convert).OrderBy(t => t.Start).ToList();

	private static TimeRange Convert(string timeRangeString)
	{
		var startAndEndTimes = timeRangeString.Split('-').Select(Parse).ToArray();
		return new(startAndEndTimes[0], startAndEndTimes[1]);
	}

	private static TimeOnly Parse(string s)
		=> TimeOnly.Parse(s, CultureInfo.CurrentCulture);
}
