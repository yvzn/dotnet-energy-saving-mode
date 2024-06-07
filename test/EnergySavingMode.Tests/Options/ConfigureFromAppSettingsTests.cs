using EnergySavingMode.Options;
using FluentAssertions;

namespace EnergySavingMode.Tests.Options;

public class ConfigureFromAppSettingsTests
{
	public class Configure
	{
		[Fact]
		public void Should_read_valid_schedule()
		{
			// Arrange
			Settings settings = new()
			{
				Enabled =
				{
					Mon = ["00:00-07:59", "20:00-23:59"],
				}
			};

			ConfigureFromAppSettings sut = new (Microsoft.Extensions.Options.Options.Create(settings));

			// Act
			Configuration result = new();
			sut.Configure(result);

			// Assert
			result.Schedule.Should().ContainKey(DayOfWeek.Monday);
			result.Schedule[DayOfWeek.Monday].Should().Equal(
					new TimeRange(new(0, 0), new(7, 59)),
					new TimeRange(new(20, 0), new(23, 59))
				);
		}

		[Fact]
		public void Should_read_valid_week_schedule()
		{
			// Arrange
			Settings settings = new()
			{
				Enabled =
				{
					Mon = ["00:00-23:59"],
					Tue = ["00:00-23:59"],
					Wed = ["00:00-23:59"],
					Thu = ["00:00-23:59"],
					Fri = ["00:00-23:59"],
					Sat = ["00:00-23:59"],
					Sun = ["00:00-23:59"],
				}
			};

			ConfigureFromAppSettings sut = new (Microsoft.Extensions.Options.Options.Create(settings));

			// Act
			Configuration result = new();
			sut.Configure(result);

			// Assert
			foreach (var dayofWeek in Enum.GetValues<DayOfWeek>())
			{
				result.Schedule.Should().ContainKey(
					dayofWeek,
					because: "it should contain day of week {0}",
					dayofWeek.ToString());
				result.Schedule[dayofWeek].Should().Equal(
					[new TimeRange(new(0, 0), new(23, 59))],
					"it should be converted from settings for {0}",
					dayofWeek.ToString()
				);
			}
		}
	}
}
