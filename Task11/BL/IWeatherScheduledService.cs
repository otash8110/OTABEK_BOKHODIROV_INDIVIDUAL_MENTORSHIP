using DAL.WeatherHistoryEntity;

namespace BL
{
    public interface IWeatherScheduledService
    {
        Task FetchAndSaveWeatherAsync(string cityName);
        Task FetchAndSaveManyWeathersAsync(IEnumerable<string> cityNames);
        IEnumerable<WeatherHistory> GetFilteredWeatherHIstory(string cityName, DateTime from, DateTime to);
    }
}
