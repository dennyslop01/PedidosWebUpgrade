namespace PedidosWebUpgrade.Domain.Entities
{
    public class Pago
    {
        public string? IdTipoPago { get; set; }
        public List<ListaGeneral> TiposPagos { get; set; } = [];
        public string? FechaOperacion { get; set; } = "";
        public string? IdTipoMoneda { get; set; }
        public List<ListaGeneral> TiposMonedas { get; set; } = [];
        public double MontoMoneda { get; set; } = 0;
        public double TasaCambio { get; set; } = 0;
        public double MontoBolivares { get; set; } = 0;
        public string? Referencia { get; set; } = "";
        public string? IdBanco { get; set; }
        public List<ListaGeneral> Bancos { get; set; } = [];
        public string Adjunto { get; set; } = "";
    }
}
