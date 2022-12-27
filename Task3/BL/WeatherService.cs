using BL.HttpService;
using DAL;

namespace BL
{
    public class WeatherService: IWeatherService
    {
        private readonly IWeatherHttpClient weatherHttpClient;
        private readonly IWeatherRepository weatherRepository;

        public WeatherService(IWeatherHttpClient weatherHttpClient, IWeatherRepository weatherRepository)
        {
            this.weatherHttpClient = weatherHttpClient;
            this.weatherRepository = weatherRepository;
        }

        public async Task<string> GetWeatherByCityNameAsync(string cityName)
        {
            if (!ValidateCityName(cityName))
            {
                Weather response = await this.weatherHttpClient.FetchWeatherByCityNameAsync(cityName);
                if (response != null)
                {
                    this.weatherRepository.Save(response);
                    var temperatureComment = GenerateTemperatureComment(response.Main.Temp);

                    return $"In {cityName} {response.Main.Temp} °C. {temperatureComment}";
                }

                else 
                    throw new Exception("Could not fetch weather information");
            }
            else
                throw new ArgumentException("City name is empty or null");
        }

        private bool ValidateCityName(string cityName)
        {
            return String.IsNullOrEmpty(cityName);
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
