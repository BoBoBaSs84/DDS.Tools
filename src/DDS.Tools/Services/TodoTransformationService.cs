// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Enumerators;
using DDS.Tools.Extensions;
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
		// The hash set only indexes what has been seen. Counting it would report distinct
		// content rather than transferred files, which differ whenever a mode does not deduplicate.
		HashSet<string> processedHashes = [];
		int todosDoneCount = 0;
		int todosDuplicateCount = 0;

		foreach (TodoModel todo in todos)
		{
			if (IsDuplicate(todo, settings, processedHashes))
			{
				AnsiConsole.MarkupLine($"[yellow]'{todo.FullPathName}' is a duplicate![/]");
				todosDuplicateCount++;
				continue;
			}

			TransferImage(settings, todo, imageType);
			processedHashes.Add(todo.FileHash);
			todosDoneCount++;
		}

		return new(todosDoneCount, todosDuplicateCount);
	}

	private static bool IsDuplicate(TodoModel todo, ConvertSettingsBase settings, ISet<string> processedHashes)
		=> DeduplicatesByHash(settings.ConvertMode) && processedHashes.Contains(todo.FileHash);

	private void TransferImage(ConvertSettingsBase settings, TodoModel todo, ImageType imageType)
	{
		using IImageModel image = _imageModelFactory(imageType);
		image.Load(todo.FullPathName);

		string targetFolder = PrepareTargetFolder(settings, image, todo);

		if (!_directoryProvider.CreateDirectory(targetFolder).Exists)
			return;

		string newFileName = GetTargetFileName(settings, todo);

		// The grouping mode copies the source untouched, every other mode re-encodes.
		if (settings.ConvertMode.Equals(ConvertModeType.Grouping))
		{
			_fileProvider.Copy(todo.FullPathName, _pathProvider.Combine(targetFolder, newFileName));
			return;
		}

		image.Save(_pathProvider.Combine(targetFolder, $"{newFileName}.{imageType.GetTargetType()}"), settings);
	}

	private static bool DeduplicatesByHash(ConvertModeType convertMode)
		=> convertMode is ConvertModeType.Automatic or ConvertModeType.Grouping;

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
			newTargetFolder = _pathProvider.Combine(newTargetFolder, $"{image.Width}x{image.Height}");
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
}
