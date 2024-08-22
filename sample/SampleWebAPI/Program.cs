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

internal class EnergySavingModeEventHandler(IEnergySavingModeEvents energySavingMode, ILogger<EnergySavingModeEventHandler> logger) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await Task.Yield();

		energySavingMode.Enabled += EnergySavingMode_Enabled;
		energySavingMode.Disabled += EnergySavingMode_Disabled;

		while (!stoppingToken.IsCancellationRequested) {
			await Task.Delay(1_000, stoppingToken);
		}

		energySavingMode.Enabled -= EnergySavingMode_Enabled;
		energySavingMode.Disabled -= EnergySavingMode_Disabled;
	}

	private void EnergySavingMode_Enabled(object? sender, EventArgs e)
	{
		Console.WriteLine("Energy Saving Mode is Enabled...");
	}

	private void EnergySavingMode_Disabled(object? sender, EventArgs e)
	{
		Console.WriteLine("Energy Saving Mode is Disabled...");
	}
}
