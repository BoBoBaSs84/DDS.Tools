using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Configuration;

using DDS.Tools.Enumerators;
using DDS.Tools.Models;
using DDS.Tools.Services;
using DDS.Tools.Settings;

using Spectre.Console;
using Spectre.Console.Cli;

namespace DDS.Tools.Commands;

public class WeatherForecastCommand : Command<WeatherForecastSettings>
{
	private readonly IConfiguration _configuration;

	private readonly WeatherForecastService _weatherForecastService;

	public WeatherForecastCommand(IConfiguration configuration, WeatherForecastService weatherForecastService)
	{
		_configuration = configuration;
		_weatherForecastService = weatherForecastService;
	}

	public override int Execute([NotNull] CommandContext context, [NotNull] WeatherForecastSettings settings)
	{
		TemperatureUnit unit = settings.Unit ??
									 _configuration.GetValue<TemperatureUnit>("Unit");

		IEnumerable<WeatherForecast> forecasts = _weatherForecastService.GetForecasts(settings.Count);

		Table table = new();

		_ = table.AddColumn("Date");
		_ = table.AddColumn("Temp");
		_ = table.AddColumn("Summary");

		foreach (WeatherForecast forecast in forecasts)
		{
			int temperature = unit == TemperatureUnit.Fahrenheit
								? forecast.TemperatureF
								: forecast.TemperatureC;

			_ = table.AddRow(
					forecast.Date.ToShortDateString(),
					temperature.ToString(),
					forecast.Summary);
		}

		_ = table.Expand();

		AnsiConsole.Write(table);

		return 0;
	}
}