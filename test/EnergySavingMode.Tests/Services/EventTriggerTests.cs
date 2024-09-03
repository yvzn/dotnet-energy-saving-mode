using EnergySavingMode.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Configuration = EnergySavingMode.Options.Configuration;
using Opts = Microsoft.Extensions.Options.Options;

namespace EnergySavingMode.Tests.Services;

public class EventTriggerTests
{
	public class ExecuteAsync
	{
		[Fact]
		public async Task Should_trigger_enabled_event_on_startup()
		{
			// Arrange
			var configuration = new Configuration()
			{
				Schedule =
				{
					[DayOfWeek.Monday] = [new (new (11, 0), new (12, 0))]
				}
			};

			var timeline = new Timeline(Opts.Create(configuration));

			var now = new DateTime(2024, 6, 10, 11, 30, 0, 0, DateTimeKind.Local);
			var timeProvider = new FakeTimeProvider(now);

			var isEnabledEventTriggered = false;
			Task OnEnabledCallback(CancellationToken _) => Task.FromResult(isEnabledEventTriggered = true);

			var eventBroadcast = new EventBroadcast(Substitute.For<ILogger<EventBroadcast>>());
			eventBroadcast.OnEnabled(OnEnabledCallback);

			EventTrigger sut = new(timeline, timeProvider, eventBroadcast);

			// Act
			await sut.StartAsync(CancellationToken.None);
			await sut.StopAsync(CancellationToken.None);

			// Assert
			isEnabledEventTriggered.Should().BeTrue();
		}

		[Fact]
		public async Task Should_trigger_disabled_event_on_startup()
		{
			// Arrange
			var configuration = new Configuration()
			{
				Schedule =
				{
					[DayOfWeek.Monday] = [new (new (11, 0), new (12, 0))]
				}
			};

			var timeline = new Timeline(Opts.Create(configuration));

			var now = new DateTime(2024, 6, 10, 10, 30, 0, 0, DateTimeKind.Local);
			var timeProvider = new FakeTimeProvider(now);

			var isDisabledEventTriggered = false;
			Task OnDisabledCallback(CancellationToken _) => Task.FromResult(isDisabledEventTriggered = true);

			var eventBroadcast = new EventBroadcast(Substitute.For<ILogger<EventBroadcast>>());
			eventBroadcast.OnDisabled(OnDisabledCallback);

			EventTrigger sut = new(timeline, timeProvider, eventBroadcast);

			// Act
			await sut.StartAsync(CancellationToken.None);
			await sut.StopAsync(CancellationToken.None);

			// Assert
			isDisabledEventTriggered.Should().BeTrue();
		}

		[Fact]
		public async Task Should_stop_when_shutdown()
		{
			// Arrange
			var configuration = new Configuration()
			{
				Schedule =
				{
					[DayOfWeek.Monday] = [new (new (11, 0), new (12, 0))]
				}
			};

			var timeline = new Timeline(Opts.Create(configuration));

			var now = new DateTime(2024, 6, 10, 10, 30, 0, 0, DateTimeKind.Local);
			var timeProvider = new FakeTimeProvider(now);

			var eventBroadcast = new EventBroadcast(Substitute.For<ILogger<EventBroadcast>>());

			EventTrigger sut = new(timeline, timeProvider, eventBroadcast);

			var cts = new CancellationTokenSource();

			// Act
			await sut.StartAsync(cts.Token);
			await cts.CancelAsync();

			// Assert
			sut.ExecuteTask?.Status.Should().BeOneOf(TaskStatus.WaitingForActivation, TaskStatus.RanToCompletion, TaskStatus.Canceled);
		}

		[Fact]
		public async Task Should_trigger_enabled_event_on_schedule()
		{
			// Arrange
			var configuration = new Configuration()
			{
				Schedule =
				{
					[DayOfWeek.Monday] = [new (new (11, 0), new (12, 0))]
				}
			};

			var timeline = new Timeline(Opts.Create(configuration));

			var now = new DateTime(2024, 6, 10, 10, 59, 59, 999, DateTimeKind.Local);
			var timeProvider = new FakeTimeProvider(now);

			var isEnabledEventTriggered = false;
			Task OnEnabledCallback(CancellationToken _) => Task.FromResult(isEnabledEventTriggered = true);

			var eventBroadcast = new EventBroadcast(Substitute.For<ILogger<EventBroadcast>>());
			eventBroadcast.OnEnabled(OnEnabledCallback);

			EventTrigger sut = new(timeline, timeProvider, eventBroadcast);

			var cts = new CancellationTokenSource();

			// Act
			await sut.StartAsync(cts.Token);
			await Task.Delay(millisecondsDelay: 100);
			await cts.CancelAsync();

			// Assert
			isEnabledEventTriggered.Should().BeTrue();
		}

		[Fact]
		public async Task Should_trigger_multiple_events_on_schedule()
		{
			// Arrange
			var configuration = new Configuration()
			{
				Schedule =
				{
					[DayOfWeek.Monday] = [
						new (new (11, 0, 0, 200), new (11, 0, 0, 300)),
						new (new (11, 0, 0, 400), new (11, 0, 0, 500)),
					]
				}
			};

			var timeline = new Timeline(Opts.Create(configuration));

			var now = new DateTime(2024, 6, 17, 10, 59, 59, 999, DateTimeKind.Local);
			var timeProvider = new FakeTimeProvider(now);

			int enabledEventCount = 0;
			Task OnEnabledCallback(CancellationToken _) => Task.FromResult(++enabledEventCount);

			int disabledEventCount = 0;
			Task OnDisabledCallback(CancellationToken _) => Task.FromResult(++disabledEventCount);

			var eventBroadcast = new EventBroadcast(Substitute.For<ILogger<EventBroadcast>>());
			eventBroadcast.OnEnabled(OnEnabledCallback);
			eventBroadcast.OnDisabled(OnDisabledCallback);

			EventTrigger sut = new(timeline, timeProvider, eventBroadcast);

			var cts = new CancellationTokenSource();

			// Act
			await sut.StartAsync(cts.Token);
			await Task.Delay(millisecondsDelay: 800);
			await cts.CancelAsync();

			// Assert
			enabledEventCount.Should().Be(2);
			disabledEventCount.Should().Be(3);
		}
	}
}
