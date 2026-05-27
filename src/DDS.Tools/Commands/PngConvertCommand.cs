// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

using DDS.Tools.Commands.Base;
using DDS.Tools.Enumerators;
using DDS.Tools.Interfaces.Providers;
using DDS.Tools.Interfaces.Services;
using DDS.Tools.Settings;

using Spectre.Console.Cli;

namespace DDS.Tools.Commands;

/// <summary>
/// The png convert command class.
/// </summary>
/// <param name="loggerService">The logger service instance to use.</param>
/// <param name="todoService">The todo service instance to use.</param>
/// <param name="directoryProvider">The directory provider instance to use.</param>
/// <param name="fileProvider">The file provider instance to use.</param>
/// <param name="pathProvider">The path provider instance to use.</param>
internal sealed class PngConvertCommand(
	ILoggerService<PngConvertCommand> loggerService,
	ITodoService todoService,
	IDirectoryProvider directoryProvider,
	IFileProvider fileProvider,
	IPathProvider pathProvider)
	: ConvertCommandBase<PngConvertSettings, PngConvertCommand>(loggerService, todoService, directoryProvider, fileProvider, pathProvider)
{
	private const ImageType Type = ImageType.PNG;

	/// <inheritdoc/>
	protected override int Execute([NotNull] CommandContext context, [NotNull] PngConvertSettings settings, CancellationToken cancellationToken)
	 => ExecuteCommand(settings, Type);
}
