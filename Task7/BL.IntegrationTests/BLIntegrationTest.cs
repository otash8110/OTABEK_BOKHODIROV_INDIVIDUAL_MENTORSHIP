using BL.HttpService;
using BL.Validation;
using DAL;
using Microsoft.Extensions.Configuration;

namespace BL.IntegrationTests
{
    public class BLIntegrationTest
    {
        private readonly string apiKey = "ca399f8aa2c4f7fec1611d935942a7d4";
        private readonly string apiKeySecond = "adcfbeac4c0b4d2eb7c170313230401";
        

        [Theory]
        [InlineData("Tashkent")]
        [InlineData("Moscow")]
        public async void GetWeatherByCityNameAsync_WhenCalled_ReturnsTemperatureString(string cityName)
        {
            var weatherHttpService = new WeatherHttpClient(apiKey, apiKeySecond);
            var weatherRepository = new WeatherRepository();
            var weatherValidatorService = new ValidationService();

            var weatherService = new WeatherService(
                weatherHttpService,
                weatherRepository,
                weatherValidatorService);

            var weatherResult = await weatherService.GetWeatherByCityNameAsync(cityName);

            Assert.Matches("[0-9]+,[0-9]+", weatherResult);
        }

        [Theory]
        [InlineData("Tashkent", 2)]
        [InlineData("Moscow", 3)]
        public async void GetFutureWeatherByCityNameAsync_WhenCalled_ReturnsTemperatureString(string cityName, int days)
        {
            var weatherHttpService = new WeatherHttpClient(apiKey, apiKeySecond);
            var weatherRepository = new WeatherRepository();
            var weatherValidatorService = new ValidationService();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var weatherService = new WeatherService(
                weatherHttpService,
                weatherRepository,
                weatherValidatorService);

            var weatherResult = await weatherService.GetFutureWeatherByCityNameAsync(cityName, days, configuration);

            Assert.Matches("(:.[0-9].[0-9])|(: [0-9].)|(: -[0-9].[0-9])|(: -[0-9].)", weatherResult);
        }

        [Theory]
        [InlineData("Moscow", "Tashkent")]
        [InlineData("Moscow", "Tashkent", "Ohio")]
        public async void GetMaxWeatherByCityNamesAsync_WhenCalled_ReturnsMaxTemperatureStringWithDebug(params string[] cityNames)
        {
            var weatherHttpService = new WeatherHttpClient(apiKey, apiKeySecond);
            var weatherRepository = new WeatherRepository();
            var weatherValidatorService = new ValidationService();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var weatherService = new WeatherService(
                weatherHttpService,
                weatherRepository,
                weatherValidatorService);

            var weatherResult = await weatherService.GetMaxWeatherByCityNamesAsync(cityNames, configuration);

            Assert.Matches("City: [A-z]+ : (([-0-9]+,[0-9]+)|([0-9])+). Timer: [0-9]+ ms", weatherResult);
            Assert.Matches("City with the highest temperature (([-0-9]+,[0-9]+)|([0-9]+)) C: [A-z]+. Successful request count: [0-9]+, failed: [0-9]+.", weatherResult);
        }

    }
}