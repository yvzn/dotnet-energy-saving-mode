using FluentAssertions;
using Microsoft.Extensions.Options;
using System;

namespace EnergySavingMode.Options;

public class SettingsValidationTests
{
	public class Validate()
	{
		[Fact]
		public void Should_succeed_when_valid_schedule()
		{
			// Arrange
			Settings settings = new()
			{
				Enabled =
				{
					Mon = ["00:00-07:59", "20:00-23:59"],
				}
			};

			SettingsValidation sut = new();

			// Act
			var result = sut.Validate(default, settings);

			// Assert
			result.Should().Be(ValidateOptionsResult.Success);
		}

		[Fact]
		public void Should_succeed_when_empty_schedule()
		{
			// Arrange
			Settings settings = new()
			{
				Enabled =
				{
					Mon = [],
				}
			};

			SettingsValidation sut = new();

			// Act
			var result = sut.Validate(default, settings);

			// Assert
			result.Should().Be(ValidateOptionsResult.Success);
		}

		[Fact]
		public void Should_succeed_when_valid_week_schedule()
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

			SettingsValidation sut = new();

			// Act
			var result = sut.Validate(default, settings);

			// Assert
			result.Should().Be(ValidateOptionsResult.Success);
		}

		[Theory]
		[InlineData("00:00")]
		[InlineData("-00:00")]
		[InlineData("00:00-")]
		public void Should_fail_when_entry_is_not_a_range(string entry)
		{
			// Arrange
			Settings settings = new()
			{
				Enabled =
				{
					Mon = [entry],
				}
			};

			SettingsValidation sut = new();

			// Act
			var result = sut.Validate(default, settings);

			// Assert
			result.Failures.Should().ContainMatch(
				$"*{entry}*",
				because: "'{0}' does not have a start time and an end time",
				entry);
		}

		[Theory]
		[InlineData("00:00-25:00")]
		[InlineData("25:00-00:00")]
		[InlineData("00:00-00:70")]
		[InlineData("00:00-abcd")]
		public void Should_fail_when_entry_has_an_invalid_time(string entry)
		{
			// Arrange
			Settings settings = new()
			{
				Enabled =
				{
					Mon = [entry],
				}
			};

			SettingsValidation sut = new();

			// Act
			var result = sut.Validate(default, settings);

			// Assert
			result.Failures.Should().ContainMatch(
				$"*{entry}*",
				because: "'{0}' contains an invalid value",
				entry);
		}

		[Fact]
		public void Should_fail_when_start_time_after_end_time()
		{
			// Arrange
			Settings settings = new()
			{
				Enabled =
				{
					Mon = ["15:00-12:00"],
				}
			};

			SettingsValidation sut = new();

			// Act
			var result = sut.Validate(default, settings);

			// Assert
			result.Failures.Should().ContainMatch(
				"*15:00-12:00*",
				because: "'15:00-12:00' start time is not before end time");
		}


		[Fact]
		public void Should_validate_all_days_of_week()
		{
			// Arrange
			Settings settings = new()
			{
				Enabled =
				{
					Mon = ["Mon"],
					Tue = ["Tue"],
					Wed = ["Wed"],
					Thu = ["Thu"],
					Fri = ["Fri"],
					Sat = ["Sat"],
					Sun = ["Sun"],
				}
			};

			SettingsValidation sut = new();

			// Act
			var result = sut.Validate(default, settings);

			// Assert
			result.Failures.Should().ContainMatch("*Mon*", because: "Mon schedule should be valid");
			result.Failures.Should().ContainMatch("*Tue*", because: "Tue schedule should be valid");
			result.Failures.Should().ContainMatch("*Wed*", because: "Wed schedule should be valid");
			result.Failures.Should().ContainMatch("*Thu*", because: "Thu schedule should be valid");
			result.Failures.Should().ContainMatch("*Fri*", because: "Fri schedule should be valid");
			result.Failures.Should().ContainMatch("*Sat*", because: "Sat schedule should be valid");
			result.Failures.Should().ContainMatch("*Sun*", because: "Sun schedule should be valid");
		}
	}
}
