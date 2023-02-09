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
            httpClientMock.Setup(x => x.FetchWeatherByCityNameAsync(cityName, CancellationToken.None)).Returns(FetchResultString);

            validation = new ValidationService();
        }

        [Fact]
        public async void GetWeatherByCityNameAsync_WhenCalled_ReturnsString()
        {
            var mockConfiguration = new Mock<IConfiguration>();
            var weatherService = new WeatherService(httpClientMock.Object, weatherRepositoryMock.Object, validation, mockConfiguration.Object);

            var result = await weatherService.GetWeatherByCityNameAsync(cityName);

            Assert.IsType<string>(result);
        }

        [Fact]
        public async void GetWeatherByCityNameAsync_NullPassed_ReturnsArgumentException()
        {
            var mockConfiguration = new Mock<IConfiguration>();
            var weatherService = new WeatherService(httpClientMock.Object, weatherRepositoryMock.Object, validation, mockConfiguration.Object);


            await Assert.ThrowsAsync<ArgumentException>(async () => await weatherService.GetWeatherByCityNameAsync(null));
        }

        [Fact]
        public async void GetWeatherByCityNameAsync_EmptyStringPassed_ReturnsArgumentException()
        {
            var mockConfiguration = new Mock<IConfiguration>();
            var weatherService = new WeatherService(httpClientMock.Object, weatherRepositoryMock.Object, validation, mockConfiguration.Object);


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
            var weatherService = new WeatherService(mockHttpClient.Object, weatherRepositoryMock.Object, validation, mockConfiguration.Object);
            

            var result = await weatherService.GetFutureWeatherByCityNameAsync(cityName,
                days);

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
            var weatherService = new WeatherService(mockHttpClient.Object, weatherRepositoryMock.Object, validation, mockConfiguration.Object);


            await Assert.ThrowsAsync<ArgumentException>(async () => await weatherService.GetFutureWeatherByCityNameAsync(cityName,
                days));
        }

        [Theory]
        [InlineData("Moscow", "Tashkent")]
        [InlineData("Moscow", "Tashkent", "Ohio")]
        public async void GetMaxWeatherByCityNamesAsync_WhenCalled_ReturnsStringWithDebugInfo(params string[] cityNames)
        {
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetupGet(x => x["IncludeDebugInfo"]).Returns("true");
            var mockHttpClient = new Mock<IWeatherHttpClient>();
            mockHttpClient.Setup(x => x.FetchWeatherByCityNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(FetchResultString);
            var weatherService = new WeatherService(mockHttpClient.Object, weatherRepositoryMock.Object, validation, mockConfiguration.Object);

            var weatherResult = await weatherService.GetMaxWeatherByCityNamesAsync(cityNames);

            Assert.Matches("City: [A-z]+ : (([-0-9]+,[0-9]+)|([0-9])+). Timer: [0-9]+ ms", weatherResult);
            Assert.Matches("City with the highest temperature (([-0-9]+,[0-9]+)|([0-9]+)) C: [A-z]+. Successful request count: [0-9]+, failed: [0-9]+.", weatherResult);
        }

        [Theory]
        [InlineData("")]
        public async void GetMaxWeatherByCityNamesAsync_EmptyCitySent_ReturnsErrorMessage(params string[] cityNames)
        {
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetupGet(x => x["IncludeDebugInfo"]).Returns("true");
            var mockHttpClient = new Mock<IWeatherHttpClient>();
            mockHttpClient.Setup(x => x.FetchWeatherByCityNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(FetchResultString);
            var weatherService = new WeatherService(mockHttpClient.Object, weatherRepositoryMock.Object, validation, mockConfiguration.Object);

            var weatherResult = await weatherService.GetMaxWeatherByCityNamesAsync(cityNames);

            Assert.Matches("Error, no successful requests. Failed requests count: [0-9]+.", weatherResult);
        }

        [Theory]
        [InlineData("")]
        public async void GetMaxWeatherByCityNamesAsync_WrongCityNameSent_ReturnsErrorMessage(params string[] cityNames)
        {
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetupGet(x => x["IncludeDebugInfo"]).Returns("true");
            var mockHttpClient = new Mock<IWeatherHttpClient>();
            mockHttpClient.Setup(x => x.FetchWeatherByCityNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Throws(new Exception("City not found"));
            var weatherService = new WeatherService(mockHttpClient.Object, weatherRepositoryMock.Object, validation, mockConfiguration.Object);

            var weatherResult = await weatherService.GetMaxWeatherByCityNamesAsync(cityNames);

            Assert.Matches("Error, no successful requests. Failed requests count: [0-9]+.", weatherResult);
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