namespace PedidosWebUpgrade.Domain.Entities
{
    public class Descuento
    {
        public double BaseImponibleCesta { get; set; }
        public double ImpuestoCesta { get; set; }
        public double DescuentoCesta { get; set; }
        public double TotalPagarCesta { get; set; }
    }
}
