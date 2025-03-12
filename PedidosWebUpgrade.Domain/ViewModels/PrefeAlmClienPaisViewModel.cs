using PedidosWebUpgrade.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedidosWebUpgrade.Domain.ViewModels
{
    public class PrefeAlmClienPaisViewModel
    {

        public PrefeAlmClienPaisViewModel()
        {
            this.ListaPaises = new List<ListaGeneral>();
            this.ListaClientes = new List<ListaGeneral>();
            this.ListaAlmacenes = new List<ListaGeneral>();
            this.PrefAlmClientPaisList = new List<PreferenciaAlmacenClientePais>();
        }

        public List<PreferenciaAlmacenClientePais> PrefAlmClientPaisList { get; set; }

        public int IdPreferencia { get; set; }
        [Required]
        [Display(Name = "Código del Almacen:")]
        public string? IdAlmacen { get; set; }
        [Required]
        [Display(Name = "Código del Cliente:")]
        public string? IdCliente { get; set; }
        [Required]
        [Display(Name = "Código del País:")]
        public string? Codpais { get; set; }

        public List<ListaGeneral> ListaPaises { get; set; }
        public List<ListaGeneral> ListaClientes { get; set; }
        public List<ListaGeneral> ListaAlmacenes { get; set; }

    }
}
