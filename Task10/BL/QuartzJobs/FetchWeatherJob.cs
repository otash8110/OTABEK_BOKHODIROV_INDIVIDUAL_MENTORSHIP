using Quartz;

namespace BL.QuartzJobs
{
    public class FetchWeatherJob : IJob
    {
        private readonly IWeatherScheduledService weatherService;

        public FetchWeatherJob(IWeatherScheduledService weatherService)
        {
            this.weatherService = weatherService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;

            string jobSays = dataMap.GetString("cityName");

            System.Diagnostics.Debug.WriteLine($"EXECUTED JOB, {context.FireTimeUtc}, {jobSays}");

            await weatherService.FetchAndSaveWeatherAsync(jobSays);
        }
    }
}
