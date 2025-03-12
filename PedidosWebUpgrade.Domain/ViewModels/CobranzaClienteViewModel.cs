using PedidosWebUpgrade.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedidosWebUpgrade.Domain.ViewModels
{
    public class CobranzaClienteViewModel
    {
        public string? IdCliente { get; set; }
        public string? Cliente { get; set; }
        public List<EstadoCuenta> Movimientos { get; set; } = new List<EstadoCuenta>();
        public Pago PagoCliente { get; set; } = new Pago();
        public double MontoAsignar { get; set; } = 0;
    }
}
