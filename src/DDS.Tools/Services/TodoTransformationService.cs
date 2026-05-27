// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Enumerators;
using DDS.Tools.Interfaces.Models;
using DDS.Tools.Interfaces.Providers;
using DDS.Tools.Models;
using DDS.Tools.Settings.Base;

using Spectre.Console;

namespace DDS.Tools.Services;

/// <summary>
/// Handles todo transformation execution and duplicate detection.
/// </summary>
/// <param name="directoryProvider">The directory provider instance to use.</param>
/// <param name="fileProvider">The file provider instance to use.</param>
/// <param name="pathProvider">The path provider instance to use.</param>
/// <param name="imageModelFactory">The image model factory instance to use.</param>
internal sealed class TodoTransformationService(
	IDirectoryProvider directoryProvider,
	IFileProvider fileProvider,
	IPathProvider pathProvider,
	Func<ImageType, IImageModel> imageModelFactory)
{
	private readonly IDirectoryProvider _directoryProvider = directoryProvider;
	private readonly IFileProvider _fileProvider = fileProvider;
	private readonly IPathProvider _pathProvider = pathProvider;
	private readonly Func<ImageType, IImageModel> _imageModelFactory = imageModelFactory;

	internal TodoProcessingResult GetTodosDone(TodoCollection todos, ConvertSettingsBase settings, ImageType imageType)
	{
		HashSet<string> todosDone = [];
		int todosDuplicateCount = 0;

		foreach (TodoModel todo in todos)
			GetTodoDone(todo, settings, imageType, todosDone, ref todosDuplicateCount);

		return new(todosDone.Count, todosDuplicateCount);
	}

	private void GetTodoDone(TodoModel todo, ConvertSettingsBase settings, ImageType imageType, ISet<string> todosDone, ref int todosDuplicateCount)
	{
		if (settings.ConvertMode.Equals(ConvertModeType.Automatic))
		{
			if (todosDone.Contains(todo.FileHash))
			{
				AnsiConsole.MarkupLine($"[yellow]'{todo.FullPathName}' is a duplicate![/]");
				todosDuplicateCount++;
				return;
			}
		}
		else if (settings.ConvertMode.Equals(ConvertModeType.Grouping))
		{
			if (todosDone.Contains(todo.FileHash))
			{
				AnsiConsole.MarkupLine($"[yellow]'{todo.FullPathName}' is a duplicate![/]");
				todosDuplicateCount++;
				return;
			}

			CopyImage(settings, todo, imageType);
			todosDone.Add(todo.FileHash);
			return;
		}

		SaveImage(settings, todo, imageType);
		todosDone.Add(todo.FileHash);
	}

	private void CopyImage(ConvertSettingsBase settings, TodoModel todo, ImageType imageType)
	{
		IImageModel image = _imageModelFactory(imageType);
		image.Load(todo.FullPathName);

		string targetFolder = PrepareTargetFolder(settings, image, todo);
		DirectoryInfo directoryInfo = _directoryProvider.CreateDirectory(targetFolder);

		if (directoryInfo.Exists)
		{
			string newFileName = GetTargetFileName(settings, todo);
			string newFilePath = _pathProvider.Combine(targetFolder, newFileName);

			_fileProvider.Copy(todo.FullPathName, newFilePath);
		}
	}

	private void SaveImage(ConvertSettingsBase settings, TodoModel todo, ImageType imageType)
	{
		IImageModel image = _imageModelFactory(imageType);
		image.Load(todo.FullPathName);

		string targetFolder = PrepareTargetFolder(settings, image, todo);
		DirectoryInfo directoryInfo = _directoryProvider.CreateDirectory(targetFolder);

		if (directoryInfo.Exists)
		{
			string newFileName = $"{GetTargetFileName(settings, todo)}.{GetTargetFileExtensions(imageType)}";
			string newFilePath = _pathProvider.Combine(targetFolder, newFileName);

			image.Save(newFilePath, settings);
		}
	}

	private string PrepareTargetFolder(ConvertSettingsBase settings, IImageModel image, TodoModel todo)
	{
		string newTargetFolder = todo.TargetFolder;

		if (settings.ConvertMode.Equals(ConvertModeType.Automatic))
		{
			newTargetFolder = _pathProvider.Combine(newTargetFolder, $"{image.Width}");
			return newTargetFolder;
		}
		else if (settings.ConvertMode.Equals(ConvertModeType.Grouping))
		{
			newTargetFolder = _pathProvider.Combine(newTargetFolder, $"{image.Width}x{image.Heigth}");
			return newTargetFolder;
		}

		if (settings.RetainStructure)
			return $"{newTargetFolder}{todo.RelativePath}";

		if (settings.SeparateBySize)
			newTargetFolder = _pathProvider.Combine(newTargetFolder, $"{image.Width}");

		return newTargetFolder;
	}

	private static string GetTargetFileName(ConvertSettingsBase settings, TodoModel todo)
	{
		if (settings.ConvertMode == ConvertModeType.Manual && settings.RetainStructure)
		{
			FileInfo info = new(todo.FullPathName);
			return todo.FileName.Replace(info.Extension, string.Empty);
		}
		else if (settings.ConvertMode.Equals(ConvertModeType.Grouping))
		{
			return todo.FileName;
		}

		return todo.FileHash;
	}

	private static string GetTargetFileExtensions(ImageType imageType)
		=> imageType switch
		{
			ImageType.DDS => $"{ImageType.PNG}",
			ImageType.PNG => $"{ImageType.DDS}",
			_ => throw new ArgumentOutOfRangeException(nameof(imageType), imageType, null)
		};
}
