namespace PedidosWebUpgrade.Domain.Entities
{
    public class TipoCambio
    {
        public string? IdTipoCambio { get; set; }
        public string? MonedaOrigen { get; set; }
        public string? MonedaDestino { get; set; }
        public string? TasaCambio { get; set; }
        public DateTime Fecha { get; set; }
    }
}
