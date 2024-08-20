using EnergySavingMode.Services;
using FluentAssertions;
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

            var timeline = new CompactTimeline(new(Opts.Create(configuration)));

            var now = new DateTime(2024, 6, 10, 11, 30, 0, 0, DateTimeKind.Local);
            var timeProvider = new FakeTimeProvider(now);

            EventTrigger sut = new(timeline, timeProvider);

            var isEnabledEventTriggered = false;
            void sut_Enabled(object? _, EventArgs? __) { isEnabledEventTriggered = true; }

            sut.Enabled += sut_Enabled;

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

            var timeline = new CompactTimeline(new(Opts.Create(configuration)));

            var now = new DateTime(2024, 6, 10, 10, 30, 0, 0, DateTimeKind.Local);
            var timeProvider = new FakeTimeProvider(now);

            EventTrigger sut = new(timeline, timeProvider);

            var isDisabledEventTriggered = false;
            void sut_Disabled(object? _, EventArgs? __) { isDisabledEventTriggered = true; }

            sut.Disabled += sut_Disabled;

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

            var timeline = new CompactTimeline(new(Opts.Create(configuration)));

            var now = new DateTime(2024, 6, 10, 10, 30, 0, 0, DateTimeKind.Local);
            var timeProvider = new FakeTimeProvider(now);

            EventTrigger sut = new(timeline, timeProvider);

            var cts = new CancellationTokenSource();

            // Act
            await sut.StartAsync(cts.Token);
            await cts.CancelAsync();

            // Assert
            sut.ExecuteTask?.Status.Should().BeOneOf(TaskStatus.WaitingForActivation, TaskStatus.RanToCompletion);
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

            var timeline = new CompactTimeline(new(Opts.Create(configuration)));

            var now = new DateTime(2024, 6, 10, 10, 59, 59, 999, DateTimeKind.Local);
            var timeProvider = new FakeTimeProvider(now);

            EventTrigger sut = new(timeline, timeProvider);

            var isEnabledEventTriggered = false;
            void sut_Enabled(object? _, EventArgs? __) { 
                isEnabledEventTriggered = true; 
            }

            sut.Enabled += sut_Enabled;

            var cts = new CancellationTokenSource();

            // Act
            await sut.StartAsync(cts.Token);
            await Task.Delay(millisecondsDelay: 100);
            await cts.CancelAsync();

            // Assert
            isEnabledEventTriggered.Should().BeTrue();
        }
    }
}
