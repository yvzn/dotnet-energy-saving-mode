namespace EnergySavingMode.Services;

internal interface ITimeline
{
	IEnumerable<EventOccurence> GetNextEventOccurences(DateTime startingDateTime);
}
