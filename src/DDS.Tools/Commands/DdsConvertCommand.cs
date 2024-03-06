using System.Diagnostics.CodeAnalysis;

using DDS.Tools.Interfaces.Services;
using DDS.Tools.Settings;

using Microsoft.Extensions.Logging;

using Spectre.Console;
using Spectre.Console.Cli;

namespace DDS.Tools.Commands;

/// <summary>
/// The dds convert command class.
/// </summary>
/// <param name="logger">The logger instance to use.</param>
internal sealed class DdsConvertCommand(ILoggerService<DdsConvertCommand> logger) : Command<DdsConvertSettings>
{
	private readonly ILoggerService<DdsConvertCommand> _logger = logger;

	private static readonly Action<ILogger, Exception?> LogException =
		LoggerMessage.Define(LogLevel.Error, 0, "Exception occured.");

	/// <inheritdoc/>
	public override int Execute([NotNull] CommandContext context, [NotNull] DdsConvertSettings settings)
	{
		try
		{
			return 0;
		}
		catch (Exception ex)
		{
			_logger.Log(LogException, ex);
			AnsiConsole.Markup($"[maroon]{ex.Message}[/]");
			return 1;
		}
	}
}
