using System.ComponentModel.DataAnnotations;

namespace PedidosWebUpgrade.Domain.Entities
{
    public class Sistema
    {
        public int Id { get; set; }

        [Display(Name = "Estado del sistema:")]
        [Required]
        public string? Estado { get; set; }

        [Display(Name = "Motivo:")]
        [Required]
        public string? Motivo { get; set; }

        public string? Usuario { get; set; }

        public string? Fecha { get; set; }

        public string? Hora { get; set; }

        [Display(Name = "Destinatarios:")]
        [Required]
        public string? Destinatarios { get; set; }

        public List<ListaGeneral> Estados = new List<ListaGeneral>();

        public Sistema()
        {
            Estados = new List<ListaGeneral>() { new ListaGeneral { Codigo = "A", Descripcion = "Activo" },
                                               new ListaGeneral { Codigo = "I", Descripcion = "Inactivo" } };
        }
    }
}
