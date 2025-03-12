using System.ComponentModel.DataAnnotations;

namespace PedidosWebUpgrade.Domain.Entities
{
    public class Perfil
    {
        [Key]
        public int IdPerfil { get; set; }

        [Display(Name = "Perfil:")]
        [Required]
        public string? Nombre { get; set; }

        [Display(Name = "Descripción:")]
        [Required]
        public string? Descripcion { get; set; }

        [Display(Name = "Activo:")]
        public bool Activo { get; set; }

        public string? DescripcionActivo
        {
            get
            {
                switch (Activo)
                {
                    case true: return "ACTIVO";
                    default: return "INACTIVO";
                }
            }

        }
        public List<PerfilMenu> OpcionesMenu { get; set; } = [];
    }
}
