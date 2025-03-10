namespace PedidosWebUpgrade.Domain.Entities
{
    public class Customer
    {
        public string? CustomerId { get; set; }
        public string? Name { get; set; }
        public double CreditLimit { get; set; }
        public string? Region { get; set; }
        public string? SalesmanId { get; set; }
        public string? Warehouse { get; set; }
        public int DiasRequerido { get; set; }
        public string? WarehouseFacturacion { get; set; }
        public string? ListaPrecio { get; set; }
        public bool FacturasPendientes { get; set; }
        public string? Email { get; set; }
        public string? CPGP { get; set; }
        public string? CodCondicionPago { get; set; }
        public string? IdListaPrecio { get; set; }
    }
}
