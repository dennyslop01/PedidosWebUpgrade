using System.ComponentModel.DataAnnotations;

namespace PedidosWebUpgrade.Domain.Entities
{
    public class ContadorPedidosPais
    {
        [Display(Name = "Año")]
        public int AnnoCurso { get; set; } = 0;

        [Display(Name = "Año Nuevo:")]
        public int AnnoNuevo { get; set; }

        [Required]
        [Display(Name = "Código:")]
        public string? CodigoPais { get; set; } = string.Empty;

        [Display(Name = "País:")]
        public string? Pais { get; set; } = string.Empty;

        [Display(Name = "Contador Actual Asignado:")]
        public int Contador { get; set; } = 0;

        [Display(Name = "Dias de Travesia:")]
        public int DiasTravesia { get; set; }

        public List<ListaGeneral> Paises { get; set; } = new List<ListaGeneral>();
    }
}
