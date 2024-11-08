using System.Diagnostics.CodeAnalysis;

using DDS.Tools.Commands.Base;
using DDS.Tools.Enumerators;
using DDS.Tools.Interfaces.Services;
using DDS.Tools.Settings;

using Microsoft.Extensions.Logging;

using Spectre.Console;
using Spectre.Console.Cli;

namespace DDS.Tools.Commands;

/// <summary>
/// The dds convert command class.
/// </summary>
/// <param name="loggerService">The logger service instance to use.</param>
/// <param name="todoService">The todo service instance to use.</param>
/// <param name="serviceProvider">The service provier instance to use.</param>
internal sealed class DdsConvertCommand(ILoggerService<DdsConvertCommand> loggerService, ITodoService todoService, IServiceProvider serviceProvider)
	: ConvertCommandBase<DdsConvertSettings>(todoService, serviceProvider)
{
	private const ImageType Type = ImageType.DDS;
	private readonly ILoggerService<DdsConvertCommand> _loggerService = loggerService;

	private static readonly Action<ILogger, Exception?> LogException =
		LoggerMessage.Define(LogLevel.Error, 0, "Exception occured.");

	/// <inheritdoc/>
	public override int Execute([NotNull] CommandContext context, [NotNull] DdsConvertSettings settings)
	{
		try
		{
			return AnsiConsole.Status()
				.Spinner(Spinner.Known.Line)
				.Start("Processing..", action => Action(settings, Type));
		}
		catch (Exception ex)
		{
			_loggerService.Log(LogException, ex);
			AnsiConsole.MarkupLine($"[maroon]{ex.Message}[/]");
			return 1;
		}
	}
}
