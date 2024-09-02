namespace EnergySavingMode.Services;

internal class EventBroadcast : IEnergySavingModeEvents
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

	private static Task InvokeCallbacksAsync(ICollection<EventCallbackAsync> eventCallbacks, CancellationToken cancellationToken)
	{
		// do not wait for completion and return immediately
		_ = Task.WhenAll(eventCallbacks.Select(x => x.Invoke(cancellationToken)));

		return Task.CompletedTask;
	}
}
