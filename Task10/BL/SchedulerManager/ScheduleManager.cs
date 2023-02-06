using BL.QuartzJobs;
using DAL.WeatherHistoryOptionsModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Quartz;

namespace BL.SchedulerManager
{
    public class ScheduleManager : ISchedulerManager, IHostedService
    {
        private IOptionsMonitor<CitiesOption> citiesOption;
        private readonly ISchedulerFactory schedulerFactory;

        public ScheduleManager(IOptionsMonitor<CitiesOption> citiesOption,
            ISchedulerFactory schedulerFactory)
        {
            this.citiesOption = citiesOption;
            this.schedulerFactory = schedulerFactory;
            this.citiesOption.OnChange(OnChangeCallback);
        }

        public async Task ScheduleJobs()
        {
            var scheduler = await schedulerFactory.GetScheduler();

            var jobsList = new List<TriggerAndJob>();

            foreach (var i in citiesOption.CurrentValue.Cities)
            {
                var trigger = TriggerBuilder.Create()
                    .WithIdentity(i.Name)
                    .WithSimpleSchedule(x => x
                            .WithIntervalInSeconds(Convert.ToInt32(i.Timer)))
                    .Build();

                var job = JobBuilder.Create<FetchWeatherJob>()
                    .WithIdentity(i.Name)
                    .Build();

                await scheduler.ScheduleJob(job, trigger);
            }
        }

        private async void OnChangeCallback(CitiesOption citiesOption)
        {
           await ScheduleJobs();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
           await ScheduleJobs();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private record TriggerAndJob(ITrigger trigger, IJob Job);
    }

}
