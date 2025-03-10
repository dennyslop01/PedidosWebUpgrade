using System.ComponentModel.DataAnnotations;

namespace PedidosWebUpgrade.Domain.Entities
{
    public class Vendedor
    {
        public int IdVendedor { get; set; }

        [Display(Name = "Nombrre:")]
        [Required]
        public string? Nombre { get; set; }

        [Display(Name = "Línea 1:")]
        public string? Linea1 { get; set; }

        [Display(Name = "Línea 2:")]
        public string? Linea2 { get; set; }

        [Display(Name = "Línea 3:")]
        public string? Linea3 { get; set; }

        [Display(Name = "Línea 4:")]
        public string? Linea4 { get; set; }

        [Display(Name = "Línea 5:")]
        public string? Linea5 { get; set; }

        [Display(Name = "Logo Orden Producción:")]
        public string? LogoOrdenProduccion { get; set; }

        [Display(Name = "Logo Proforma:")]
        public string? LogoProforma { get; set; }
    }
}
