using System.Diagnostics.CodeAnalysis;

using DDS.Tools.Enumerators;
using DDS.Tools.Interfaces.Models;
using DDS.Tools.Interfaces.Services;
using DDS.Tools.Models;
using DDS.Tools.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DDS.Tools.Extensions;

/// <summary>
/// The service collection extensions class.
/// </summary>
[SuppressMessage("Style", "IDE0058", Justification = "Not relevant here.")]
internal static class ServiceCollectionExtensions
{
	/// <summary>
	/// Registers the application services to the service collection.
	/// </summary>
	/// <param name="services">The service collection to enrich.</param>
	/// <param name="environment">The host environment instance to use.</param>
	/// <returns>The enriched service collection.</returns>	
	internal static IServiceCollection RegisterServices(this IServiceCollection services, IHostEnvironment environment)
	{
		services.RegisterLoggerService(environment);

		services.AddSingleton<DdsEncoder>();
		services.AddSingleton<DdsDecoder>();
		services.AddSingleton<ITodoService, TodoService>();

		services.AddKeyedTransient<IImageModel, PngImageModel>(ImageType.PNG);
		services.AddKeyedTransient<IImageModel, DdsImageModel>(ImageType.DDS);

		return services;
	}

	[SuppressMessage("Interoperability", "CA1416", Justification = "Validate platform compatibility done.")]
	private static IServiceCollection RegisterLoggerService(this IServiceCollection services, IHostEnvironment environment)
	{
		services.TryAddSingleton(typeof(ILoggerService<>), typeof(LoggerService<>));

		services.AddLogging(configure =>
		{
			configure.ClearProviders();
			configure.AddEventLog(settings => settings.SourceName = environment.ApplicationName);

			if (environment.IsDevelopment())
			{
				configure.SetMinimumLevel(LogLevel.Debug);
			}
			else
			{
				configure.SetMinimumLevel(LogLevel.Warning);
			}
		});

		return services;
	}
}
