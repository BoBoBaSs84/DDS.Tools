// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

using DDS.Tools.Enumerators;
using DDS.Tools.Imaging;
using DDS.Tools.Interfaces.Imaging;
using DDS.Tools.Interfaces.Providers;
using DDS.Tools.Interfaces.Services;
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
	/// The image formats a codec is registered for.
	/// </summary>
	private static readonly ImageType[] SupportedFormats = [ImageType.DDS, ImageType.PNG, ImageType.TGA, ImageType.JPG];

	/// <summary>
	/// Registers the application services to the service collection.
	/// </summary>
	/// <param name="services">The service collection to enrich.</param>
	/// <param name="environment">The host environment instance to use.</param>
	/// <returns>The enriched service collection.</returns>
	internal static IServiceCollection RegisterServices(this IServiceCollection services, IHostEnvironment environment)
	{
		services.RegisterLoggerService(environment);
		services.RegisterImageCodecs();

		services.AddSingleton<ITodoService, TodoService>();

		services.AddSingleton<IDirectoryProvider, DirectoryProvider>();
		services.AddSingleton<IFileProvider, FileProvider>();
		services.AddSingleton<IPathProvider, PathProvider>();

		return services;
	}

	private static IServiceCollection RegisterImageCodecs(this IServiceCollection services)
	{
		foreach (ImageType format in SupportedFormats)
		{
			DirectXTexCodec codec = new(format);
			services.AddSingleton<IImageDecoder>(codec);
			services.AddSingleton<IImageEncoder>(codec);
		}

		services.AddSingleton<IImageCodecRegistry, ImageCodecRegistry>();

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
