using System.Diagnostics.CodeAnalysis;

using DDS.Tools.Interfaces.Services;
using DDS.Tools.Settings;

using Microsoft.Extensions.Logging;

using Spectre.Console;
using Spectre.Console.Cli;

namespace DDS.Tools.Commands;

/// <summary>
/// The png convert command class.
/// </summary>
internal sealed class PngConvertCommand(ILoggerService<DdsConvertCommand> logger) : Command<PngConvertSettings>
{
	private readonly ILoggerService<DdsConvertCommand> _logger = logger;

	private static readonly Action<ILogger, Exception?> LogException =
		LoggerMessage.Define(LogLevel.Error, 0, "Exception occured.");

	/// <inheritdoc/>
	public override int Execute([NotNull] CommandContext context, [NotNull] PngConvertSettings settings)
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
