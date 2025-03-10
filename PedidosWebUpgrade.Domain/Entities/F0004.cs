using System.ComponentModel.DataAnnotations;

namespace PedidosWebUpgrade.Domain.Entities
{
    public class F0004
    {
        [Display(Name = "Código definido por el usuario:")]
        [Required]
        public string? dtrt { get; set; }

        [Display(Name = "Código del producto:")]
        [Required]
        public string?   dtsy { get; set; }


        [Display(Name = "Descripción:")]
        [Required]
        public string? dtdl01 { get; set; }
    }
}
