using System.Diagnostics.CodeAnalysis;

using DDS.Tools.Interfaces.Providers;

namespace DDS.Tools.Providers;

/// <summary>
/// The file provider class.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Wrapper class.")]
internal sealed class FileProvider : IFileProvider
{
	public void Move(string sourceFileName, string destFileName)
		=> File.Move(sourceFileName, destFileName);

	public void WriteAllText(string path, string? contents)
		=> File.WriteAllText(path, contents);
}
