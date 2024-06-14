using EnergySavingMode.Options;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace EnergySavingMode.Tests.Options;

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
				because: "'{0}' does not have a start time or end time",
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
				because: "'{0}' contains an invalid time value",
				entry);
		}

		[Fact]
		public void Should_validate_all_days_of_week()
		{
			// Arrange
			Settings settings = new()
			{
				Enabled =
				{
					Mon = ["Invalid schedule Mon"],
					Tue = ["Invalid schedule Tue"],
					Wed = ["Invalid schedule Wed"],
					Thu = ["Invalid schedule Thu"],
					Fri = ["Invalid schedule Fri"],
					Sat = ["Invalid schedule Sat"],
					Sun = ["Invalid schedule Sun"],
				}
			};

			SettingsValidation sut = new();

			// Act
			var result = sut.Validate(default, settings);

			// Assert
			result.Failures.Should().ContainMatch("*Mon*", because: "Schedule for Mon is not valid");
			result.Failures.Should().ContainMatch("*Tue*", because: "Schedule for Tue is not valid");
			result.Failures.Should().ContainMatch("*Wed*", because: "Schedule for Wed is not valid");
			result.Failures.Should().ContainMatch("*Thu*", because: "Schedule for Thu is not valid");
			result.Failures.Should().ContainMatch("*Fri*", because: "Schedule for Fri is not valid");
			result.Failures.Should().ContainMatch("*Sat*", because: "Schedule for Sat is not valid");
			result.Failures.Should().ContainMatch("*Sun*", because: "Schedule for Sun is not valid");
		}
	}
}
