using BL.QuartzJobs;
using BL.SchedulerManager;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace BL
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddBLServices(this IServiceCollection services)
        {

            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();

            });

            services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
            });

            services.AddScoped<IWeatherScheduledService, WeatherSchedulerService>();
            services.AddSingleton<ScheduleManager>();
            services.AddHostedService(x => x.GetService<ScheduleManager>());

            return services;
        }
    }
}
