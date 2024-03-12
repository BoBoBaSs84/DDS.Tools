using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Spectre.Console.Cli;

namespace DDS.Tools.Common;

/// <summary>
/// The type registrar class.
/// </summary>
/// <param name="builder">The host builder instance to use.</param>
[ExcludeFromCodeCoverage]
internal sealed class TypeRegistrar(IHostBuilder builder) : ITypeRegistrar
{
	private readonly IHostBuilder _builder = builder;

	public ITypeResolver Build()
			=> new TypeResolver(_builder.Build());

	public void Register(Type service, Type implementation)
			=> _builder.ConfigureServices((_, services) => services.AddSingleton(service, implementation));

	public void RegisterInstance(Type service, object implementation)
			=> _builder.ConfigureServices((_, services) => services.AddSingleton(service, implementation));

	public void RegisterLazy(Type service, Func<object> func)
	{
		ArgumentNullException.ThrowIfNull(func);

		_ = _builder.ConfigureServices((_, services) => services.AddSingleton(service, _ => func()));
	}
}
