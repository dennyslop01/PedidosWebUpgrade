namespace PedidosWebUpgrade.Domain.Entities
{
    public class Email
    {
        public string? From { get; set; }
        public string? To { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }
        public string? Host { get; set; }
        public int Port { get; set; }
        public string? footer { get; set; }
        public string? CopyTo { get; set; }
        public string? UseSSL { get; set; }
        public string? UseAuth { get; set; }
    }
}
