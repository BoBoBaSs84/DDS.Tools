using Microsoft.Extensions.Logging;

using DDS.Tools.Models;

namespace DDS.Tools.Services;

public class WeatherForecastService(ILogger<WeatherForecastService> logger)
{
	private static readonly string[] Summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

	private readonly ILogger _logger = logger;

	public IEnumerable<WeatherForecast> GetForecasts(int count)
	{
		_logger.LogDebug("Getting {count} forecasts.", count);

		Random rng = new();

		IEnumerable<WeatherForecast> forecasts = Enumerable.Range(1, count).Select(index => new WeatherForecast
		{
			Date = DateTime.Now.AddDays(index),
			TemperatureC = rng.Next(-20, 55),
			Summary = Summaries[rng.Next(Summaries.Length)]
		});

		return forecasts;
	}
}
