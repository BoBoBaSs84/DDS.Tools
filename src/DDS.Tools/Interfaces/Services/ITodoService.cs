// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
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
	/// Returns a collection of todos depending on the provided <see cref="ConvertSettingsBase"/>.
	/// </summary>
	/// <param name="settings">The settings that need to be considered.</param>
	/// <param name="imageType">The image type to work with.</param>
	/// <returns>A collection of todos.</returns>
	TodoCollection GetTodos(ConvertSettingsBase settings, ImageType imageType);

	/// <summary>
	/// Returns a collection of todos depending on the provided <see cref="ConvertSettingsBase"/>
	/// and the <paramref name="jsonFileContent"/>.
	/// </summary>
	/// <param name="settings">The settings that need to be considered.</param>
	/// <param name="imageType">The image type to work with.</param>
	/// <param name="jsonFileContent">The json content to work with.</param>
	/// <returns>A collection of todos.</returns>
	TodoCollection GetTodos(ConvertSettingsBase settings, ImageType imageType, string jsonFileContent);

	/// <summary>
	/// Get the todos done.
	/// </summary>
	/// <param name="todos">The collection of todos.</param>
	/// <param name="settings">The settings that need to be considered.</param>
	/// <param name="imageType">The image type to work with.</param>
	/// <param name="jsonExists">Do the todos come from a json result file?</param>
	void GetTodosDone(TodoCollection todos, ConvertSettingsBase settings, ImageType imageType, bool jsonExists = false);
}
