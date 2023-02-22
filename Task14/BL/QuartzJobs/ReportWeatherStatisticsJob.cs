using Microsoft.Extensions.Logging;
using Quartz;

namespace BL.QuartzJobs
{
    public class ReportWeatherStatisticsJob : IJob
    {
        public ReportWeatherStatisticsJob()
        {
        }

        public Task Execute(IJobExecutionContext context)
        {
            System.Diagnostics.Debug.WriteLine($"REPORT WEATHER EXEC: {context.FireTimeUtc}");

            return Task.CompletedTask;
        }
    }
}
