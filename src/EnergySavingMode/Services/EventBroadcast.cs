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

	internal async Task SendAsync(bool isEnergySavingModeEnabled)
	{
		var eventCallbacks = isEnergySavingModeEnabled ? enabledEventCallbacks : disabledEventCallbacks;
		await InvokeCallbacksAsync(eventCallbacks);
	}

	private static Task InvokeCallbacksAsync(ICollection<EventCallbackAsync> eventCallbacks)
	{
		// do not wait for completion and return immediately
		_ = Task.WhenAll(eventCallbacks.Select(x => x.Invoke()));

		return Task.CompletedTask;
	}
}
