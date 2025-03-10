namespace PedidosWebUpgrade.Domain.Entities
{
    public class Salesman
    {
        public string? Salesmanid { get; set; }
        public string? Name { get; set; }
        public string? Region { get; set; }
        public int TopSerial { get; set; }
        public int BottomSerial { get; set; }
        public string? Comments { get; set; }
        public int LastSerial { get; set; }
        public int Logged { get; set; }
        public string? Sessionid { get; set; }
        public int Movil { get; set; }
        public string? Mail { get; set; }
        public string? MailCoordinador { get; set; }
        public int Estado { get; set; }
    }
}
