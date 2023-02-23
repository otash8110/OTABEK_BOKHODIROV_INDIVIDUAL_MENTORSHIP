using BL.Enums;
using Microsoft.Extensions.Logging;
using Quartz;

namespace BL.QuartzJobs
{
    public class ReportWeatherStatisticsJob : IJob
    {
        private readonly IWeatherScheduledService weatherService;

        public ReportWeatherStatisticsJob(IWeatherScheduledService weatherService)
        {
            this.weatherService = weatherService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;

            var cityNames = (IEnumerable<string>) dataMap["cityNames"];
            var userId = dataMap["UserId"];
            var period = (Period) dataMap["Period"];

            var from = DateTime.Now.AddHours(-(int)period);
            var to = DateTime.Now;

            var result = await weatherService.GetWeatherHistoryForPeriodReport(cityNames, from, to);

            System.Diagnostics.Debug.WriteLine($"REPORT WEATHER EXEC: {context.FireTimeUtc}, {from}, {to}");

            return;
        }
    }
}
