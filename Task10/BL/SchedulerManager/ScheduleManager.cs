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
        private IScheduler scheduler;

        public ScheduleManager(IOptionsMonitor<CitiesOption> citiesOption,
            ISchedulerFactory schedulerFactory)
        {
            this.citiesOption = citiesOption;
            this.schedulerFactory = schedulerFactory;
            this.citiesOption.OnChange(OnChangeCallback);
        }

        public async Task ScheduleJobs(bool isReschedule)
        {
            var citiesGrouped = citiesOption.CurrentValue.Cities.OrderBy(c => c.Name).GroupBy(c => c.Timer);

            if (isReschedule)
            {
                await scheduler.Clear();
            }

            foreach (var cityTime in citiesGrouped)
            {
                if (cityTime.Count() > 1)
                {
                    await ScheduleManyCitiesJob(cityTime);
                    continue;
                }

                foreach (var city in cityTime)
                {
                    await ScheduleOneCityJob(city);
                }
            }
        }

        private async Task ScheduleManyCitiesJob(IGrouping<string, City> group)
        {
            List<string> cityNames = group.Select(c => c.Name).ToList();

            var city = new City()
            {
                Name = string.Join(string.Empty, cityNames.ToArray()),
                Timer = group.Key
            };

            var trigger = CreateTrigger(city);

            var job = JobBuilder.Create<FetchManyWeathersJob>()
                .WithIdentity(city.Name)
                .Build();
            job.JobDataMap["cityNames"] = cityNames;

            await scheduler.ScheduleJob(job, trigger);
        }

        private async Task ScheduleOneCityJob(City city)
        {
            var trigger = CreateTrigger(city);

            var job = JobBuilder.Create<FetchWeatherJob>()
                .WithIdentity(city.Name)
                .UsingJobData("cityName", city.Name)
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        private ITrigger CreateTrigger(City city)
        {
            return TriggerBuilder.Create()
                    .WithIdentity(city.Name)
                    .StartNow()
                    .WithSimpleSchedule(x => x
                            .WithIntervalInSeconds(Convert.ToInt32(city.Timer))
                            .RepeatForever())
                    .Build();
        }

        private async void OnChangeCallback(CitiesOption citiesOption)
        {
            await ScheduleJobs(true);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            this.scheduler = await schedulerFactory.GetScheduler();
            await ScheduleJobs(false);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

}
