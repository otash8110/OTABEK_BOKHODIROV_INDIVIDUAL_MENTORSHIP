using DAL.Persistent;

namespace DAL.WeatherHistoryEntity
{
    public class WeatherHistoryRepository : IWeatherHistoryRepository
    {
        private readonly ApplicationDbContext context;
        public WeatherHistoryRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public void Insert(WeatherHistory weatherHistory)
        {
            context.Set<WeatherHistory>().Add(weatherHistory);
            context.SaveChanges();
        }

        public void InsertMany(IEnumerable<WeatherHistory> weatherHistoryList)
        {
            context.Set<WeatherHistory>().AddRange(weatherHistoryList);
            context.SaveChanges();
        }
    }
}
