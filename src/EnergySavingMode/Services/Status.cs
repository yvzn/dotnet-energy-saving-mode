using EnergySavingMode.Options;
using Microsoft.Extensions.Options;

namespace EnergySavingMode.Services;

internal class Status : IEnergySavingModeStatus
{
	public Status(IOptions<Configuration> options)
	{
	}

	public string Timestamp => DateTimeOffset.UtcNow.ToString("o");
}
