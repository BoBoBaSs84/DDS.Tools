using DDS.Tools.Enumerators;
using DDS.Tools.Models;
using DDS.Tools.Settings.Base;

namespace DDS.Tools.Interfaces.Services;

/// <summary>
/// The todo service interface.
/// </summary>
internal interface ITodoService
{
	/// <summary>
	/// Returns a collection of todos depending on the provided <see cref="ConvertSettings"/>.
	/// </summary>
	/// <param name="settings">The settings that need to be considered.</param>
	/// <param name="imageType">The image type to work with.</param>
	/// <returns>A collection of todos.</returns>
	TodoCollection GetTodos(ConvertSettings settings, ImageType imageType);
}
