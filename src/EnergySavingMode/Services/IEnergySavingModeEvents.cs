namespace EnergySavingMode.Services;

public interface IEnergySavingModeEvents
{
	public event EventHandler Enabled;
	public event EventHandler Disabled;
}
