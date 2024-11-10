using System.Diagnostics.CodeAnalysis;

using DDS.Tools.Interfaces.Providers;

namespace DDS.Tools.Providers;

/// <summary>
/// The file provider class.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Wrapper class.")]
internal sealed class FileProvider : IFileProvider
{
	public void Copy(string sourceFileName, string destFileName)
		=> File.Copy(sourceFileName, destFileName);

	public bool Exists([NotNullWhen(true)] string? path)
		=> File.Exists(path);

	public void Move(string sourceFileName, string destFileName)
		=> File.Move(sourceFileName, destFileName);

	public string ReadAllText(string path)
		=> File.ReadAllText(path);

	public void WriteAllText(string path, string? contents)
		=> File.WriteAllText(path, contents);
}
