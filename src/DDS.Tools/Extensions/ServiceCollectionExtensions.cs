// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

using DDS.Tools.Enumerators;
using DDS.Tools.Interfaces.Models;
using DDS.Tools.Interfaces.Providers;
using DDS.Tools.Interfaces.Services;
using DDS.Tools.Models;
using DDS.Tools.Providers;
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

		services.TryAddSingleton<DdsEncoder>();
		services.TryAddSingleton<DdsDecoder>();
		services.TryAddSingleton<ITodoService, TodoService>();

		services.TryAddSingleton<IDirectoryProvider, DirectoryProvider>();
		services.TryAddSingleton<IFileProvider, FileProvider>();
		services.TryAddSingleton<IPathProvider, PathProvider>();

		services.TryAddKeyedTransient<IImageModel, PngImageModel>(ImageType.PNG);
		services.TryAddKeyedTransient<IImageModel, DdsImageModel>(ImageType.DDS);

		return services;
	}

	private static IServiceCollection RegisterLoggerService(this IServiceCollection services, IHostEnvironment environment)
	{
		services.TryAddSingleton(typeof(ILoggerService<>), typeof(LoggerService<>));

		services.AddLogging(configure =>
		{
			configure.ClearProviders();
			configure.AddConsole();
			configure.SetMinimumLevel(LogLevel.Warning);

			if (environment.IsDevelopment())
				configure.SetMinimumLevel(LogLevel.Debug);
		});

		return services;
	}
}
