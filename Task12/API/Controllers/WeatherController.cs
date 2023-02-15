using BL;
using BL.HttpService;
using BL.Validation;
using DAL;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : Controller
    {
        private readonly IWeatherService weatherService;
        private readonly IWeatherScheduledService weatherScheduledService;
        public WeatherController(IValidation validation,
            IWeatherRepository weatherRepository,
            IWeatherHttpClient httpClient,
            IConfiguration configuration,
            IWeatherScheduledService weatherScheduledService)
        {
            weatherService = new WeatherService(httpClient, weatherRepository, validation, configuration);
            this.weatherScheduledService = weatherScheduledService;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> CurrentWeather(string cityName)
        {
            var result = await weatherService.GetWeatherByCityNameAsync(cityName);
            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> FutureWeather(string cityName, int days)
        {
            var result = await weatherService.GetFutureWeatherByCityNameAsync(cityName, days);
            return Ok(result);
        }

        [HttpGet("[action]")]
        public IActionResult HistoryWeather(string cityName, DateTime from, DateTime to)
        {
            try
            {
                var result = weatherScheduledService.GetFilteredWeatherHistory(cityName, from, to);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
