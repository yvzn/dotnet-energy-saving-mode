namespace EnergySavingMode.Services;

internal class Status(Timeline timeline, TimeProvider timeProvider) : IEnergySavingModeStatus
{
	public StatusInfo Current => GetCurrentStatus();

	private StatusInfo GetCurrentStatus()
	{
		var now = timeProvider.GetUtcNow().LocalDateTime;
		var next = timeline.Next(now);

		if (!next.Any())
		{
			return new(false, now);
		}

		var isEnabled = next.First().Type == TimelineEventType.End;
		return new(isEnabled, now);
	}
}

