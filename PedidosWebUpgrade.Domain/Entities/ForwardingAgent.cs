using System.ComponentModel.DataAnnotations;

namespace PedidosWebUpgrade.Domain.Entities
{
    public class ForwardingAgent
    {
        [Display(Name = "Código:")]
        [Required]
        public int AgenteId { get; set; }

        [Display(Name = "Nombre:")]
        [Required]
        public string? Nombre { get; set; }

        [Display(Name = "Linea 2:")]
        public string? Linea2 { get; set; }

        [Display(Name = "Linea 3:")]
        public string? Linea3 { get; set; }

        [Display(Name = "Linea 4:")]
        public string? Linea4 { get; set; }

        [Display(Name = "Linea 5:")]
        public string? Linea5 { get; set; }

        [Display(Name = "Linea 6:")]
        public string? Linea6 { get; set; }

        [Display(Name = "Linea 7:")]
        public string? Linea7 { get; set; }

        [Display(Name = "Linea 8:")]
        public string? Linea8 { get; set; }
    }
}
