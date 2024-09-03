using Microsoft.Extensions.Logging;

namespace EnergySavingMode.Services;

public class EventBroadcast(ILogger<EventBroadcast> logger) : IEnergySavingModeEvents
{
	private readonly ICollection<EventCallbackAsync> enabledEventCallbacks = [];
	private readonly ICollection<EventCallbackAsync> disabledEventCallbacks = [];

	public void OnEnabled(EventCallbackAsync callbackAsync)
	{
		enabledEventCallbacks.Add(callbackAsync);
	}

	public void OnDisabled(EventCallbackAsync callbackAsync)
	{
		disabledEventCallbacks.Add(callbackAsync);
	}

	internal async Task SendAsync(bool isEnergySavingModeEnabled, CancellationToken cancellationToken)
	{
		var eventCallbacks = isEnergySavingModeEnabled ? enabledEventCallbacks : disabledEventCallbacks;
		await InvokeCallbacksAsync(eventCallbacks, cancellationToken);
	}

	private Task InvokeCallbacksAsync(ICollection<EventCallbackAsync> eventCallbacks, CancellationToken cancellationToken)
	{
		// do not wait for completion and return immediately
		_ = Task.WhenAll(eventCallbacks.Select(x => InvokeCallbackAsync(x, cancellationToken)));

		return Task.CompletedTask;
	}

	private async Task InvokeCallbackAsync(EventCallbackAsync eventCallback, CancellationToken cancellationToken)
	{
		try {
			await eventCallback.Invoke(cancellationToken);
		} catch (Exception ex) {
			logger.LogError(ex, "Error while invoking callback on EnergySavingMode event");
		}
	}
}
