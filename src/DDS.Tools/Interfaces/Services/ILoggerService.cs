// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using Microsoft.Extensions.Logging;

namespace DDS.Tools.Interfaces.Services;

/// <summary>
/// The logger service interface.
/// </summary>
/// <typeparam name="T">The type to work with.</typeparam>
public interface ILoggerService<T> where T : class
{
	/// <summary>
	/// Logs a message with the help of a delegate
	/// </summary>
	/// <remarks>Delegate must be defined like this: <b>LoggerMessage.Define(LogLevel, EventId, "test")</b></remarks>
	/// <param name="del">Delegate which performs the logging</param>
	/// <param name="exception">Exception (optional)</param>
	void Log(Action<ILogger, Exception?> del, Exception? exception = null);

	/// <summary>
	/// Logs a message with the help of a delegate
	/// </summary>
	/// <remarks>Delegate must be defined like this: <b>LoggerMessage.Define{T}(LogLevel, EventId, "test {param}")</b></remarks>
	/// <typeparam name="T1">Type of parameter of the string template</typeparam>
	/// <param name="del">Delegate which performs the logging</param>
	/// <param name="param">Parameter</param>
	/// <param name="exception">Exception (optional)</param>
	void Log<T1>(Action<ILogger, T1, Exception?> del, T1 param, Exception? exception = null);

	/// <summary>
	/// Logs a message with the help of a delegate
	/// </summary>
	/// <remarks>Delegate must be defined like this: <b>LoggerMessage.Define{T1, T2}(LogLevel, EventId, "test {param1} {param2}")</b></remarks>
	/// <typeparam name="T1">Type of parameter 1 of the string template</typeparam>
	/// <typeparam name="T2">Type of parameter 2 of the string template</typeparam>
	/// <param name="del">Delegate which performs the logging</param>
	/// <param name="param1">Parameter 1</param>
	/// <param name="param2">Parameter 2</param>
	/// <param name="exception">Exception (optional)</param>
	void Log<T1, T2>(Action<ILogger, T1, T2, Exception?> del, T1 param1, T2 param2, Exception? exception = null);

	/// <summary>
	/// Logs a message with the help of a delegate
	/// </summary>
	/// <remarks>Delegate must be defined like this: <b>LoggerMessage.Define{T1, T2, T3}(LogLevel, EventId, "test {param1} {param2} {param3}")</b></remarks>
	/// <typeparam name="T1">Type of parameter 1 of the string template</typeparam>
	/// <typeparam name="T2">Type of parameter 2 of the string template</typeparam>
	/// <typeparam name="T3">Type of parameter 3 of the string template</typeparam>
	/// <param name="del">Delegate which performs the logging</param>
	/// <param name="param1">Parameter 1</param>
	/// <param name="param2">Parameter 2</param>
	/// <param name="param3">Parameter 3</param>
	/// <param name="exception">Exception (optional)</param>
	void Log<T1, T2, T3>(Action<ILogger, T1, T2, T3, Exception?> del, T1 param1, T2 param2, T3 param3, Exception? exception = null);

	/// <summary>
	/// Logs a message with the help of a delegate
	/// </summary>
	/// <remarks>Delegate must be defined like this: <b>LoggerMessage.Define{T1, T2, T3, T4}(LogLevel, EventId, "test {param1} {param2} {param3} {param4}")</b></remarks>
	/// <typeparam name="T1">Type of parameter 1 of the string template</typeparam>
	/// <typeparam name="T2">Type of parameter 2 of the string template</typeparam>
	/// <typeparam name="T3">Type of parameter 3 of the string template</typeparam>
	/// <typeparam name="T4">Type of parameter 4 of the string template</typeparam>
	/// <param name="del">Delegate which performs the logging</param>
	/// <param name="param1">Parameter 1</param>
	/// <param name="param2">Parameter 2</param>
	/// <param name="param3">Parameter 3</param>
	/// <param name="param4">Parameter 4</param>
	/// <param name="exception">Exception (optional)</param>
	void Log<T1, T2, T3, T4>(Action<ILogger, T1, T2, T3, T4, Exception?> del, T1 param1, T2 param2, T3 param3, T4 param4, Exception? exception = null);

	/// <summary>
	/// Logs a message with the help of a delegate
	/// </summary>
	/// <remarks>Delegate must be defined like this: <b>LoggerMessage.Define{T1, T2, T3, T4, T5}(LogLevel, EventId, "test {param1} {param2} {param3} {param4} {param5}")</b></remarks>
	/// <typeparam name="T1">Type of parameter 1 of the string template</typeparam>
	/// <typeparam name="T2">Type of parameter 2 of the string template</typeparam>
	/// <typeparam name="T3">Type of parameter 3 of the string template</typeparam>
	/// <typeparam name="T4">Type of parameter 4 of the string template</typeparam>
	/// <typeparam name="T5">Type of parameter 5 of the string template</typeparam>
	/// <param name="del">Delegate which performs the logging</param>
	/// <param name="param1">Parameter 1</param>
	/// <param name="param2">Parameter 2</param>
	/// <param name="param3">Parameter 3</param>
	/// <param name="param4">Parameter 4</param>
	/// <param name="param5">Parameter 5</param>
	/// <param name="exception">Exception (optional)</param>
	void Log<T1, T2, T3, T4, T5>(Action<ILogger, T1, T2, T3, T4, T5, Exception?> del, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, Exception? exception = null);

	/// <summary>
	/// Logs a message with the help of a delegate
	/// </summary>
	/// <remarks>Delegate must be defined like this: <b>LoggerMessage.Define{T1, T2, T3, T4, T5, T6}(LogLevel, EventId, "test {param1} {param2} {param3} {param4} {param5} {param6}")</b></remarks>
	/// <typeparam name="T1">Type of parameter 1 of the string template</typeparam>
	/// <typeparam name="T2">Type of parameter 2 of the string template</typeparam>
	/// <typeparam name="T3">Type of parameter 3 of the string template</typeparam>
	/// <typeparam name="T4">Type of parameter 4 of the string template</typeparam>
	/// <typeparam name="T5">Type of parameter 5 of the string template</typeparam>
	/// <typeparam name="T6">Type of parameter 6 of the string template</typeparam>
	/// <param name="del">Delegate which performs the logging</param>
	/// <param name="param1">Parameter 1</param>
	/// <param name="param2">Parameter 2</param>
	/// <param name="param3">Parameter 3</param>
	/// <param name="param4">Parameter 4</param>
	/// <param name="param5">Parameter 5</param>
	/// <param name="param6">Parameter 6</param>
	/// <param name="exception">Exception (optional)</param>
	void Log<T1, T2, T3, T4, T5, T6>(Action<ILogger, T1, T2, T3, T4, T5, T6, Exception?> del, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, Exception? exception = null);
}
