using BL.HttpService;
using BL.Validation;
using DAL;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace BL
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherHttpClient weatherHttpClient;
        private readonly IWeatherRepository weatherRepository;
        private readonly IValidation validationService;

        public WeatherService(IWeatherHttpClient weatherHttpClient,
            IWeatherRepository weatherRepository,
            IValidation validationService)
        {
            this.weatherHttpClient = weatherHttpClient;
            this.weatherRepository = weatherRepository;
            this.validationService = validationService;
        }

        public async Task<string> GetFutureWeatherByCityNameAsync(string cityName, int days, IConfiguration configuration)
        {
            if (!validationService.ValidateCityName(cityName) && validationService.ValidateMinMaxDays(days, configuration))
            {
                WeatherForecast response =
                    await weatherHttpClient.FetchFutureWeatherAsync(cityName, days);
                var dayNumber = 1;
                var weatherStringResult = "";

                weatherStringResult += $"{cityName} weather forecast:\n";

                if (response != null)
                {
                    foreach (var item in response.Forecast.Forecastday)
                    {
                        var temperatureComment = GenerateTemperatureComment(item.Day.Avgtemp_c);

                        weatherStringResult += $"Day {dayNumber}: {item.Day.Avgtemp_c}. {temperatureComment}\n";
                        dayNumber++;
                    }

                    return weatherStringResult;
                }
                else
                    throw new Exception("Could not fetch weather information");
            }
            else
                throw new ArgumentException("City name is empty or null or days number is inaccurate");
        }

        public async Task<string> GetManyWeatherByCityNamesAsync(string[] cityNames, IConfiguration configuration)
        {
            bool isDebugShown;
            bool.TryParse(configuration["IncludeDebugInfo"], out isDebugShown);
            var maxTemperatureWeather = new WeatherAsyncResult();
            string stringResult = "";
            var weatherTasks = new List<Task<WeatherAsyncResult>>();
            var successTasks = 0;
            var failTasks = 0;

            foreach (var cityName in cityNames)
            {
                var fetchWeatherTask = GetWeatherWithOperationResultAsync(cityName);
                weatherTasks.Add(fetchWeatherTask);
            }

            await Task.WhenAll(weatherTasks);

            for (int i = 0; i < weatherTasks.Count; i++)
            {
                var result = await weatherTasks[i];


                if (result.Weather == null)
                {
                    failTasks++;
                    if (isDebugShown)
                    {
                        stringResult += $"City: {result.CityName}. Error: {result.Error}. Timer: {result.Time} ms\n";
                    }
                    continue;
                }

                if (isDebugShown)
                {
                    stringResult += $"City: {result.CityName} : {result.Weather.Main.Temp}. Timer: {result.Time} ms\n";
                }
                successTasks++;

                if (maxTemperatureWeather.Weather is null || result.Weather.Main.Temp > maxTemperatureWeather.Weather.Main.Temp)
                {
                    maxTemperatureWeather = result;
                }
            }

            stringResult += $"\nCity with the highest temperature {maxTemperatureWeather.Weather.Main.Temp} C: {maxTemperatureWeather.CityName}. " +
                $"Successful request count: {successTasks}, failed: {failTasks}.";

            return stringResult;
        }

        public async Task<string> GetWeatherByCityNameAsync(string cityName)
        {
            if (!validationService.ValidateCityName(cityName))
            {
                Weather response = await weatherHttpClient.FetchWeatherByCityNameAsync(cityName);
                if (response != null)
                {
                    weatherRepository.Insert(response);
                    var temperatureComment = GenerateTemperatureComment(response.Main.Temp);

                    return $"In {cityName} {response.Main.Temp} °C. {temperatureComment}";
                }

                else
                    throw new Exception("Could not fetch weather information");
            }
            else
                throw new ArgumentException("City name is empty or null");
        }

        private string GenerateTemperatureComment(float value)
        {
            return value switch
            {
                < 0 => "Dress warmly.",
                >= 0 and < 20 => "It's fresh.",
                >= 20 and < 30 => "Good weather.",
                >= 30 => "it's time to go to the beach.",
                _ => throw new ArgumentOutOfRangeException(nameof(value), $"Unexpected temperature value")
            };
        }

        private async Task<WeatherAsyncResult> GetWeatherWithOperationResultAsync(string cityName)
        {
            var result = new WeatherAsyncResult();
            result.CityName = cityName;

            var watch = new Stopwatch();
            watch.Start();

            try
            {
                result.Weather = await weatherHttpClient.FetchWeatherByCityNameAsync(cityName);
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
            }
            finally
            {
                watch.Stop();
                result.Time = watch.ElapsedMilliseconds;
            }

            return result;
        }
    }
}
