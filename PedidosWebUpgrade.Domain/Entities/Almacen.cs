using System.ComponentModel.DataAnnotations;

namespace PedidosWebUpgrade.Domain.Entities
{
    public class Almacen
    {
        public Almacen()
        {
            this.ListaOrdenProdupcion = new List<ListaGeneral>();
            this.ListProforma = new List<ListaGeneral>();
        }

        public int Id { get; set; }

        [Display(Name = "Código Almacen:")]
        [Required]
        public string? IdAlmacen { get; set; }

        [Display(Name = "Descripción:")]
        public string? Descripcion { get; set; }

        [Display(Name = "Modo:")]
        public string? Modo { get; set; }

        [Display(Name = "Vendedor Orden:")]
        public int IdVendedorOrden { get; set; }

        [Display(Name = "VendedorProforma:")]
        public int IdVendedorProforma { get; set; }

        public List<ListaGeneral> ListaOrdenProdupcion { get; set; }
        public List<ListaGeneral> ListProforma { get; set; }

        public string? NombreVendedorProforma { get; set; }
        public string? NombreVendedorOrderProdupcion { get; set; }
    }
}
