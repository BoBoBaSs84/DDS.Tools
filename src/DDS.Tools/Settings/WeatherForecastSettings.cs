using System.ComponentModel;

using DDS.Tools.Enumerators;

using Spectre.Console.Cli;

namespace DDS.Tools.Settings;

public class WeatherForecastSettings : CommandSettings
{
	[Description("The number of weather forecasts to display.")]
	[CommandArgument(0, "[count]")]
	public int Count { get; set; }

	[Description("The unit of temperature.")]
	[CommandOption("-u|--unit")]
	public TemperatureUnit? Unit { get; set; }
}