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

		// The encoder carries per image output options, so it must not be shared.
		services.AddTransient<DdsEncoder>();
		services.AddSingleton<DdsDecoder>();
		services.AddSingleton<ITodoService, TodoService>();

		services.AddSingleton<IDirectoryProvider, DirectoryProvider>();
		services.AddSingleton<IFileProvider, FileProvider>();
		services.AddSingleton<IPathProvider, PathProvider>();

		// Image models are disposable and created per file. Resolving them from the container
		// would enroll every instance in the root scope's disposal tracking and keep the native
		// image memory alive for the whole run, so they are activated with the caller owning them.
		services.AddSingleton<Func<ImageType, IImageModel>>(serviceProvider => imageType => imageType switch
		{
			ImageType.PNG => ActivatorUtilities.CreateInstance<PngImageModel>(serviceProvider),
			ImageType.DDS => ActivatorUtilities.CreateInstance<DdsImageModel>(serviceProvider),
			_ => throw new ArgumentOutOfRangeException(nameof(imageType), imageType, null)
		});

		return services;
	}

	private static IServiceCollection RegisterLoggerService(this IServiceCollection services, IHostEnvironment environment)
	{
		services.AddSingleton(typeof(ILoggerService<>), typeof(LoggerService<>));

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
