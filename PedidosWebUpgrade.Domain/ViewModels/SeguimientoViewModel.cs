using PedidosWebUpgrade.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedidosWebUpgrade.Domain.ViewModels
{
    public class SeguimientoViewModel
    {
        public string? IdOrden { get; set; }
        public List<EstatusOrden> EstatusOrden { get; set; } = [];
        public List<SeguimientoOrden> SeguimientoOrden { get; set; } = [];
    }
}
