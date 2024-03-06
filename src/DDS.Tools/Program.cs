using System.Reflection;

using DDS.Tools.Common;
using DDS.Tools.Extensions;

using Microsoft.Extensions.Hosting;

using Spectre.Console;
using Spectre.Console.Cli;

internal class Program
{
	private static readonly Assembly Assembly = typeof(Program).Assembly;

	private static int Main(string[] args)
	{
		AnsiConsole.Write(new FigletText($"{Assembly.GetName().Name}"));
		AnsiConsole.WriteLine($"{Assembly.GetName().Name} Command-line-interface {Assembly.GetName().Version}");
		AnsiConsole.WriteLine();

		IHostBuilder builder = Host.CreateDefaultBuilder(args)
			.ConfigureServices((context, services) => services.RegisterServices(context.HostingEnvironment));

		TypeRegistrar registrar = new(builder);

		CommandApp app = new(registrar);

		// Register available commands
		app.Configure(config => config.ConfigureCommands());

		return app.Run(args);
	}
}
