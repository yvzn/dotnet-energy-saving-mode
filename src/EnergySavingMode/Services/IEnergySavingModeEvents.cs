namespace EnergySavingMode.Services;

public interface IEnergySavingModeEvents
{
	public void OnEnabled(EventCallbackAsync callbackAsync);

	public void OnDisabled(EventCallbackAsync callbackAsync);
}

public delegate Task EventCallbackAsync();
