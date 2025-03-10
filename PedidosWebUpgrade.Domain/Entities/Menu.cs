using System.ComponentModel.DataAnnotations;

namespace PedidosWebUpgrade.Domain.Entities
{
    public class Menu
    {
        public int IdMenu { get; set; }

        [Display(Name = "Padre:")]
        [Required]
        public int IdPadre { get; set; }

        [Display(Name = "Opción:")]
        [Required]
        public string? Descripcion { get; set; }

        [Display(Name = "Orden:")]
        public int Orden { get; set; }

        [Display(Name = "Icono:")]
        public string? Icon { get; set; }

        [Display(Name = "Enlace:")]
        public string? Url { get; set; }

        [Display(Name = "Activo:")]
        public bool Activo { get; set; }

        [Display(Name = "Visible:")]
        public bool Visible { get; set; }

        public List<ListaGeneral> Padres { get; set; }

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

        public bool PuedeCrear { get; set; }
        public bool PuedeConsultar { get; set; }
        public bool PuedeActualizar { get; set; }
        public bool PuedeEliminar { get; set; }
    }
}
