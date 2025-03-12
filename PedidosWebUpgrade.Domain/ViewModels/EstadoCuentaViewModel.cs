using PedidosWebUpgrade.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedidosWebUpgrade.Domain.ViewModels
{
    public class EstadoCuentaViewModel
    {
        public List<ListaGeneral> Clientes { get; set; } = [];
        public List<EstadoCuenta> Movimientos { get; set; } = [];
        public string? CodigoCliente { get; set; }
    }
}
