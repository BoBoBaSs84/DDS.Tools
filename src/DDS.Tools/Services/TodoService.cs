using BB84.Extensions;
using BB84.Extensions.Serialization;

using DDS.Tools.Enumerators;
using DDS.Tools.Exceptions;
using DDS.Tools.Interfaces.Models;
using DDS.Tools.Interfaces.Services;
using DDS.Tools.Models;
using DDS.Tools.Settings.Base;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Spectre.Console;

namespace DDS.Tools.Services;

/// <summary>
/// The todo service class.
/// </summary>
/// <param name="logger">The logger service instance to use.</param>
/// <param name="provider">The service provider instance to use.</param>
internal sealed class TodoService(ILoggerService<TodoService> logger, IServiceProvider provider) : ITodoService
{
	private readonly ILoggerService<TodoService> _logger = logger;
	private readonly IServiceProvider _provider = provider;
	private readonly List<string> _todosDone = [];
	private int _todosDuplicateCount = 0;

	private static readonly Action<ILogger, Exception?> LogException =
		LoggerMessage.Define(LogLevel.Error, 0, "Exception occured.");

	/// <inheritdoc/>
	public TodoCollection GetTodos(ConvertSettings settings, ImageType imageType)
	{
		TodoCollection todos = [];

		try
		{
			string sourceFolder = settings.SourceFolder;

			if (!Directory.Exists(sourceFolder))
				throw new ServiceException($"Directory '{sourceFolder}' not found.");

			string searchPattern = $"*.{imageType}";
			string[] files = Directory.GetFiles(sourceFolder, searchPattern, SearchOption.AllDirectories);

			if (files.Length.Equals(0))
				return todos;

			DirectoryInfo directoryInfo = new(sourceFolder);

			foreach (string file in files)
			{
				FileInfo fileInfo = new(file);

				string relativePath = $"{fileInfo.DirectoryName!.Replace(directoryInfo.Parent!.FullName, string.Empty)}";

				IImageModel image = _provider.GetRequiredKeyedService<IImageModel>(imageType);

				image.Load(file);

				todos.Add(new(image.Name, relativePath, file, settings.TargetFolder, image.Hash));
			}

			return todos;
		}
		catch (Exception ex)
		{
			_logger.Log(LogException, ex);
			AnsiConsole.MarkupLine($"[maroon]{ex.Message}[/]");
			return todos;
		}
	}

	public void GetTodosDone(TodoCollection todos, ConvertSettings settings, ImageType imageType)
	{
		try
		{
			todos.AsParallel().ForEach(t => GetTodoDone(t, settings, imageType));

			if (settings.ConvertMode.Equals(ConvertModeType.Automatic))
			{
				string jsonResult = todos.ToJson();
				string jsonResultPath = Path.Combine(settings.TargetFolder, "Result.json");
				File.WriteAllText(jsonResultPath, jsonResult);
			}

			AnsiConsole.MarkupLine($"[green]Todos done:\t{_todosDone.Count}[/]");
			AnsiConsole.MarkupLine($"[yellow]Duplicates:\t{_todosDuplicateCount}[/]");
		}
		catch (Exception ex)
		{
			_logger.Log(LogException, ex);
			AnsiConsole.MarkupLine($"[maroon]{ex.Message}[/]");
		}
	}

	private void GetTodoDone(TodoModel todo, ConvertSettings settings, ImageType imageType)
	{
		if (_todosDone.Contains(todo.Hash))
		{
			AnsiConsole.MarkupLine($"[yellow]{todo.FileName} is a duplicate[/]");
			_todosDuplicateCount++;
			return;
		}

		SaveImage(settings, todo, imageType);

		_todosDone.Add(todo.Hash);
	}

	private void SaveImage(ConvertSettings settings, TodoModel todo, ImageType imageType)
	{
		IImageModel image = _provider.GetRequiredKeyedService<IImageModel>(imageType);
		image.Load(todo.FullPathName);

		string targetFolder = PrepareTargetFolder(settings, image, todo.TargetPath);
		_ = Directory.CreateDirectory(targetFolder);

		string newFileName = $"{GetTargetFileName(settings, todo)}.{GetTargetFileExtensions(imageType)}";
		string newFilePath = Path.Combine(targetFolder, newFileName);

		image.Save(newFilePath);
	}

	private static string PrepareTargetFolder(ConvertSettings settings, IImageModel image, string targetFolder)
	{
		string newTargetFolder = targetFolder;

		if (settings.ConvertMode.Equals(ConvertModeType.Automatic))
		{
			newTargetFolder = Path.Combine(newTargetFolder, $"{image.Width}");
			return newTargetFolder;
		}

		if (settings.RetainStructure)
			return newTargetFolder;

		if (settings.SeparateBySize)
			newTargetFolder = Path.Combine(newTargetFolder, $"{image.Width}");

		return newTargetFolder;
	}

	private static string GetTargetFileName(ConvertSettings settings, TodoModel todo)
	{
		if (settings.ConvertMode == ConvertModeType.Manual && settings.RetainStructure)
		{
			FileInfo info = new(todo.FullPathName);
			return todo.FileName.Replace(info.Extension, string.Empty);
		}

		return todo.Hash;
	}

	private static string GetTargetFileExtensions(ImageType imageType)
		=> imageType switch
		{
			ImageType.DDS => $"{ImageType.PNG}",
			ImageType.PNG => $"{ImageType.DDS}",
			_ => string.Empty,
		};
}
