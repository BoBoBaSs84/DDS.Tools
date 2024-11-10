using System.Diagnostics.CodeAnalysis;

using DDS.Tools.Interfaces.Providers;

namespace DDS.Tools.Providers;

/// <summary>
/// The directory provider class.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Wrapper class.")]
internal sealed class DirectoryProvider : IDirectoryProvider
{
	public DirectoryInfo CreateDirectory(string path)
		=> Directory.CreateDirectory(path);

	public bool Exists([NotNullWhen(true)] string? path)
		=> Directory.Exists(path);

	public string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
		=> Directory.GetFiles(path, searchPattern, searchOption);
}
