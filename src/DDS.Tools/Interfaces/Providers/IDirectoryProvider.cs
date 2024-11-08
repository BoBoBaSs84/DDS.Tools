namespace DDS.Tools.Interfaces.Providers;

/// <summary>
/// The interface for the directory provider.
/// </summary>
public interface IDirectoryProvider
{
	/// <inheritdoc cref="Directory.CreateDirectory(string)"/>
	DirectoryInfo CreateDirectory(string path);
}
