using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Logging;
using DDS.Tools.Interfaces.Services;

namespace DDS.Tools.Services;

/// <summary>
/// The logger service class.
/// </summary>
/// <typeparam name="T">The type to work with.</typeparam>
[ExcludeFromCodeCoverage(Justification = "Wrapper class.")]
internal sealed class LoggerService<T>(ILogger<T> logger) : ILoggerService<T> where T : class
{
	/// <inheritdoc/>
	public void Log(Action<ILogger, Exception?> del, Exception? exception = null)
		=> del?.Invoke(logger, exception);

	/// <inheritdoc/>
	public void Log<T1>(Action<ILogger, T1, Exception?> del, T1 param, Exception? exception = null)
		=> del?.Invoke(logger, param, exception);

	/// <inheritdoc/>
	public void Log<T1, T2>(Action<ILogger, T1, T2, Exception?> del, T1 param1, T2 param2,
		Exception? exception = null)
		=> del?.Invoke(logger, param1, param2, exception);

	/// <inheritdoc/>
	public void Log<T1, T2, T3>(Action<ILogger, T1, T2, T3, Exception?> del, T1 param1, T2 param2,
		T3 param3, Exception? exception = null)
		=> del?.Invoke(logger, param1, param2, param3, exception);

	/// <inheritdoc/>
	public void Log<T1, T2, T3, T4>(Action<ILogger, T1, T2, T3, T4, Exception?> del, T1 param1,
		T2 param2, T3 param3, T4 param4, Exception? exception = null)
		=> del?.Invoke(logger, param1, param2, param3, param4, exception);

	/// <inheritdoc/>
	public void Log<T1, T2, T3, T4, T5>(Action<ILogger, T1, T2, T3, T4, T5, Exception?> del, T1 param1,
		T2 param2, T3 param3, T4 param4, T5 param5, Exception? exception = null)
		=> del?.Invoke(logger, param1, param2, param3, param4, param5, exception);

	/// <inheritdoc/>
	public void Log<T1, T2, T3, T4, T5, T6>(Action<ILogger, T1, T2, T3, T4, T5, T6, Exception?> del,
		T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, Exception? exception = null)
		=> del?.Invoke(logger, param1, param2, param3, param4, param5, param6, exception);
}
