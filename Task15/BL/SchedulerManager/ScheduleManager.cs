using BL.Enums;
using BL.QuartzJobs;
using DAL.WeatherHistoryOptionsModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl.Matchers;

namespace BL.SchedulerManager
{
    public class ScheduleManager : ISchedulerManager
    {
        private IOptionsMonitor<CitiesOption> citiesOption;
        private readonly ISchedulerFactory schedulerFactory;
        private IScheduler scheduler;
        private readonly string weatherStatisticsGroupName = "weatherStatistics";

        public ScheduleManager(IOptionsMonitor<CitiesOption> citiesOption,
            ISchedulerFactory schedulerFactory)
        {
            this.citiesOption = citiesOption;
            this.schedulerFactory = schedulerFactory;
            this.citiesOption.OnChange(OnChangeCallback);
        }

        public async Task ScheduleJobs(bool isReschedule)
        {
            if (citiesOption.CurrentValue.Cities != null)
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
        }

        public async Task ScheduleWeatherStatisticsJob(string userId, IEnumerable<string> cityNames, Period period)
        {
            if (await IsJobExistAndRunning(userId))
            {
                await DeleteScheduledJobInWeatherReport(userId);
            }

            var job = JobBuilder.Create<ReportWeatherStatisticsJob>()
                .WithIdentity(userId, weatherStatisticsGroupName)
                .Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity(userId)
                .WithSimpleSchedule(x => x
                                .WithIntervalInHours((int) period)
                                .RepeatForever())
                .StartNow()
                .Build();

            job.JobDataMap["UserId"] = userId;
            job.JobDataMap["cityNames"] = cityNames;
            job.JobDataMap["Period"] = period;

            await scheduler.ScheduleJob(job, trigger);
        }

        private async Task<bool> IsJobExistAndRunning(string userId)
        {
            var groupMatcher = GroupMatcher<JobKey>.GroupContains(weatherStatisticsGroupName);
            var jobKeys = await scheduler.GetJobKeys(groupMatcher);

            return jobKeys.Count > 0 && jobKeys.Where(n => n.Name == userId).Select(n => n.Name == userId).First() ? true : false;
        }

        private async Task<bool> DeleteScheduledJobInWeatherReport(string userId)
        {
            return await scheduler.DeleteJob(JobKey.Create(userId, weatherStatisticsGroupName));
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
