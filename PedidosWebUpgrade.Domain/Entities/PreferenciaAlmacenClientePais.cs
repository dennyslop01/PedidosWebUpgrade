using System.ComponentModel.DataAnnotations;

namespace PedidosWebUpgrade.Domain.Entities
{
    public class PreferenciaAlmacenClientePais
    {
        public int IdPreferencia { get; set; }
        [Required]
        [Display(Name = "Código del Almacen:")]
        public string? IdAlmacen { get; set; }
        [Required]
        [Display(Name = "Código del Cliente:")]
        public string? IdCliente { get; set; }
        [Required]
        [Display(Name = "Código del País:")]
        public string? Codpais { get; set; }
        public string? NombrePais { get; set; }
        public string? NombreAlmacen { get; set; }
        public string? NombreCliente { get; set; }
    }
}
