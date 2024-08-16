namespace EnergySavingMode.Services;

internal class Status(CompactTimeline timeline, TimeProvider timeProvider) : IEnergySavingModeStatus
{
	public StatusInfo Current => GetCurrentStatus();

	private StatusInfo GetCurrentStatus()
	{
		var now = timeProvider.GetUtcNow().LocalDateTime;
		var next = timeline.GetNextEvents(now);

		if (!next.Any())
		{
			return new(false, now);
		}

		var isEnabled = next.First().Type == EventType.End;
		return new(isEnabled, now);
	}
}

