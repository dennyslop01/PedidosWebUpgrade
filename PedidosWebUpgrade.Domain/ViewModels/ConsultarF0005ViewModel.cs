using PedidosWebUpgrade.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedidosWebUpgrade.Domain.ViewModels
{
    public class ConsultarF0005ViewModel
    {
        public List<F0005> ListF0005 = [];

        [Display(Name = "Código de producto:")]
        public string CodigoProducto { get; set; } = string.Empty;

        [Display(Name = "Código definido por el usuario:")]
        public string CodigoUsuario { get; set; } = string.Empty;

        public List<ListaGeneral> Productos { get; set; } = [];

        public List<ListaGeneral> CodigoUsuarios { get; set; } = [];
    }
}
