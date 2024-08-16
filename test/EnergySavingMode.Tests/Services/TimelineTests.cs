using EnergySavingMode.Services;
using FluentAssertions;
using Configuration = EnergySavingMode.Options.Configuration;
using Opts = Microsoft.Extensions.Options.Options;

namespace EnergySavingMode.Tests.Services;

public class TimelineTests
{
	public class GetNextEventSeries
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
			var result = sut.GetNextEventSeries(dateTime);

			// Assert
			result.FirstOrDefault().Should().Be(
				new SeriesEvent(DayOfWeek.Monday, new(12, 0), EventType.Start, daysElapsed: 0)
				);
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
			var result = sut.GetNextEventSeries(dateTime);

			// Assert
			result.FirstOrDefault().Should().Be(
				new SeriesEvent(DayOfWeek.Monday, new(15, 0), EventType.End, daysElapsed: 0)
			);
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
			var result = sut.GetNextEventSeries(dateTime);

			// Assert
			result.Should().ContainInConsecutiveOrder(
				new SeriesEvent(DayOfWeek.Monday, new(12, 0), EventType.Start, daysElapsed: 0),
				new SeriesEvent(DayOfWeek.Monday, new(15, 0), EventType.End, 0),
				new SeriesEvent(DayOfWeek.Monday, new(18, 0), EventType.Start, 0),
				new SeriesEvent(DayOfWeek.Monday, new(21, 0), EventType.End, 0)
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
			var result = sut.GetNextEventSeries(dateTime);

			// Assert
			result.Should().ContainInConsecutiveOrder(
				new SeriesEvent(DayOfWeek.Monday, new(12, 0), EventType.Start, daysElapsed: 0),
				new SeriesEvent(DayOfWeek.Monday, new(15, 0), EventType.End, 0),
				new SeriesEvent(DayOfWeek.Tuesday, new(12, 0), EventType.Start, 1),
				new SeriesEvent(DayOfWeek.Tuesday, new(15, 0), EventType.End, 1)
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
			var result = sut.GetNextEventSeries(dateTime);

			// Assert
			result.Should().ContainInConsecutiveOrder(
				new SeriesEvent(DayOfWeek.Monday, new(12, 0), EventType.Start, daysElapsed: 0),
				new SeriesEvent(DayOfWeek.Monday, new(15, 0), EventType.End, 0),
				new SeriesEvent(DayOfWeek.Monday, new(12, 0), EventType.Start, 7),
				new SeriesEvent(DayOfWeek.Monday, new(15, 0), EventType.End, 7)
				);
		}

		[Fact]
		public void Should_handle_empty_schedule()
		{
			// Arrange
			var configuration = new Configuration();

			Timeline sut = new(Opts.Create(configuration));

			var dateTime = new DateTime(2024, 6, 10, 12, 0, 0, DateTimeKind.Local);

			// Act
			var result = sut.GetNextEventSeries(dateTime);

			// Assert
			result.Should().BeEmpty();
		}
	}

	public class GetNextConcreteEvents()
	{
		[Fact]
		public void Should_return_next_events()
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
			var result = sut.GetNextEventOccurences(dateTime);

			// Assert
			result.Should().ContainInConsecutiveOrder(
				new EventOccurence(new(2024, 6, 10, 12, 0, 0, DateTimeKind.Local), EventType.Start),
				new EventOccurence(new(2024, 6, 10, 15, 0, 0, DateTimeKind.Local), EventType.End),
				new EventOccurence(new(2024, 6, 11, 12, 0, 0, DateTimeKind.Local), EventType.Start),
				new EventOccurence(new(2024, 6, 11, 15, 0, 0, DateTimeKind.Local), EventType.End),
				new EventOccurence(new(2024, 6, 17, 12, 0, 0, DateTimeKind.Local), EventType.Start),
				new EventOccurence(new(2024, 6, 17, 15, 0, 0, DateTimeKind.Local), EventType.End)
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
			var result = sut.GetNextEventOccurences(dateTime);

			// Assert
			result.Should().ContainInConsecutiveOrder(
				new EventOccurence(new(2024, 6, 10, 12, 0, 0, DateTimeKind.Local), EventType.Start),
				new EventOccurence(new(2024, 6, 10, 15, 0, 0, DateTimeKind.Local), EventType.End),
				new EventOccurence(new(2024, 6, 17, 12, 0, 0, DateTimeKind.Local), EventType.Start),
				new EventOccurence(new(2024, 6, 17, 15, 0, 0, DateTimeKind.Local), EventType.End)
				);
		}
	}
}
