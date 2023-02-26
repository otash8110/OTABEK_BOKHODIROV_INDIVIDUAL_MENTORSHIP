using DAL.Persistent;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace DAL.WeatherHistoryEntity
{
    public class WeatherHistoryRepository : IWeatherHistoryRepository
    {
        private readonly ApplicationDbContext context;
        public WeatherHistoryRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<WeatherHistory> Filter(Func<WeatherHistory, bool> filter)
        {
            return context.WeatherHistory.Where(filter).ToList();
        }

        public async Task<IEnumerable<WeatherHistory>> GetAll()
        {
            return await context.WeatherHistory.ToListAsync();
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
