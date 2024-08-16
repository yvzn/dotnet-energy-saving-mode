using EnergySavingMode.Services;
using FluentAssertions;
using Configuration = EnergySavingMode.Options.Configuration;
using Opts = Microsoft.Extensions.Options.Options;

namespace EnergySavingMode.Tests.Services;

public class CompactTimelineTests
{
	public class GetNextEvents
	{
		[Fact]
		public void Should_return_next_event_in_sequence()
		{
			// Arrange
			var configuration = new Configuration()
			{
				Schedule =
				{
					[DayOfWeek.Monday] = [new (new (12, 0), new (15, 0))]
				}
			};

			CompactTimeline sut = new(new(Opts.Create(configuration)));

			var dateTime = new DateTime(2024, 6, 10, 14, 0, 0, DateTimeKind.Local);

			// Act
			var result = sut.GetNextEvents(dateTime);

			// Assert
			result.FirstOrDefault().Should().Be(new EventOccurence(new(2024, 6, 10, 15, 0, 0, DateTimeKind.Local), EventType.End));
		}

		[Fact]
		public void Should_ignore_contiguous_events_in_the_same_day()
		{
			// Arrange
			var configuration = new Configuration()
			{
				Schedule =
				{
					[DayOfWeek.Monday] = [new (new (12, 0), new (14, 59)), new(new(15, 0), new(16, 0))]
				}
			};

			CompactTimeline sut = new(new(Opts.Create(configuration)));

			var dateTime = new DateTime(2024, 6, 10, 14, 0, 0, DateTimeKind.Local);

			// Act
			var result = sut.GetNextEvents(dateTime);

			// Assert
			result.FirstOrDefault().Should().Be(new EventOccurence(new(2024, 6, 10, 16, 0, 0, DateTimeKind.Local), EventType.End));
		}

		[Fact]
		public void Should_ignore_contiguous_events_in_consecutive_days()
		{
			// Arrange
			var configuration = new Configuration()
			{
				Schedule =
				{
					[DayOfWeek.Monday] = [new (new (12, 0), new (23, 59))],
					[DayOfWeek.Tuesday] = [new (new (0, 0), new (15, 0))]
				}
			};

			CompactTimeline sut = new(new(Opts.Create(configuration)));

			var dateTime = new DateTime(2024, 6, 10, 14, 0, 0, DateTimeKind.Local);

			// Act
			var result = sut.GetNextEvents(dateTime);

			// Assert
			result.FirstOrDefault().Should().Be(new EventOccurence(new(2024, 6, 11, 15, 0, 0, DateTimeKind.Local), EventType.End));
		}

		[Fact]
		public void Should_ignore_contiguous_events_in_multiple_consecutive_days()
		{
			// Arrange
			var configuration = new Configuration()
			{
				Schedule =
				{
					[DayOfWeek.Monday] = [new (new (12, 0), new (23, 59))],
					[DayOfWeek.Tuesday] = [new (new (0, 0), new (23, 59))],
					[DayOfWeek.Wednesday] = [new (new (0, 0), new (15, 0))]
				}
			};

			CompactTimeline sut = new(new(Opts.Create(configuration)));

			var dateTime = new DateTime(2024, 6, 10, 14, 0, 0, DateTimeKind.Local);

			// Act
			var result = sut.GetNextEvents(dateTime);

			// Assert
			result.FirstOrDefault().Should().Be(new EventOccurence(new(2024, 6, 12, 15, 0, 0, DateTimeKind.Local), EventType.End));
		}

		[Fact]
		public void Should_handle_schedule_with_all_contiguous_events()
		{
			// Arrange
			var configuration = new Configuration()
			{
				Schedule =
				{
					[DayOfWeek.Monday] = [new (new (0, 0), new (23, 59))],
					[DayOfWeek.Tuesday] = [new (new (0, 0), new (23, 59))],
					[DayOfWeek.Wednesday] = [new (new (0, 0), new (23, 59))],
					[DayOfWeek.Thursday] = [new (new (0, 0), new (23, 59))],
					[DayOfWeek.Friday] = [new (new (0, 0), new (23, 59))],
					[DayOfWeek.Saturday] = [new (new (0, 0), new (23, 59))],
					[DayOfWeek.Sunday] = [new (new (0, 0), new (23, 59))],
				}
			};

			CompactTimeline sut = new(new(Opts.Create(configuration)));

			var dateTime = new DateTime(2024, 6, 10, 12, 0, 0, DateTimeKind.Local);

			// Act
			var result = sut.GetNextEvents(dateTime);

			// Assert
			result.Should().BeEmpty();
		}
	}
}
