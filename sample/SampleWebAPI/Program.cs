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
		timestamp = energySavingModeStatus.Timestamp
	};
	return status;
});

app.Run();

