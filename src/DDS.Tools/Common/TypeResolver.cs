using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Hosting;

using Spectre.Console.Cli;

namespace DDS.Tools.Common;

/// <summary>
/// The type resolver class.
/// </summary>
/// <param name="provider">The host provider instance to use.</param>
[ExcludeFromCodeCoverage]
internal sealed class TypeResolver(IHost provider) : ITypeResolver, IDisposable
{
	private readonly IHost _host = provider ?? throw new ArgumentNullException(nameof(provider));

	public object? Resolve(Type? type)
		=> type is not null ? _host.Services.GetService(type) : null;

	public void Dispose()
		=> _host.Dispose();
}
