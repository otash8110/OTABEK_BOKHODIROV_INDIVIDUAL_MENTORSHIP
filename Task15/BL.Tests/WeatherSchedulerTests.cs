using BL.HttpService;
using DAL;
using DAL.WeatherHistoryEntity;
using Moq;
using System.Diagnostics;

namespace BL.Tests
{
    public class WeatherSchedulerTests
    {

        [Theory]
        [InlineData("Tashkent", "2023-02-08T22:00:00.000Z", "2023-02-09T23:59:00.000Z")]
        public void GetFilteredWeatherHistory_WhenCalled_ReturnsWeatherHistory(string cityName, DateTime from, DateTime to)
        {
            var weatherRepositoryMock = new Mock<IWeatherHistoryRepository>();
            weatherRepositoryMock.Setup(x => x.Filter(It.IsAny<Func<WeatherHistory, bool>>()))
                .Returns(TestUtils.GetWeatherHistory);
            var weatherHttpClientMock = new Mock<IWeatherHttpClient>();
            var weatherSchedulerService = new WeatherSchedulerService(weatherRepositoryMock.Object, weatherHttpClientMock.Object);

            var result = weatherSchedulerService.GetFilteredWeatherHistory(cityName, from, to);

            Assert.IsType<List<WeatherHistory>>(result);
        }

        [Theory]
        [InlineData("Tashkent", "0001-01-01", "0001-01-01")]
        public void GetFilteredWeatherHistory_WhenWrongDatePassed_ReturnsArgumentOutOfRangeException(string cityName, DateTime from, DateTime to)
        {
            var weatherRepositoryMock = new Mock<IWeatherHistoryRepository>();
            weatherRepositoryMock.Setup(x => x.Filter(It.IsAny<Func<WeatherHistory, bool>>()))
                .Returns(TestUtils.GetWeatherHistory);
            var weatherHttpClientMock = new Mock<IWeatherHttpClient>();
            var weatherSchedulerService = new WeatherSchedulerService(weatherRepositoryMock.Object, weatherHttpClientMock.Object);

            Assert.Throws<ArgumentOutOfRangeException>(() => weatherSchedulerService.GetFilteredWeatherHistory(cityName, from, to));
        }
    }

    public static class TestUtils
    {
        public static IEnumerable<WeatherHistory> GetWeatherHistory(Func<WeatherHistory, bool> filter)
        {
            var weatherHistoryList = new List<WeatherHistory>()
            {
                new WeatherHistory()
                {
                    Id = 1,
                    Name = "Tashkent",
                    Temperature = 3,
                    Time = new DateTime(2023, 2, 9, 22, 5, 0)
                },
                new WeatherHistory()
                {
                    Id = 2,
                    Name = "Tashkent",
                    Temperature = 5,
                    Time = new DateTime(2023, 2, 9, 21, 5, 0)
                },
                new WeatherHistory()
                {
                    Id = 3,
                    Name = "Moscow",
                    Temperature = -5,
                    Time = new DateTime(2023, 2, 9, 21, 5, 0)
                }
            };

            return weatherHistoryList.Where(filter).ToList();
        }
    }
}
