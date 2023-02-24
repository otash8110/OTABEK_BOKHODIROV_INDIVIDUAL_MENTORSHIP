using BL;
using BL.Enums;
using BL.HttpService;
using BL.SchedulerManager;
using BL.Validation;
using DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : Controller
    {
        private readonly IWeatherService weatherService;
        private readonly IWeatherScheduledService weatherScheduledService;
        private readonly ScheduleManager schedulerManager;
        public WeatherController(IValidation validation,
            IWeatherRepository weatherRepository,
            IWeatherHttpClient httpClient,
            IConfiguration configuration,
            IWeatherScheduledService weatherScheduledService,
            ScheduleManager schedulerManager)
        {
            weatherService = new WeatherService(httpClient, weatherRepository, validation, configuration);
            this.weatherScheduledService = weatherScheduledService;
            this.schedulerManager = schedulerManager;
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost("[action]")]
        public async Task<IActionResult> SubscribeUser(string userId, string[] cityNames, Period period)
        {
            try
            {
                await schedulerManager.ScheduleWeatherStatisticsJob(userId, cityNames, period);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
