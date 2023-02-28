using System.Net.Mail;
using System.Net;

namespace BL.Email
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient smtpClient;

        public EmailService(string smtpAddress, int port, string credEmail, string credPassword)
        {
            smtpClient = new SmtpClient(smtpAddress, port)
            {
                Credentials = new NetworkCredential(credEmail, credPassword),
                EnableSsl = true
            };
        }
        public async Task SendEmail(string from, string to, string subject, string body)
        {
            await smtpClient.SendMailAsync(from, to, subject, body);
        }
    }
}
