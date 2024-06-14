using EnergySavingMode.Services;
using FluentAssertions;
using Configuration = EnergySavingMode.Options.Configuration;
using Opts = Microsoft.Extensions.Options.Options;

namespace EnergySavingMode.Tests.Services;

public class TimelineTests
{
	public class Next
	{
		[Fact]
		public void Should_return_next_start_event_in_sequence()
		{
			// Arrange
			var configuration = new Configuration()
			{
				Schedule =
				{
					[DayOfWeek.Monday] = [new (new (12, 0), new (15, 0))]
				}
			};

			Timeline sut = new(Opts.Create(configuration));

			var dateTime = new DateTime(2024, 6, 10, 0, 0, 0, DateTimeKind.Local);

			// Act
			var result = sut.Next(dateTime);

			// Assert
			result.FirstOrDefault().Should().Be(new TimelineEvent(DayOfWeek.Monday, new(12, 0), TimelineEventType.Start));
		}

		[Fact]
		public void Should_return_next_end_event_in_sequence()
		{
			// Arrange
			var configuration = new Configuration()
			{
				Schedule =
				{
					[DayOfWeek.Monday] = [new (new (12, 0), new (15, 0))]
				}
			};

			Timeline sut = new(Opts.Create(configuration));

			var dateTime = new DateTime(2024, 6, 10, 14, 0, 0, DateTimeKind.Local);

			// Act
			var result = sut.Next(dateTime);

			// Assert
			result.FirstOrDefault().Should().Be(new TimelineEvent(DayOfWeek.Monday, new(15, 0), TimelineEventType.End));
		}

		[Fact]
		public void Should_return_next_events_in_the_same_day()
		{
			// Arrange
			var configuration = new Configuration()
			{
				Schedule =
				{
					[DayOfWeek.Monday] = [new (new (12, 0), new (15, 0)), new (new (18, 0), new (21, 0))]
				}
			};

			Timeline sut = new(Opts.Create(configuration));

			var dateTime = new DateTime(2024, 6, 10, 12, 0, 0, DateTimeKind.Local);

			// Act
			var result = sut.Next(dateTime);

			// Assert
			result.Should().ContainInConsecutiveOrder(
				new TimelineEvent(DayOfWeek.Monday, new(12, 0), TimelineEventType.Start),
				new TimelineEvent(DayOfWeek.Monday, new(15, 0), TimelineEventType.End),
				new TimelineEvent(DayOfWeek.Monday, new(18, 0), TimelineEventType.Start),
				new TimelineEvent(DayOfWeek.Monday, new(21, 0), TimelineEventType.End)
				);
		}

		[Fact]
		public void Should_return_next_events_in_the_following_days()
		{
			// Arrange
			var configuration = new Configuration()
			{
				Schedule =
				{
					[DayOfWeek.Monday] = [new (new (12, 0), new (15, 0))],
					[DayOfWeek.Tuesday] = [new (new (12, 0), new (15, 0))]
				}
			};

			Timeline sut = new(Opts.Create(configuration));

			var dateTime = new DateTime(2024, 6, 10, 12, 0, 0, DateTimeKind.Local);

			// Act
			var result = sut.Next(dateTime);

			// Assert
			result.Should().ContainInConsecutiveOrder(
				new TimelineEvent(DayOfWeek.Monday, new(12, 0), TimelineEventType.Start),
				new TimelineEvent(DayOfWeek.Monday, new(15, 0), TimelineEventType.End),
				new TimelineEvent(DayOfWeek.Tuesday, new(12, 0), TimelineEventType.Start),
				new TimelineEvent(DayOfWeek.Tuesday, new(15, 0), TimelineEventType.End)
				);
		}

		[Fact]
		public void Should_loop_over_weeks()
		{
			// Arrange
			var configuration = new Configuration()
			{
				Schedule =
				{
					[DayOfWeek.Monday] = [new (new (12, 0), new (15, 0))]
				}
			};

			Timeline sut = new(Opts.Create(configuration));

			var dateTime = new DateTime(2024, 6, 10, 12, 0, 0, DateTimeKind.Local);

			// Act
			var result = sut.Next(dateTime);

			// Assert
			result.Should().ContainInConsecutiveOrder(
				new TimelineEvent(DayOfWeek.Monday, new(12, 0), TimelineEventType.Start),
				new TimelineEvent(DayOfWeek.Monday, new(15, 0), TimelineEventType.End),
				new TimelineEvent(DayOfWeek.Monday, new(12, 0), TimelineEventType.Start),
				new TimelineEvent(DayOfWeek.Monday, new(15, 0), TimelineEventType.End)
				);
		}

		[Fact]
		public void Should_loop_handle_empty_schedule()
		{
			// Arrange
			var configuration = new Configuration();

			Timeline sut = new(Opts.Create(configuration));

			var dateTime = new DateTime(2024, 6, 10, 12, 0, 0, DateTimeKind.Local);

			// Act
			var result = sut.Next(dateTime);

			// Assert
			result.Should().BeEmpty();
		}
	}
}
