using EnergySavingMode;
using EnergySavingMode.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEnergySavingMode(
	enabled =>
	{
		enabled[DayOfWeek.Monday] =
			[new(new(0, 0), new(07, 59)), new(new(20, 00), new(23, 59))];
		enabled[DayOfWeek.Tuesday] =
			[new(new(0, 0), new(07, 59)), new(new(20, 00), new(23, 59))];
		enabled[DayOfWeek.Wednesday] =
			[new(new(0, 0), new(07, 59)), new(new(20, 00), new(23, 59))];
		enabled[DayOfWeek.Thursday] =
			[new(new(0, 0), new(07, 59)), new(new(20, 00), new(23, 59))];
		enabled[DayOfWeek.Friday] =
			[new(new(0, 0), new(07, 59)), new(new(20, 00), new(23, 59))];
		enabled[DayOfWeek.Saturday] =
			[new(new(0, 0), new(23, 59))];
		enabled[DayOfWeek.Sunday] =
			[new(new(0, 0), new(23, 59))];
	}
);

builder.Services.AddHostedService<EnergySavingModeEventHandler>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", (IEnergySavingModeStatus energySavingModeStatus) =>
{
	var status = new
	{
		energySavingMode = energySavingModeStatus.Current.IsEnabled ? "Enabled" : "Disabled",
		timestamp = energySavingModeStatus.Current.Timestamp.ToString("o")
	};
	return status;
});

app.Run();




internal class EnergySavingModeEventHandler(
	IEnergySavingModeEvents energySavingMode,
	ILogger<EnergySavingModeEventHandler> logger) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken _)
	{
		await Task.Yield();

		energySavingMode.OnEnabled(EnergySavingMode_Enabled);
		energySavingMode.OnDisabled(EnergySavingMode_Disabled);
	}

	private Task EnergySavingMode_Enabled(CancellationToken _)
	{
		logger.LogInformation("Energy Saving Mode is Enabled...");
		return Task.CompletedTask;
	}

	private Task EnergySavingMode_Disabled(CancellationToken _)
	{
		logger.LogInformation("Energy Saving Mode is Disabled...");
		return Task.CompletedTask;
	}
}
