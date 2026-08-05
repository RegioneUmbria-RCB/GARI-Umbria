namespace AgronicaCoreAPI.Messaging.Infrastructure
{
    public class RabbitMqOptions
    {
        public const string SectionName = "RabbitMQ";

        public bool Enabled { get; set; } = false;
        public string Hostname { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
        public string ExchangeRequests { get; set; } = "sync.requests";
        public string ExchangeResponses { get; set; } = "sync.responses";
        public string DlxExchange { get; set; } = "sync.dlx";
    }
}
