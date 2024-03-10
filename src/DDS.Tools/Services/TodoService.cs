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
	/// <exception cref="ServiceException"></exception>
	public TodoCollection GetTodos(ConvertSettingsBase settings, ImageType imageType)
	{
		try
		{
			TodoCollection todos = [];

			string searchPattern = $"*.{imageType}";
			string[] files = Directory.GetFiles(settings.SourceFolder, searchPattern, SearchOption.AllDirectories);

			if (files.Length.Equals(0))
				return todos;

			files.AsParallel().ForEach(file => GetTodo(todos, settings, imageType, file));

			return todos;
		}
		catch (Exception ex)
		{
			_logger.Log(LogException, ex);
			throw new ServiceException($"Something went wrong in {nameof(GetTodos)}!", ex);
		}
	}

	/// <inheritdoc/>
	/// <exception cref="ServiceException"></exception>
	public TodoCollection GetTodosFromJson(ConvertSettingsBase settings, ImageType imageType, string jsonFilePath)
	{
		try
		{
			TodoCollection todos = [];

			string jsonFileContent = File.ReadAllText(jsonFilePath);

			TodoCollection todosFromJson = jsonFileContent.FromJson<TodoCollection>();

			todosFromJson.AsParallel().ForEach(tfj => GetTodoFromJson(todos, settings, imageType, tfj));

			return todos;
		}
		catch (Exception ex)
		{
			_logger.Log(LogException, ex);
			throw new ServiceException($"Something went wrong in {nameof(GetTodosFromJson)}!", ex);
		}
	}

	/// <inheritdoc/>
	/// <exception cref="ServiceException"></exception>
	public void GetTodosDone(TodoCollection todos, ConvertSettingsBase settings, ImageType imageType)
	{
		try
		{
			todos.AsParallel().ForEach(t => GetTodoDone(t, settings, imageType));

			if (settings.ConvertMode.Equals(ConvertModeType.Automatic))
			{
				string jsonContent = todos.ToJson();
				string jsonFilePath = Path.Combine(settings.TargetFolder, "Result.json");
				File.WriteAllText(jsonFilePath, jsonContent);
			}

			AnsiConsole.MarkupLine($"[green]Todos done:\t{_todosDone.Count}[/]");
			AnsiConsole.MarkupLine($"[yellow]Duplicates:\t{_todosDuplicateCount}[/]");
		}
		catch (Exception ex)
		{
			_logger.Log(LogException, ex);
			throw new ServiceException($"Something went wrong in {nameof(GetTodosDone)}!", ex);
		}
	}

	/// <inheritdoc/>
	public void GetTodosDoneFromJson(TodoCollection todos, ConvertSettingsBase settings, ImageType imageType)
	{
		try
		{
			todos.AsParallel().ForEach(t => GetTodoDone(t, settings, imageType));

		}
		catch (Exception ex)
		{
			_logger.Log(LogException, ex);
			throw new ServiceException($"Something went wrong in {nameof(GetTodosDoneFromJson)}!", ex);
		}
	}

	private void GetTodo(TodoCollection todos, ConvertSettingsBase settings, ImageType imageType, string file)
	{
		FileInfo fileInfo = new(file);

		IImageModel image = _provider.GetRequiredKeyedService<IImageModel>(imageType);
		image.Load(file);

		TodoModel todo = new(
			fileName: fileInfo.Name,
			relativePath: $"{fileInfo.DirectoryName?.Replace(settings.SourceFolder, string.Empty)}",
			fullPathName: fileInfo.FullName,
			targetFolder: settings.TargetFolder,
			fileHash: image.Hash
			);

		todos.Add(todo);
	}

	private static void GetTodoFromJson(TodoCollection todos, ConvertSettingsBase settings, ImageType imageType, TodoModel todoFromJson)
	{
		string newFullPathName =
			Path.Combine(settings.TargetFolder, todoFromJson.RelativePath, todoFromJson.FileName.Replace(GetTargetFileExtensions(imageType), $"{imageType}"));

		TodoModel todo = new(
			fileName: $"{todoFromJson.FileHash}.{GetTargetFileExtensions(imageType)}",
			relativePath: todoFromJson.RelativePath,
			fullPathName: newFullPathName,
			targetFolder: settings.TargetFolder,
			fileHash: todoFromJson.FileHash
			);

		todos.Add(todo);
	}

	private void GetTodoDone(TodoModel todo, ConvertSettingsBase settings, ImageType imageType)
	{
		if (settings.ConvertMode.Equals(ConvertModeType.Automatic))
		{
			if (_todosDone.Contains(todo.FileHash))
			{
				AnsiConsole.MarkupLine($"[yellow]'{todo.FullPathName}' is a duplicate![/]");
				_todosDuplicateCount++;
				return;
			}
		}

		SaveImage(settings, todo, imageType);

		_todosDone.Add(todo.FileHash);
	}

	private void SaveImage(ConvertSettingsBase settings, TodoModel todo, ImageType imageType)
	{
		IImageModel image = _provider.GetRequiredKeyedService<IImageModel>(imageType);
		image.Load(todo.FullPathName);

		string targetFolder = PrepareTargetFolder(settings, image, todo.TargetFolder);
		_ = Directory.CreateDirectory(targetFolder);

		string newFileName = $"{GetTargetFileName(settings, todo)}.{GetTargetFileExtensions(imageType)}";
		string newFilePath = Path.Combine(targetFolder, newFileName);

		image.Save(newFilePath);
	}

	private static string PrepareTargetFolder(ConvertSettingsBase settings, IImageModel image, string targetFolder)
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

	private static string GetTargetFileName(ConvertSettingsBase settings, TodoModel todo)
	{
		if (settings.ConvertMode == ConvertModeType.Manual && settings.RetainStructure)
		{
			FileInfo info = new(todo.FullPathName);
			return todo.FileName.Replace(info.Extension, string.Empty);
		}

		return todo.FileHash;
	}

	private static string GetTargetFileExtensions(ImageType imageType)
		=> imageType switch
		{
			ImageType.DDS => $"{ImageType.PNG}",
			ImageType.PNG => $"{ImageType.DDS}",
			_ => string.Empty,
		};
}
