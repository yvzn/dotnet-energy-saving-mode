namespace EnergySavingMode.Tests.Services;

internal class FakeTimeProvider(DateTime startingDateTime) : TimeProvider()
{
	private readonly DateTimeOffset mockStartingDateTime = new(startingDateTime);

	private readonly Lazy<DateTimeOffset> actualStartingDateTime = new(() => DateTimeOffset.UtcNow);

	public override DateTimeOffset GetUtcNow()
	{
		var elapsed = DateTimeOffset.UtcNow - actualStartingDateTime.Value;
		var mockNow = mockStartingDateTime.Add(elapsed);
Console.WriteLine($"now: {mockNow:dd/MM:HH:mm:ss:fff}");
		return mockNow.ToUniversalTime();
	}
}
