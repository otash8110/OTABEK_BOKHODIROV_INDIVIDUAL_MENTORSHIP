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

            services.AddHostedService<ScheduleManager>();

            return services;
        }
    }
}
