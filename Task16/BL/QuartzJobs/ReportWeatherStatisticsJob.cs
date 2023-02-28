using BL.Email;
using BL.Enums;
using BL.Options;
using DAL.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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
        private readonly EmailConfig emailConfig;
        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;

        public ReportWeatherStatisticsJob(IWeatherScheduledService weatherService,
            UserManager<ApplicationUser> userManager,
            IOptions<RabbitMqConfig> mqConfig,
            IOptions<EmailConfig> emailConfig,
            IConfiguration configuration)
        {
            this.weatherService = weatherService;
            this.userManager = userManager;
            this.mqConfig = mqConfig.Value;
            this.configuration = configuration;
            this.emailConfig = emailConfig.Value;
            emailService = new EmailService(this.emailConfig.SMTPAddress, this.emailConfig.Port, this.emailConfig.Email, this.emailConfig.Password);
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

            var msg = new
            {
                EmailAddress = receiverEmail,
                Subject = $"Weather Report for period {from} --- {to}",
                Body = result
            };

            if (bool.Parse(configuration["SeparateEmailService"]) == false)
            {
                await emailService.SendEmail(emailConfig.Email, msg.EmailAddress, msg.Subject, msg.Body);
            } else
            {
                var factory = new ConnectionFactory { HostName = mqConfig.HostName };
                var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare(mqConfig.EmailQueue, exclusive: false);

                var json = JsonConvert.SerializeObject(msg);
                var body = Encoding.UTF8.GetBytes(json);

                channel.BasicPublish("", mqConfig.EmailQueue, body: body);
            }

            return;
        }
    }
}
