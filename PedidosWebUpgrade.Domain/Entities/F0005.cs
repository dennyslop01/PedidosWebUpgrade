using System.ComponentModel.DataAnnotations;

namespace PedidosWebUpgrade.Domain.Entities
{
    public class F0005
    {
        [Display(Name = "Código:")]
        [Required]
        public string? drky { get; set; }

        [Display(Name = "Código definido por el usuario:")]
        [Required]
        public string? drrt { get; set; }

        [Display(Name = "Código de producto:")]
        [Required]
        public string? drsy { get; set; }

        [Display(Name = "Descripción 01:")]
        [Required]
        public string? drdl01 { get; set; }

        [Display(Name = "Descripción 02:")]
        public string? drdl02 { get; set; }

        [Display(Name = "Uso especial:")]
        public string? drsphd { get; set; }
    }
}
