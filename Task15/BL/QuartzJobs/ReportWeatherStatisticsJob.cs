using BL.Enums;
using BL.Options;
using DAL.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
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
        private readonly RabbitMqConfig mqConfig;


        public ReportWeatherStatisticsJob(IWeatherScheduledService weatherService,
            UserManager<ApplicationUser> userManager,
            IOptions<RabbitMqConfig> mqConfig)
        {
            this.weatherService = weatherService;
            this.userManager = userManager;
            this.mqConfig = mqConfig.Value;
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

            var factory = new ConnectionFactory { HostName = mqConfig.HostName };
            var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(mqConfig.EmailQueue, exclusive: false);

            var json = JsonConvert.SerializeObject(new
            {
                EmailAddress = receiverEmail,
                Subject = $"Weather Report for period {from} --- {to}",
                Body = result
            });
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish("", mqConfig.EmailQueue, body: body);

            System.Diagnostics.Debug.WriteLine($"REPORT WEATHER EXEC: {context.FireTimeUtc}, {from}, {to}");

            return;
        }
    }
}
