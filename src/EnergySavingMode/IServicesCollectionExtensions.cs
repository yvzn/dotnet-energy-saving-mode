using EnergySavingMode.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
			services.ConfigureOptions<Options.ConfigureFromAppSettings>();

			return services;
		}

		private static IServiceCollection AddEnergySavingModeServices(
			this IServiceCollection services)
		{
			services.AddScoped<IEnergySavingModeStatus, Status>();

			return services;
		}
	}
}

