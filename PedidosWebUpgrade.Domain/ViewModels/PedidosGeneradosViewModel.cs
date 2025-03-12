using PedidosWebUpgrade.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedidosWebUpgrade.Domain.ViewModels
{
    public class PedidosGeneradosViewModel
    {
        public List<ListaGeneral> Clientes { get; set; } = new List<ListaGeneral>();
        public string? CodigoCliente { get; set; }

        public string? FechaDesde { get; set; }
        public string? FechaHasta { get; set; }
        public string? NumeroPedido { get; set; }
        public List<ListaGeneral> Estatus { get; set; } = new List<ListaGeneral>();
        public string? CodigoEstatus { get; set; }

        public List<Orders> Ordenes { get; set; } = new List<Orders>();

        public SeguimientoViewModel SeguimientoPedido { get; set; } = new SeguimientoViewModel();

        public List<HistoriaOrder> PedidosHistoricos { get; set; } = new List<HistoriaOrder>();
    }

}
