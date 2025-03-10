namespace PedidosWebUpgrade.Domain.Entities
{
    public class EstadoCuenta
    {
        public int Id { get; set; }
        public string? CustomerId { get; set; }
        public string? TipoDocumento { get; set; }
        public string? Descripcion { get; set; }
        public string? NumeroDocumento { get; set; }
        public string? FechaDocumento { get; set; }
        public int DiasVencimiento { get; set; }
        public double MontoDocumento { get; set; }
        public double MontoPendiente { get; set; }
        public string? Observacion { get; set; }
        public string? TipoDocOriginal { get; set; }
        public string? NroDocOriginal { get; set; }
    }
}
