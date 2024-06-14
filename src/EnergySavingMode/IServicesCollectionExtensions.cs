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

		private static IServiceCollection AddEnergySavingModeOptions(
			this IServiceCollection services,
			IConfigurationSection configurationSection)
		{
			services.AddOptions();

			services.Configure<Options.Settings>(configurationSection);
			services.AddSingleton<IValidateOptions<Options.Settings>, Options.SettingsValidation>();

			services.ConfigureOptions<Options.ConfigureFromAppSettings>();
			services.AddSingleton<IValidateOptions<Options.Configuration>, Options.ConfigurationValidation>();

			services.AddOptionsWithValidateOnStart<Options.Settings>();
			services.AddOptionsWithValidateOnStart<Options.Configuration>();

			return services;
		}

		private static IServiceCollection AddEnergySavingModeServices(
			this IServiceCollection services)
		{
			services.AddScoped<IEnergySavingModeStatus, Status>();
			services.AddScoped<Timeline>();
			services.AddSingleton(TimeProvider.System);

			return services;
		}
	}
}

