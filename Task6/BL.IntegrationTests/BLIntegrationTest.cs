using BL.HttpService;
using BL.Validation;
using DAL;

namespace BL.IntegrationTests
{
    public class BLIntegrationTest
    {
        private readonly string apiKey = "ca399f8aa2c4f7fec1611d935942a7d4";

        [Theory]
        [InlineData("Tashkent")]
        [InlineData("Moscow")]
        public async void GetWeatherByCityNameAsync_WhenCalled_ReturnsTemperatureString(string cityName)
        {
            var weatherHttpService = new WeatherHttpClient(apiKey);
            var weatherRepository = new WeatherRepository();
            var weatherValidatorService = new ValidationService();

            var weatherService = new WeatherService(
                weatherHttpService,
                weatherRepository,
                weatherValidatorService);

            var weatherResult = await weatherService.GetWeatherByCityNameAsync(cityName);

            Assert.Matches("[0-9]+,[0-9]+", weatherResult);
        }
    }
}