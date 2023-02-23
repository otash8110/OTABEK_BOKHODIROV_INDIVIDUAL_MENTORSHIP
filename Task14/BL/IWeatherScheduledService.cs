using DAL.WeatherHistoryEntity;

namespace BL
{
    public interface IWeatherScheduledService
    {
        Task FetchAndSaveWeatherAsync(string cityName);
        Task FetchAndSaveManyWeathersAsync(IEnumerable<string> cityNames);
        IEnumerable<WeatherHistory> GetFilteredWeatherHistory(string cityName, DateTime from, DateTime to);
        Task<string> GetWeatherHistoryForPeriodReport(IEnumerable<string> cityNames, DateTime from, DateTime to);
    }
}
