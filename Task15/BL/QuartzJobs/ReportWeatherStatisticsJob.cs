using BL.Enums;
using DAL.Identity;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Quartz;
using RabbitMQ.Client;
using System.Text;

namespace BL.QuartzJobs
{
    public class ReportWeatherStatisticsJob : IJob
    {
        private readonly IWeatherScheduledService weatherService;
        private readonly UserManager<ApplicationUser> userManager;

        public ReportWeatherStatisticsJob(IWeatherScheduledService weatherService,
            UserManager<ApplicationUser> userManager)
        {
            this.weatherService = weatherService;
            this.userManager = userManager;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;

            var cityNames = (IEnumerable<string>) dataMap["cityNames"];
            var userId = (string) dataMap["UserId"];
            var period = (Period) dataMap["Period"];

            var from = DateTime.Now.AddHours(-(int)period);
            var to = DateTime.Now;

            var result = await weatherService.GetWeatherHistoryForPeriodReport(cityNames, from, to);
            var user = await userManager.FindByIdAsync(userId);
            var receiverEmail = user.Email;

            var factory = new ConnectionFactory { HostName = "localhost" };
            var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare("emails", exclusive: false);

            var json = JsonConvert.SerializeObject(new
            {
                EmailAddress = receiverEmail,
                Subject = $"Weather Report for period {from} --- {to}",
                Body = result
            });
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish("", "emails", body: body);

            System.Diagnostics.Debug.WriteLine($"REPORT WEATHER EXEC: {context.FireTimeUtc}, {from}, {to}");

            return;
        }
    }
}
