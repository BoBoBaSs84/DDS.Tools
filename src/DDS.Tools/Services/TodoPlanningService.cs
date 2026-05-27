// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using BB84.Extensions.Serialization;

using DDS.Tools.Enumerators;
using DDS.Tools.Interfaces.Models;
using DDS.Tools.Interfaces.Providers;
using DDS.Tools.Models;
using DDS.Tools.Settings.Base;

namespace DDS.Tools.Services;

/// <summary>
/// Handles todo planning by discovering files and mapping result json entries.
/// </summary>
/// <param name="directoryProvider">The directory provider instance to use.</param>
/// <param name="pathProvider">The path provider instance to use.</param>
/// <param name="imageModelFactory">The image model factory instance to use.</param>
internal sealed class TodoPlanningService(
	IDirectoryProvider directoryProvider,
	IPathProvider pathProvider,
	Func<ImageType, IImageModel> imageModelFactory)
{
	private readonly IDirectoryProvider _directoryProvider = directoryProvider;
	private readonly IPathProvider _pathProvider = pathProvider;
	private readonly Func<ImageType, IImageModel> _imageModelFactory = imageModelFactory;

	internal TodoCollection GetTodos(ConvertSettingsBase settings, ImageType imageType)
	{
		TodoCollection todos = [];

		string[] files = _directoryProvider.GetFiles(settings.SourceFolder, $"*.{imageType}", SearchOption.AllDirectories);

		if (files.Length.Equals(0))
			return todos;

		foreach (string file in files)
			MapTodoFromFile(todos, settings, imageType, file);

		return todos;
	}

	internal TodoCollection GetTodos(ConvertSettingsBase settings, ImageType imageType, string jsonFileContent)
	{
		TodoCollection todos = [];

		TodoCollection todosFromJson = jsonFileContent.FromJson<TodoCollection>();

		foreach (TodoModel todoFromJson in todosFromJson)
			MapTodoFromJson(todos, settings, imageType, todoFromJson);

		return todos;
	}

	private void MapTodoFromFile(TodoCollection todos, ConvertSettingsBase settings, ImageType imageType, string file)
	{
		FileInfo fileInfo = new(file);

		IImageModel image = _imageModelFactory(imageType);
		image.Load(file);

		TodoModel todo = new(
			fileName: fileInfo.Name,
			relativePath: $"{fileInfo.DirectoryName?.Replace(settings.SourceFolder, string.Empty)}",
			fullPathName: fileInfo.FullName,
			targetFolder: settings.TargetFolder,
			fileHash: image.Hash
			);

		todos.Enqueue(todo);
	}

	private void MapTodoFromJson(TodoCollection todos, ConvertSettingsBase settings, ImageType imageType, TodoModel todoFromJson)
	{
		string newFullPathName = _pathProvider
			.Combine(settings.TargetFolder, todoFromJson.RelativePath, todoFromJson.FileName.Replace(GetTargetFileExtensions(imageType), $"{imageType}"));

		TodoModel todo = new(
			fileName: $"{todoFromJson.FileHash}.{GetTargetFileExtensions(imageType)}",
			relativePath: todoFromJson.RelativePath,
			fullPathName: newFullPathName,
			targetFolder: settings.TargetFolder,
			fileHash: todoFromJson.FileHash
			);

		todos.Enqueue(todo);
	}

	private static string GetTargetFileExtensions(ImageType imageType)
		=> imageType switch
		{
			ImageType.DDS => $"{ImageType.PNG}",
			ImageType.PNG => $"{ImageType.DDS}",
			_ => throw new ArgumentOutOfRangeException(nameof(imageType), imageType, null)
		};
}
