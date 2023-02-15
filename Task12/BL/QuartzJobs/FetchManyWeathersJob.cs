using Quartz;

namespace BL.QuartzJobs
{
    public class FetchManyWeathersJob : IJob
    {
        private readonly IWeatherScheduledService weatherService;

        public FetchManyWeathersJob(IWeatherScheduledService weatherService)
        {
            this.weatherService = weatherService;
        }
        
        public async Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;

            var cityNames = (IEnumerable<string>) dataMap["cityNames"];

            System.Diagnostics.Debug.WriteLine($"EXECUTED JOB, {context.FireTimeUtc}, {cityNames.Count()}");

            await weatherService.FetchAndSaveManyWeathersAsync(cityNames);
        }
    }
}
