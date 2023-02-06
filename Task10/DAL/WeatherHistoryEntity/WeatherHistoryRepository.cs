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
        public async Task Insert(WeatherHistory weatherHistory)
        {
            await context.Set<WeatherHistory>().AddAsync(weatherHistory);
            context.SaveChanges();
        }

        public async Task InsertMany(IEnumerable<WeatherHistory> weatherHistoryList)
        {
            await context.Set<WeatherHistory>().AddRangeAsync(weatherHistoryList);
            context.SaveChanges();
        }
    }
}
