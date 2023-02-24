using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Worker.Models;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json.Serialization;

namespace RabbitMQ.Worker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost"};
            var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare("emails", exclusive: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var email = JsonConvert.DeserializeObject<Email>(message);

                Console.WriteLine(message);

                using var client = new SmtpClient("smtp.mail.ru", 587)
                {
                    Credentials = new NetworkCredential("", ""),
                    EnableSsl = true
                };

                await client.SendMailAsync("otash_baxadirov@mail.ru", email.EmailAddress, email.Subject, email.Body);
            };

            channel.BasicConsume(queue: "emails", autoAck: true, consumer: consumer);
            Console.ReadKey();
        }
    }
}