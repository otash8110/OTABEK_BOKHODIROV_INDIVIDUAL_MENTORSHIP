using Quartz;

namespace BL.QuartzJobs
{
    public class FetchWeatherJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;

            string jobSays = dataMap.GetString("cityName");
        }
    }
}
