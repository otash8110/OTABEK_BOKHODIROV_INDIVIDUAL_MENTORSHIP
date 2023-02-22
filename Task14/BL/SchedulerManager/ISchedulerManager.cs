using BL.Enums;
using Microsoft.Extensions.Hosting;

namespace BL.SchedulerManager
{
    public interface ISchedulerManager : IHostedService
    {
        public Task ScheduleJobs(bool isReschedule);
        public Task ScheduleWeatherStatisticsJob(string userId, IEnumerable<string> cityNames, Period period);
    }
}
