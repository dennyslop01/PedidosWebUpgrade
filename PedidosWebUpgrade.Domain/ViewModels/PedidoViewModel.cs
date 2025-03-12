using PedidosWebUpgrade.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedidosWebUpgrade.Domain.Entities
{
    public class PedidoViewModel
    {
        public List<ListaGeneral> Sucursales { get; set; } = [];
        public List<ListaGeneral> Clientes { get; set; } = [];
        public List<ListaGeneral> ShipTo { get; set; } = [];
        public List<ListaGeneral> Categorias { get; set; } = [];
        public List<ListaGeneral> CabecListaPrecios { get; set; } = [];
        public List<Producto> Productos { get; set; } = [];
        public EncabezadoPedido Encabezado { get; set; } = new EncabezadoPedido();
        public string CodigoCliente { get; set; } = string.Empty;
        public string CodigoShipTo { get; set; } = string.Empty;
        public string CodigoCategoria { get; set; } = string.Empty;
        public string CodigoSucursal { get; set; } = string.Empty;
        public string CodigoCabecListaPrecios { get; set; } = string.Empty;
        public int? TipoPedido { get; set; } = 1;
    }

    public class EncabezadoPedido
    {
        public int IdOrden { get; set; } = 0;
        public double NumeroCajas { get; set; } = 0;
        public double MontoTotal { get; set; } = 0;
        public double TotalDescuento { get; set; } = 0;
        public string Moneda { get; set; } = string.Empty;
        public double ImpuestoTotal { get; set; }
    }


    public class MotivosF0005
    {

        [Display(Name = "Código del motivo:")]
        public List<ListaGeneral> Motivos { get; set; } = [];
        public string? IdMotivo { get; set; }

    }
}
