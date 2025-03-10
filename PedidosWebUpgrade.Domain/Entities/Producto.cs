namespace PedidosWebUpgrade.Domain.Entities
{
    public class Producto
    {
        public string? Codigo { get; set; }
        public string? Descripcion { get; set; }
        public double Stock { get; set; }
        public double PrecioUno { get; set; }
        public string? Warehouse { get; set; }
        public string? Brand { get; set; }
        public string? Imagen { get; set; }
        public double CantidadIngresada { get; set; }
        public string? IdListaPrecio { get; set; }
        public string? IdShipTo { get; set; }
        public string? Moneda { get; set; }
    }
}
