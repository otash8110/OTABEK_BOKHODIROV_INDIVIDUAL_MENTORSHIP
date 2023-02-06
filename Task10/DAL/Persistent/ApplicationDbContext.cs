using DAL.WeatherHistoryEntity;
using Microsoft.EntityFrameworkCore;

namespace DAL.Persistent
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<WeatherHistory> WeatherHistory { get; set; }
    }
}
