using BL.HttpService;
using BL.Validation;
using DAL;
using Microsoft.Extensions.Configuration;
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

        [Theory]
        [InlineData(1)]
        [InlineData(4)]
        public async void GetFutureWeatherByCityNameAsync_WhenCalled_ReturnsString(int days)
        {
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetupGet(x => x["MinDays"]).Returns("1");
            mockConfiguration.SetupGet(x => x["MaxDays"]).Returns("5");
            var mockHttpClient = new Mock<IWeatherHttpClient>();
            mockHttpClient.Setup(x => x.FetchFutureWeatherAsync(cityName, days)).Returns(FetchFutureWeather);
            var weatherService = new WeatherService(mockHttpClient.Object, weatherRepositoryMock.Object, validation);
            

            var result = await weatherService.GetFutureWeatherByCityNameAsync(cityName,
                days,
                mockConfiguration.Object);

            Assert.IsType<string>(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public async void GetFutureWeatherByCityNameAsync_WrongDayPassed_ThrowsArgumentException(int days)
        {
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetupGet(x => x["MinDays"]).Returns("1");
            mockConfiguration.SetupGet(x => x["MaxDays"]).Returns("5");
            var mockHttpClient = new Mock<IWeatherHttpClient>();
            mockHttpClient.Setup(x => x.FetchFutureWeatherAsync(cityName, days)).Returns(FetchFutureWeather);
            var weatherService = new WeatherService(mockHttpClient.Object, weatherRepositoryMock.Object, validation);


            await Assert.ThrowsAsync<ArgumentException>(async () => await weatherService.GetFutureWeatherByCityNameAsync(cityName,
                days,
                mockConfiguration.Object));
        }

        private async Task<Weather> FetchResultString()
        {
            var temperature = new WeatherMain()
            {
                Temp = 39
            };

            return await Task.FromResult(new Weather()
            {
                Main = temperature
            });
        }

        private async Task<WeatherForecast> FetchFutureWeather()
        {
            var days = new List<Days>();

            days.Add(new Days() { 
                Day = new Day()
                {
                    Avgtemp_c = 12
                }
            });

            days.Add(new Days()
            {
                Day = new Day()
                {
                    Avgtemp_c = 0
                }
            });
            
            days.Add(new Days()
            {
                Day = new Day()
                {
                    Avgtemp_c = -9
                }
            });

            var forecastDay = new ForecastDay()
            {
                Forecastday = days
            };

            var forecast = new WeatherForecast()
            {
                Forecast = forecastDay
            };

            return await Task.FromResult(forecast);
        }
    }
}