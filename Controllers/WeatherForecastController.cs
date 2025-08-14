using FoodDeliveryBackend.Utils;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryBackend.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecastV1")]
        [MapToApiVersion("1.0")]
        [SwaggerOperationSummary("Gets the weather forecast for the next 5 days")]
        public IEnumerable<WeatherForecast> GetV1()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet(Name = "GetWeatherForecastV2")]
        [MapToApiVersion("2.0")]
        public IEnumerable<WeatherForecast> GetV2()
        {
            return Enumerable.Range(1, 10).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-30, 60),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)] + " (V2)"
            })
            .ToArray();
        }
    }
}
