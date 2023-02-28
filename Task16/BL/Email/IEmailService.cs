namespace BL.Email
{
    public interface IEmailService
    {
        Task SendEmail(string from, string to, string subject, string body);
    }
}
