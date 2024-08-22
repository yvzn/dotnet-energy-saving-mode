namespace EnergySavingMode.Tests.Services;

internal class FakeTimeProvider(DateTime startingDateTime) : TimeProvider()
{
	private readonly DateTimeOffset mockStartingDateTime = new(startingDateTime);

	private readonly Lazy<DateTimeOffset> actualStartingDateTime = new(() => DateTimeOffset.UtcNow);

	public override DateTimeOffset GetUtcNow()
	{
		var elapsed = DateTimeOffset.UtcNow - actualStartingDateTime.Value;
		var mockNow = mockStartingDateTime.Add(elapsed);

		return mockNow.ToUniversalTime();
	}
}
