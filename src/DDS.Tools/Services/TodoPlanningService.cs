// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using BB84.Extensions;
using BB84.Extensions.Serialization;

using DDS.Tools.Enumerators;
using DDS.Tools.Exceptions;
using DDS.Tools.Interfaces.Providers;
using DDS.Tools.Models;
using DDS.Tools.Settings;

namespace DDS.Tools.Services;

/// <summary>
/// Handles todo planning by discovering files and mapping result json entries.
/// </summary>
/// <param name="directoryProvider">The directory provider instance to use.</param>
/// <param name="fileProvider">The file provider instance to use.</param>
/// <param name="pathProvider">The path provider instance to use.</param>
internal sealed class TodoPlanningService(
	IDirectoryProvider directoryProvider,
	IFileProvider fileProvider,
	IPathProvider pathProvider)
{
	private readonly IDirectoryProvider _directoryProvider = directoryProvider;
	private readonly IFileProvider _fileProvider = fileProvider;
	private readonly IPathProvider _pathProvider = pathProvider;

	internal TodoCollection GetTodos(ConvertSettings settings)
	{
		TodoCollection todos = [];

		// When '--from' is omitted every known format is scanned, so a folder may hold a
		// mix of formats; an explicit '--from' restricts the run to that single format.
		IEnumerable<ImageType> formats = settings.From is not null
			? [settings.From.Value]
			: ImageTypeExtensions.GetValuesFast();

		// The source folder may be relative or carry a trailing separator, while the file
		// information always reports an absolute path, so normalize before relating the two.
		string sourceFolder = _pathProvider.TrimEndingDirectorySeparator(_pathProvider.GetFullPath(settings.SourceFolder));

		foreach (ImageType format in formats)
		{
			string[] files = [.. format.GetFileExtensions()
				.SelectMany(extension => _directoryProvider.GetFiles(settings.SourceFolder, $"*.{extension}", SearchOption.AllDirectories))
				.Distinct()];

			foreach (string file in files)
				MapTodoFromFile(todos, settings, sourceFolder, file, format);
		}

		return todos;
	}

	internal TodoCollection GetTodos(ConvertSettings settings, string jsonFileContent)
	{
		TodoCollection todos = [];

		TodoCollection todosFromJson = jsonFileContent.FromJson<TodoCollection>();

		foreach (TodoModel todoFromJson in todosFromJson)
			MapTodoFromJson(todos, settings, todoFromJson);

		return todos;
	}

	private void MapTodoFromFile(TodoCollection todos, ConvertSettings settings, string sourceFolder, string file, ImageType sourceType)
	{
		FileInfo fileInfo = new(file);

		TodoModel todo = new(
			fileName: fileInfo.Name,
			relativePath: $"{fileInfo.DirectoryName?.Replace(sourceFolder, string.Empty, StringComparison.OrdinalIgnoreCase)}",
			fullPathName: fileInfo.FullName,
			targetFolder: settings.TargetFolder,
			fileHash: _fileProvider.ReadAllBytes(file).GetMD5String(),
			sourceType: sourceType,
			originalName: fileInfo.Name
			);

		todos.Enqueue(todo);
	}

	private void MapTodoFromJson(TodoCollection todos, ConvertSettings settings, TodoModel todoFromJson)
	{
		if (!Enum.IsDefined(todoFromJson.SourceType))
			throw new CommandException($"The result json entry '{todoFromJson.FileName}' has no original image format to restore to.");

		string originalName = string.IsNullOrEmpty(todoFromJson.OriginalName)
			? todoFromJson.FileName
			: todoFromJson.OriginalName;

		if (!ImageTypeExtensions.TryGetImageType(todoFromJson.FileName, out ImageType decodeType))
			throw new CommandException($"The result json entry '{todoFromJson.FileName}' does not carry a known image extension.");

		// The relative path carries a leading separator (or is empty), so it is concatenated
		// rather than combined, matching how the transformation rebuilds retained folders.
		string fullPathName = _pathProvider
			.Combine($"{settings.SourceFolder}{todoFromJson.RelativePath}", todoFromJson.FileName);

		TodoModel todo = new(
			fileName: originalName,
			relativePath: todoFromJson.RelativePath,
			fullPathName: fullPathName,
			targetFolder: settings.TargetFolder,
			fileHash: todoFromJson.FileHash,
			sourceType: decodeType,
			originalName: originalName
			)
		{
			TargetType = todoFromJson.SourceType
		};

		todos.Enqueue(todo);
	}
}
