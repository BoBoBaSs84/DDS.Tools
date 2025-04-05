// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using DDS.Tools.Common;
using DDS.Tools.Extensions;

using Microsoft.Extensions.Hosting;

using Spectre.Console;
using Spectre.Console.Cli;

[ExcludeFromCodeCoverage(Justification = "Program startup class.")]
internal sealed class Program
{
	private static readonly Assembly Assembly = typeof(Program).Assembly;

	private static int Main(string[] args)
	{
		AnsiConsole.Write(new FigletText($"{Assembly.GetName().Name}"));
		AnsiConsole.WriteLine($"{Assembly.GetName().Name} Command-line-interface {Assembly.GetName().Version}");
		AnsiConsole.WriteLine();

		IHostBuilder builder = Host.CreateDefaultBuilder(args)
			.ConfigureServices((context, services) => services.RegisterServices(context.HostingEnvironment));

		CommandApp app = new(new TypeRegistrar(builder));

		// Register available commands
		app.Configure(config => config.ConfigureCommands());

		return app.Run(args);
	}
}
