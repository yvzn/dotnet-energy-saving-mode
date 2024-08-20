using EnergySavingMode.Services;
using FluentAssertions;
using Configuration = EnergySavingMode.Options.Configuration;
using Opts = Microsoft.Extensions.Options.Options;

namespace EnergySavingMode.Tests.Services;

public class StatusTests
{
	public class Current
	{
		[Fact]
		public void Should_return_enabled()
		{
			// Arrange
			var configuration = new Configuration()
			{
				Schedule =
				{
					[DayOfWeek.Monday] = [new (new (11, 0), new (12, 0))]
				}
			};

			var timeline = new CompactTimeline(new(Opts.Create(configuration)));

			var now = new DateTime(2024, 6, 10, 11, 30, 0, DateTimeKind.Local);
			var timeProvider = new FakeTimeProvider(now);

			Status sut = new(timeline, timeProvider);

			// Act
			var actual = sut.Current;

			// Assert
			actual.IsEnabled.Should().BeTrue();
			actual.Timestamp.Should().BeCloseTo(now, TimeSpan.FromSeconds(1));
		}

		[Fact]
		public void Should_return_disabled()
		{
			// Arrange
			var configuration = new Configuration()
			{
				Schedule =
				{
					[DayOfWeek.Monday] = [new (new (11, 0), new (12, 0))]
				}
			};

			var timeline = new CompactTimeline(new(Opts.Create(configuration)));

			var now = new DateTime(2024, 6, 10, 13, 0, 0, DateTimeKind.Local);
			var timeProvider = new FakeTimeProvider(now);

			Status sut = new(timeline, timeProvider);

			// Act
			var actual = sut.Current;

			// Assert
			actual.IsEnabled.Should().BeFalse();
			actual.Timestamp.Should().BeCloseTo(now, TimeSpan.FromSeconds(1));
		}
	}
}

internal class FakeTimeProvider(DateTime startingDateTime) : TimeProvider()
{
	private readonly DateTimeOffset mockStartingDateTime = new(startingDateTime);

	private readonly Lazy<DateTimeOffset> actualStartingDateTime = new(() => DateTimeOffset.UtcNow);

	public override DateTimeOffset GetUtcNow()
	{
		var elapsed = DateTimeOffset.UtcNow - actualStartingDateTime.Value;
		var mockNow = mockStartingDateTime.Add(elapsed);
		return mockNow.ToUniversalTime();
	}
}
