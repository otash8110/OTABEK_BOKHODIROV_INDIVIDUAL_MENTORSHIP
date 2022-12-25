using DAL;

namespace BL
{
    public class WeatherService: IWeatherService
    {
        private readonly IWeatherManager weatherManager;
        public WeatherService(IWeatherManager weatherManager)
        {
            this.weatherManager = weatherManager;
        }

        public async Task<string> GetWeatherByCityName(string cityName)
        {
            if (!ValidateCityName(cityName))
            {
                WeatherResponse response = await this.weatherManager.FetchWeatherByCityName(cityName);
                if (response != null)
                {
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
