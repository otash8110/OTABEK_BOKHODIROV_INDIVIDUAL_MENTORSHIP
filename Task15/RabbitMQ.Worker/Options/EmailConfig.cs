namespace RabbitMQ.Worker.Options
{
    internal class EmailConfig
    {
        public string SMTPAddress { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
    }
}
