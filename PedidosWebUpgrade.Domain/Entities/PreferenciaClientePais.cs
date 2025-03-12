using System.ComponentModel.DataAnnotations;

namespace PedidosWebUpgrade.Domain.Entities
{
    public class PreferenciaClientePais
    {
        public PreferenciaClientePais()
        {
            this.Paises = [];
            this.Clientes = [];
        }

        [Display(Name = "Paises:")]
        public List<ListaGeneral> Paises { get; set; } = [];

        [Display(Name = "Clientes:")]
        public List<ListaGeneral> Clientes { get; set; } = [];

        [Required]
        public string? CodPais { get; set; }
        public string? Pais { get; set; }

        [Required]
        public string? CodCliente { get; set; }
        public string? Cliente { get; set; }
    }
}
