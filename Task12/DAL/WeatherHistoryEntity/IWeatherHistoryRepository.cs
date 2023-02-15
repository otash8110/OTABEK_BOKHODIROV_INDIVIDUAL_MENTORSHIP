namespace DAL.WeatherHistoryEntity
{
    public interface IWeatherHistoryRepository
    {
        Task Insert(WeatherHistory weatherHistory);
        Task InsertMany(IEnumerable<WeatherHistory> weatherHistoryList);
        IEnumerable<WeatherHistory> Filter(Func<WeatherHistory, bool> filter);
    }
}
