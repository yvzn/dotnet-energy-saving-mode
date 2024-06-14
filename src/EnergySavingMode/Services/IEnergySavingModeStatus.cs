namespace EnergySavingMode.Services;

public interface IEnergySavingModeStatus
{
	public StatusInfo Current { get; }
}

public record StatusInfo(bool IsEnabled, DateTime Timestamp);
