using System.Diagnostics;

namespace EnergySavingMode.Services;

internal class CompactTimeline(Timeline timeline)
{
	public IEnumerable<EventOccurence> GetNextEvents(DateTime startingDateTime)
	{
		EventOccurence? previous = default;

		var enumerator = timeline.GetNextEventOccurences(startingDateTime).GetEnumerator();
		while(enumerator.MoveNext())
		{
			var current = enumerator.Current;

			if (previous is null)
			{
				previous = current;
				continue;
			}

			if (!AreContiguous(previous, current))
			{
				yield return previous;
				previous = current;
				continue;
			}

			// skip the 2 contiguous events
			if (enumerator.MoveNext())
			{
				current = enumerator.Current;
				previous = current;
				continue;
			}

			yield break;
		}
	}

	private static bool AreContiguous(EventOccurence earlierEvent, EventOccurence laterEvent)
	{
		Debug.Assert(earlierEvent.DateTime < laterEvent.DateTime);

		return laterEvent.DateTime - earlierEvent.DateTime <= TimeSpan.FromMinutes(1);
	}
}
