namespace PedidosWebUpgrade.Domain.Entities
{
    public class SeguimientoOrden
    {
        public int IdSeguimiento { get; set; }
        public int IdOrder { get; set; }
        public int IdEstatus { get; set; }
        public string? FechaModificacion { get; set; }
        public string? HoraModificacion { get; set; }
    }
}
