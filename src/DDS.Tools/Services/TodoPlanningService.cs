// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using BB84.Extensions;
using BB84.Extensions.Serialization;

using DDS.Tools.Extensions;
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

		string[] files = settings.SourceFormat.GetFileExtensions()
			.SelectMany(extension => _directoryProvider.GetFiles(settings.SourceFolder, $"*.{extension}", SearchOption.AllDirectories))
			.Distinct()
			.ToArray();

		if (files.Length.Equals(0))
			return todos;

		// The source folder may be relative or carry a trailing separator, while the file
		// information always reports an absolute path, so normalize before relating the two.
		string sourceFolder = _pathProvider.TrimEndingDirectorySeparator(_pathProvider.GetFullPath(settings.SourceFolder));

		foreach (string file in files)
			MapTodoFromFile(todos, settings, sourceFolder, file);

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

	private void MapTodoFromFile(TodoCollection todos, ConvertSettings settings, string sourceFolder, string file)
	{
		FileInfo fileInfo = new(file);

		TodoModel todo = new(
			fileName: fileInfo.Name,
			relativePath: $"{fileInfo.DirectoryName?.Replace(sourceFolder, string.Empty, StringComparison.OrdinalIgnoreCase)}",
			fullPathName: fileInfo.FullName,
			targetFolder: settings.TargetFolder,
			fileHash: _fileProvider.ReadAllBytes(file).GetMD5String()
			);

		todos.Enqueue(todo);
	}

	private void MapTodoFromJson(TodoCollection todos, ConvertSettings settings, TodoModel todoFromJson)
	{
		string newFullPathName = _pathProvider
			.Combine(settings.TargetFolder, todoFromJson.RelativePath, todoFromJson.FileName.Replace($"{settings.To}", $"{settings.SourceFormat}"));

		TodoModel todo = new(
			fileName: $"{todoFromJson.FileHash}.{settings.To}",
			relativePath: todoFromJson.RelativePath,
			fullPathName: newFullPathName,
			targetFolder: settings.TargetFolder,
			fileHash: todoFromJson.FileHash
			);

		todos.Enqueue(todo);
	}
}
