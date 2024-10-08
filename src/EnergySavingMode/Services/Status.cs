namespace EnergySavingMode.Services;

internal class Status(ITimeline timeline, TimeProvider timeProvider) : IEnergySavingModeStatus
{
	public StatusInfo Current => GetCurrentStatus();

	private StatusInfo GetCurrentStatus()
	{
		var now = timeProvider.GetLocalNow().LocalDateTime;
		var next = timeline.GetNextEventOccurences(now);

		if (!next.Any())
		{
			return new(false, now);
		}

		var isEnabled = next.First().Type == EventType.End;
		return new(isEnabled, now);
	}
}
