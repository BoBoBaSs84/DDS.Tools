// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Enumerators;
using DDS.Tools.Imaging;
using DDS.Tools.Interfaces.Imaging;
using DDS.Tools.Interfaces.Providers;
using DDS.Tools.Models;
using DDS.Tools.Settings;

using Spectre.Console;

namespace DDS.Tools.Services;

/// <summary>
/// Handles todo transformation execution and duplicate detection.
/// </summary>
/// <param name="directoryProvider">The directory provider instance to use.</param>
/// <param name="fileProvider">The file provider instance to use.</param>
/// <param name="pathProvider">The path provider instance to use.</param>
/// <param name="codecRegistry">The image codec registry instance to use.</param>
internal sealed class TodoTransformationService(
	IDirectoryProvider directoryProvider,
	IFileProvider fileProvider,
	IPathProvider pathProvider,
	IImageCodecRegistry codecRegistry)
{
	private readonly IDirectoryProvider _directoryProvider = directoryProvider;
	private readonly IFileProvider _fileProvider = fileProvider;
	private readonly IPathProvider _pathProvider = pathProvider;
	private readonly IImageCodecRegistry _codecRegistry = codecRegistry;

	internal TodoProcessingResult GetTodosDone(TodoCollection todos, ConvertSettings settings)
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

			TransferImage(settings, todo);
			processedHashes.Add(todo.FileHash);
			todosDoneCount++;
		}

		return new(todosDoneCount, todosDuplicateCount);
	}

	private static bool IsDuplicate(TodoModel todo, ConvertSettings settings, ISet<string> processedHashes)
		=> !settings.Restore && DeduplicatesByHash(settings.ConvertMode) && processedHashes.Contains(todo.FileHash);

	private void TransferImage(ConvertSettings settings, TodoModel todo)
	{
		// On a restore run the target format travels per file; otherwise it is the setting.
		ImageType targetFormat = todo.TargetType ?? settings.To;

		ImageCanvas canvas = _codecRegistry
			.GetDecoder(todo.SourceType)
			.Decode(_fileProvider.ReadAllBytes(todo.FullPathName));

		string targetFolder = PrepareTargetFolder(settings, canvas, todo);

		if (!_directoryProvider.CreateDirectory(targetFolder).Exists)
			return;

		string newFileName = GetTargetFileName(settings, todo);

		// The grouping mode copies the source untouched, every other mode re-encodes.
		if (settings.ConvertMode.Equals(ConvertModeType.Grouping) && !settings.Restore)
		{
			_fileProvider.Copy(todo.FullPathName, _pathProvider.Combine(targetFolder, newFileName));
			todo.FileName = newFileName;
			return;
		}

		byte[] encoded = _codecRegistry.GetEncoder(targetFormat).Encode(canvas, settings.Compression);
		string writtenName = $"{newFileName}.{targetFormat}";
		_fileProvider.WriteAllBytes(_pathProvider.Combine(targetFolder, writtenName), encoded);
		todo.FileName = writtenName;
	}

	private static bool DeduplicatesByHash(ConvertModeType convertMode)
		=> convertMode is ConvertModeType.Automatic or ConvertModeType.Grouping;

	private string PrepareTargetFolder(ConvertSettings settings, ImageCanvas canvas, TodoModel todo)
	{
		string newTargetFolder = todo.TargetFolder;

		// A restore run always rebuilds the recorded folder tree.
		if (settings.Restore)
			return $"{newTargetFolder}{todo.RelativePath}";

		if (settings.ConvertMode.Equals(ConvertModeType.Automatic))
		{
			newTargetFolder = _pathProvider.Combine(newTargetFolder, $"{canvas.Width}");
			return newTargetFolder;
		}
		else if (settings.ConvertMode.Equals(ConvertModeType.Grouping))
		{
			newTargetFolder = _pathProvider.Combine(newTargetFolder, $"{canvas.Width}x{canvas.Height}");
			return newTargetFolder;
		}

		if (settings.RetainStructure)
			return $"{newTargetFolder}{todo.RelativePath}";

		if (settings.SeparateBySize)
			newTargetFolder = _pathProvider.Combine(newTargetFolder, $"{canvas.Width}");

		return newTargetFolder;
	}

	private static string GetTargetFileName(ConvertSettings settings, TodoModel todo)
	{
		if (settings.Restore)
			return Path.GetFileNameWithoutExtension(todo.OriginalName);

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
