using System.ComponentModel.DataAnnotations;

namespace PedidosWebUpgrade.Domain.Entities
{
    public class PreferenciaItemDestino
    {
        public PreferenciaItemDestino()
        {
            this.ListaPaises = new List<ListaGeneral>();
            this.ListaProductos = new List<ListaGeneral>();
        }

        [Required]
        public int Id { get; set; }

        [Display(Name = "Código del Producto:")]
        public string? ProductId { get; set; }

        [Display(Name = "Producto:")]
        public string? Producto { get; set; }

        [Display(Name = "Código del País:")]
        public string? DestinoId { get; set; }

        [Display(Name = "País:")]
        public string? Pais { get; set; }

        [Display(Name = "Paises:")]
        public List<ListaGeneral> ListaPaises { get; set; }

        [Display(Name = "Productos:")]
        public List<ListaGeneral> ListaProductos { get; set; }
    }
}
