using BL.HttpService;
using BL.Validation;
using DAL;
using Microsoft.Extensions.Configuration;

namespace BL
{
    public class WeatherService: IWeatherService
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
            Weather maxTemperatureWeather = new Weather();
            string stringResult = "";
            var weatherTasks = new List<Task<Weather>>();
            foreach (var cityName in cityNames)
            {
                var fetchWeatherTask = weatherHttpClient.FetchWeatherByCityNameAsync(cityName);
                weatherTasks.Add(fetchWeatherTask);
            }

            await Task.WhenAll(weatherTasks);

            for (int i = 0; i < weatherTasks.Count; i++)
            {
                var result = await weatherTasks[i];
                if (result == null)
                {
                    throw new Exception("Could not fetch weather information");
                }

                if (maxTemperatureWeather.Main is null || result.Main.Temp > maxTemperatureWeather.Main.Temp)
                {
                    maxTemperatureWeather = result;
                }

                stringResult += $"City: {result.CityName} :{result.Main.Temp}. Timer: ms\n";
            }

            stringResult += $"\nCity with the highest temperature {maxTemperatureWeather.Main.Temp} C: {maxTemperatureWeather.CityName}. " +
                $"Successful request count: {1}, failed: {1}.";

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
    }
}
