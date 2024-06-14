using EnergySavingMode;
using EnergySavingMode.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEnergySavingMode(
	builder.Configuration.GetSection("EnergySavingMode"));

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

