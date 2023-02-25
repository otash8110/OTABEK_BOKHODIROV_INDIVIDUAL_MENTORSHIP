using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Worker.Models;
using RabbitMQ.Worker.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace RabbitMQ.Worker
{
    internal class Program
    {
        public static IConfiguration configuration;
        private static RabbitMqConfig mqConfig;
        private static EmailConfig emailConfig;

        static void Main(string[] args)
        {
            ConfigureOptions();

            var factory = new ConnectionFactory { HostName = mqConfig.HostName};
            var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(mqConfig.EmailQueue, exclusive: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var email = JsonConvert.DeserializeObject<Email>(message);

                Console.WriteLine(message);

                using var client = new SmtpClient(emailConfig.SMTPAddress, emailConfig.Port)
                {
                    Credentials = new NetworkCredential(emailConfig.Email, emailConfig.Password),
                    EnableSsl = true
                };

                await client.SendMailAsync(emailConfig.Email, email.EmailAddress, email.Subject, email.Body);
            };

            channel.BasicConsume(queue: mqConfig.EmailQueue, autoAck: true, consumer: consumer);
            Console.ReadKey();
        }

        static void ConfigureOptions()
        {
            SetupConfiguration();
            mqConfig = new RabbitMqConfig();
            emailConfig = new EmailConfig();

            configuration.GetSection("RabbitMqConfig").Bind(mqConfig);
            configuration.GetSection("EmailConfig").Bind(emailConfig);
        }

        static void SetupConfiguration() => configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
    }
}