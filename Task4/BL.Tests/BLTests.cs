using BL.HttpService;
using BL.Validation;
using DAL;
using Moq;

namespace BL.Tests
{
    public class BLTests
    {
        private readonly string cityName = "Tashkent";
        private readonly Mock<IWeatherHttpClient> httpClientMock;
        private readonly Mock<IWeatherRepository> weatherRepositoryMock;
        private readonly IValidation validation;

        public BLTests()
        {
            httpClientMock = new Mock<IWeatherHttpClient>();
            weatherRepositoryMock = new Mock<IWeatherRepository>();
            httpClientMock.Setup(x => x.FetchWeatherByCityNameAsync(cityName)).Returns(FetchResultString);

            validation = new ValidationService();
        }

        [Fact]
        public async void GetWeatherByCityNameAsync_WhenCalled_ReturnsString()
        {
            var weatherService = new WeatherService(httpClientMock.Object, weatherRepositoryMock.Object, validation);

            var result = await weatherService.GetWeatherByCityNameAsync(cityName);

            Assert.IsType<string>(result);
        }

        [Fact]
        public async void GetWeatherByCityNameAsync_NullPassed_ReturnsArgumentException()
        {
            var weatherService = new WeatherService(httpClientMock.Object, weatherRepositoryMock.Object, validation);


            await Assert.ThrowsAsync<ArgumentException>(async () => await weatherService.GetWeatherByCityNameAsync(null));
        }

        [Fact]
        public async void GetWeatherByCityNameAsync_EmptyStringPassed_ReturnsArgumentException()
        {
            var weatherService = new WeatherService(httpClientMock.Object, weatherRepositoryMock.Object, validation);


            await Assert.ThrowsAsync<ArgumentException>(async () => await weatherService.GetWeatherByCityNameAsync(""));
        }

        private async Task<Weather> FetchResultString()
        {
            var temperature = new WeatherMainElements()
            {
                Temp = 39
            };

            return await Task.FromResult(new Weather()
            {
                Main = temperature
            });
        }
    }
}