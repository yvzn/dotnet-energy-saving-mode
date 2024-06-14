using EnergySavingMode.Options;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace EnergySavingMode.Tests.Options;

public class ConfigurationValidationTests
{
	public class Validate()
	{
		[Fact]
		public void Should_succeed_when_valid_schedule()
		{
			// Arrange
			Configuration configuration = new()
			{
				Schedule =
				{
					[DayOfWeek.Monday] = [new (new (12, 0), new (15, 0))]
				}
			};

			ConfigurationValidation sut = new();

			// Act
			var result = sut.Validate(default, configuration);

			// Assert
			result.Should().Be(ValidateOptionsResult.Success);
		}

		[Fact]
		public void Should_fail_when_start_time_after_end_time()
		{
			// Arrange
			Configuration configuration = new()
			{
				Schedule =
				{
					[DayOfWeek.Monday] = [new (new (15, 0), new (12, 0))]
				}
			};

			ConfigurationValidation sut = new();

			// Act
			var result = sut.Validate(default, configuration);

			// Assert
			result.Failures.Should().ContainMatch(
				"*15:00*",
				because: "'15:00-12:00' is not valid, start time should be before end time");
		}

		[Fact]
		public void Should_fail_when_start_time_equals_end_time()
		{
			// Arrange
			Configuration configuration = new()
			{
				Schedule =
				{
					[DayOfWeek.Monday] = [new (new (12, 0), new (12, 0))]
				}
			};

			ConfigurationValidation sut = new();

			// Act
			var result = sut.Validate(default, configuration);

			// Assert
			result.Failures.Should().ContainMatch(
				"*12:00*",
				because: "'12:00-12:00' is not valid, start time should be before end time");
		}

		[Fact]
		public void Should_fail_when_time_ranges_overlap()
		{
			// Arrange
			Configuration configuration = new()
			{
				Schedule =
				{
					[DayOfWeek.Monday] = [new (new (12, 0), new (15, 0)), new(new(13, 0), new(16, 0))]
				}
			};

			ConfigurationValidation sut = new();

			// Act
			var result = sut.Validate(default, configuration);

			// Assert
			result.Failures.Should().ContainMatch(
				"*13:00*",
				because: "The '13:00-16:00' time range overlaps another time range");
		}

		[Fact]
		public void Should_validate_all_days_of_week()
		{
			// Arrange
			Configuration configuration = new()
			{
				Schedule =
				{
					[DayOfWeek.Monday] = [new (new (1, 0), new (1, 0))],
					[DayOfWeek.Tuesday] = [new (new (2, 0), new (2, 0))],
					[DayOfWeek.Wednesday] = [new (new (3, 0), new (3, 0))],
					[DayOfWeek.Thursday] = [new (new (4, 0), new (4, 0))],
					[DayOfWeek.Friday] = [new (new (5, 0), new (5, 0))],
					[DayOfWeek.Saturday] = [new (new (6, 0), new (6, 0))],
					[DayOfWeek.Sunday] = [new (new (7, 0), new (7, 0))],
				}
			};

			ConfigurationValidation sut = new();

			// Act
			var result = sut.Validate(default, configuration);

			// Assert
			result.Failures.Should().ContainMatch("*01:00*", because: "Monday schedule is not valid");
			result.Failures.Should().ContainMatch("*02:00*", because: "Tuesday schedule is not valid");
			result.Failures.Should().ContainMatch("*03:00*", because: "Wednesday schedule is not valid");
			result.Failures.Should().ContainMatch("*04:00*", because: "Thursday schedule is not valid");
			result.Failures.Should().ContainMatch("*05:00*", because: "Friday schedule is not valid");
			result.Failures.Should().ContainMatch("*06:00*", because: "Saturday schedule is not valid");
			result.Failures.Should().ContainMatch("*07:00*", because: "Sunday schedule is not valid");
		}
	}
}
