using BL.HttpService;
using BL.Validation;
using DAL;

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

        public async Task<string> GetFutureWeatherByCityNameAsync(string cityName, int days)
        {
            if (!validationService.ValidateCityName(cityName) && validationService.ValidateMinMaxDays(days))
            {
                WeatherForecast response =
                    await weatherHttpClient.FetchWeatherListByCoordsAsync(cityName, days);
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
