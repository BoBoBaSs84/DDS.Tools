using System.Diagnostics.CodeAnalysis;

using DDS.Tools.Interfaces.Providers;

namespace DDS.Tools.Providers;

/// <summary>
/// The path provider class.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Wrapper class.")]
internal sealed class PathProvider : IPathProvider
{
	public string Combine(string path1, string path2)
		=> Path.Combine(path1, path2);

	public string Combine(string path1, string path2, string path3)
		=> Path.Combine(path1, path2, path3);
}
