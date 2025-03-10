namespace PedidosWebUpgrade.Domain.Entities
{
    public class EstatusOrden
    {
        public int IdEstatus { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int Orden { get; set; }
        public string? Icono { get; set; }
    }
}
