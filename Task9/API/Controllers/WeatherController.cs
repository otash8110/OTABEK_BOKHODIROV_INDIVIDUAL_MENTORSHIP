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
        public WeatherController(IValidation validation, IWeatherRepository weatherRepository, IWeatherHttpClient httpClient)
        {
            weatherService = new WeatherService(httpClient, weatherRepository, validation);
        }

        [HttpGet]
        public async Task<ActionResult> CurrentWeather(string cityName)
        {
            var result = await weatherService.GetWeatherByCityNameAsync(cityName);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult> FutureWeather(string cityName)
        {
            var result = await weatherService.GetFutureWeatherByCityNameAsync(cityName);
            return Ok(result);
        }
    }
}
