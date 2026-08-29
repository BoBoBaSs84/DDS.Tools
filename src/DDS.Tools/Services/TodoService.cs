// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using BB84.Extensions;

using DDS.Tools.Exceptions;
using DDS.Tools.Interfaces.Imaging;
using DDS.Tools.Interfaces.Providers;
using DDS.Tools.Interfaces.Services;
using DDS.Tools.Models;
using DDS.Tools.Properties;
using DDS.Tools.Settings;

using Microsoft.Extensions.Logging;

namespace DDS.Tools.Services;

/// <summary>
/// The todo service class.
/// </summary>
/// <param name="loggerService">The logger service instance to use.</param>
/// <param name="directoryProvider">The directory provider instance to use.</param>
/// <param name="fileProvider">The file provider instance to use.</param>
/// <param name="pathProvider">The path provider instance to use.</param>
/// <param name="codecRegistry">The image codec registry instance to use.</param>
internal sealed class TodoService(
	ILoggerService<TodoService> loggerService,
	IDirectoryProvider directoryProvider,
	IFileProvider fileProvider,
	IPathProvider pathProvider,
	IImageCodecRegistry codecRegistry) : ITodoService
{
	private readonly TodoPlanningService _todoPlanningService = new(directoryProvider, fileProvider, pathProvider);
	private readonly TodoTransformationService _todoTransformationService = new(directoryProvider, fileProvider, pathProvider, codecRegistry);
	private readonly TodoPersistenceService _todoPersistenceService = new(fileProvider, pathProvider);
	private readonly ILoggerService<TodoService> _loggerService = loggerService;

	private static readonly Action<ILogger, Exception?> LogException =
		LoggerMessage.Define(LogLevel.Error, 0, "Exception occured.");

	/// <inheritdoc/>
	/// <exception cref="ServiceException"></exception>
	public TodoCollection GetTodos(ConvertSettings settings)
		=> Execute(() => _todoPlanningService.GetTodos(settings), nameof(GetTodos));

	/// <inheritdoc/>
	/// <exception cref="ServiceException"></exception>
	public TodoCollection GetTodos(ConvertSettings settings, string jsonFileContent)
		=> Execute(() => _todoPlanningService.GetTodos(settings, jsonFileContent), nameof(GetTodos));

	/// <inheritdoc/>
	/// <exception cref="ServiceException"></exception>
	public void GetTodosDone(TodoCollection todos, ConvertSettings settings, bool jsonExists = false)
		=> Execute(() =>
		{
			TodoProcessingResult result = _todoTransformationService.GetTodosDone(todos, settings);
			_todoPersistenceService.PersistResult(todos, settings, jsonExists, result);
		}, nameof(GetTodosDone));

	/// <summary>
	/// Executes the provided operation and translates any failure into a <see cref="ServiceException"/>.
	/// </summary>
	/// <typeparam name="TResult">The result type of the operation.</typeparam>
	/// <param name="operation">The operation to execute.</param>
	/// <param name="operationName">The name of the calling operation, used within the exception message.</param>
	/// <returns>The result of the operation.</returns>
	/// <exception cref="ServiceException">If the operation failed.</exception>
	private TResult Execute<TResult>(Func<TResult> operation, string operationName)
	{
		try
		{
			return operation();
		}
		catch (Exception ex)
		{
			_loggerService.Log(LogException, ex);
			string message = Resources.ServiceException_Message.FormatInvariant(operationName);
			throw new ServiceException(message, ex);
		}
	}

	/// <inheritdoc cref="Execute{TResult}(Func{TResult}, string)"/>
	private void Execute(Action operation, string operationName)
		=> Execute<object?>(() => { operation(); return null; }, operationName);
}
