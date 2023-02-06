namespace DAL.WeatherHistoryEntity
{
    public interface IWeatherHistoryRepository
    {
        void Insert(WeatherHistory weatherHistory);
        void InsertMany(IEnumerable<WeatherHistory> weatherHistoryList);
    }
}
