using System.Diagnostics.CodeAnalysis;

namespace DDS.Tools.Interfaces.Providers;

/// <summary>
/// The interface for the file provider.
/// </summary>
public interface IFileProvider
{
	/// <inheritdoc cref="File.Exists(string?)"/>
	bool Exists([NotNullWhen(true)] string? path);

	/// <inheritdoc cref="File.Move(string, string)"/>
	void Move(string sourceFileName, string destFileName);

	/// <inheritdoc cref="File.ReadAllText(string)"/>
	string ReadAllText(string path);

	/// <inheritdoc cref="File.WriteAllText(string, string?)"/>
	void WriteAllText(string path, string? contents);
}
