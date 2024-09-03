using EnergySavingMode;
using EnergySavingMode.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEnergySavingMode(
	builder.Configuration.GetSection("EnergySavingMode"));

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




public partial class Program { }




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
