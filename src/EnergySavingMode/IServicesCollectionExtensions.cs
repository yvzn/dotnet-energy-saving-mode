using EnergySavingMode.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EnergySavingMode
{
	public static class IServicesCollectionExtensions
	{
		public static IServiceCollection AddEnergySavingMode(
			this IServiceCollection services,
			IConfigurationSection configurationSection)
		{
			services.AddEnergySavingModeOptions(configurationSection);
			services.AddEnergySavingModeServices();

			return services;
		}

		public static IServiceCollection AddEnergySavingMode(
			this IServiceCollection services,
			Action<IDictionary<DayOfWeek, ICollection<Options.TimeRange>>> configureSchedule)
		{
			services.AddEnergySavingModeOptions(configureSchedule);
			services.AddEnergySavingModeServices();

			return services;
		}

		private static IServiceCollection AddEnergySavingModeOptions(
			this IServiceCollection services,
			IConfigurationSection configurationSection)
		{
			services.AddOptions();

			services.Configure<Options.Settings>(configurationSection);
			services.ConfigureOptions<Options.ConfigureFromAppSettings>();

			services.AddEnergySavingModeOptionValidation();

			return services;
		}

		private static IServiceCollection AddEnergySavingModeOptions(
			this IServiceCollection services,
			Action<IDictionary<DayOfWeek, ICollection<Options.TimeRange>>> configureSchedule)
		{
			services.AddOptions();

			services.AddOptions<Options.Settings>();
			services.Configure<Options.Configuration>(options => configureSchedule(options.Schedule));

			services.AddEnergySavingModeOptionValidation();

			return services;
		}

		private static IServiceCollection AddEnergySavingModeOptionValidation(
			this IServiceCollection services)
		{
			services.AddSingleton<IValidateOptions<Options.Settings>, Options.SettingsValidation>();
			services.AddSingleton<IValidateOptions<Options.Configuration>, Options.ConfigurationValidation>();

			services.AddOptionsWithValidateOnStart<Options.Settings>();
			services.AddOptionsWithValidateOnStart<Options.Configuration>();

			return services;
		}

		private static IServiceCollection AddEnergySavingModeServices(
			this IServiceCollection services)
		{
			services.AddScoped<IEnergySavingModeStatus, Status>();
			services.AddSingleton<EventBroadcast>();
			services.AddSingleton<IEnergySavingModeEvents>(x => x.GetService<EventBroadcast>()!);
			services.AddSingleton<Timeline>();
			services.AddSingleton<ITimeline, CompactTimeline>();
			services.AddSingleton(TimeProvider.System);

			services.AddHostedService<EventTrigger>();
			services.AddHostedService<EventScheduleLogger>();

			return services;
		}
	}
}

