using EnergySavingMode.Services;
using FluentAssertions;

namespace EnergySavingMode.Tests.Services;

public class EventBroadcastTests
{
	public class OnEnabled
	{
		[Fact]
		public async Task Should_invoke_callback_when_enabled()
		{
			// Arrange
			var sut = new EventBroadcast();

			var isEnabledEventTriggered = false;
			Task OnEnabledCallback() => Task.FromResult(isEnabledEventTriggered = true);

			sut.OnEnabled(OnEnabledCallback);

			// Act
			await sut.SendAsync(isEnergySavingModeEnabled: true);

			// Assert
			isEnabledEventTriggered.Should().BeTrue();
		}

		[Fact]
		public async Task Should_invoke_multiple_callbacks_when_enabled()
		{
			// Arrange
			var sut = new EventBroadcast();

			var isEnabledEventTriggered_1 = false;
			Task OnEnabledCallback_1() => Task.FromResult(isEnabledEventTriggered_1 = true);
			var isEnabledEventTriggered_2 = false;
			Task OnEnabledCallback_2() => Task.FromResult(isEnabledEventTriggered_2 = true);

			sut.OnEnabled(OnEnabledCallback_1);
			sut.OnEnabled(OnEnabledCallback_2);

			// Act
			await sut.SendAsync(isEnergySavingModeEnabled: true);

			// Assert
			isEnabledEventTriggered_1.Should().BeTrue();
			isEnabledEventTriggered_2.Should().BeTrue();
		}
	}

	public class OnDisabled
	{
		[Fact]
		public async Task Should_invoke_callback_when_disabled()
		{
			// Arrange
			var sut = new EventBroadcast();

			var isDisabledEventTriggered = false;
			Task OnDisabledCallback() => Task.FromResult(isDisabledEventTriggered = true);

			sut.OnDisabled(OnDisabledCallback);

			// Act
			await sut.SendAsync(isEnergySavingModeEnabled: false);

			// Assert
			isDisabledEventTriggered.Should().BeTrue();
		}
	}
}
