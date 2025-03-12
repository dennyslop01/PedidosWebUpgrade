using PedidosWebUpgrade.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedidosWebUpgrade.Domain.Entities
{
    public class CestaViewModel
    {
        public CestaViewModel()
        {
            this.ShipTo = [];
            this.CabecListaPrecios = [];
            this.CondicionesDePago = [];
            this.PaisesFacturacion = [];
            this.AnnosCorrelativos = [];
        }
        public EncabezadoPedido Encabezado { get; set; } = new EncabezadoPedido();
        public string? NombreCliente { get; set; }
        public List<ListaGeneral> Condicion { get; set; } = [];
        public List<ListaGeneral> Descuento { get; set; } = [];
        public string? FechaPedido { get; set; }
        public string? FechaCorte { get; set; }
        public string? FechaRequerida { get; set; }
        public string? FechaEstimadaDespachoETD { get; set; }
        public string? FechaEstimadaLlegadaETA { get; set; }
        public string? Observaciones { get; set; }
        public List<Producto> Productos { get; set; } = [];
        public string? CodigoCondicion { get; set; }
        public int CodigoDescuento { get; set; }
        public string? CodigoCliente { get; set; }
        public bool Prepagado { get; set; }

        public string? TasaNegociacion { get; set; }

        public List<ListaGeneral> Carriers { get; set; } = [];
        public string? CodigoCarrier { get; set; }

        public List<ListaGeneral> PuertosDescargas { get; set; } = [];
        public string? CodigoPuerto { get; set; }

        public List<ListaGeneral> IncosTerms { get; set; } = [];
        public string? CodigoIncoTerms { get; set; }


        public List<ListaGeneral> Referencias01 { get; set; } = [];
        public string? CodigoReferencia01 { get; set; }

        public List<ListaGeneral> ForwardingAgents { get; set; } = [];
        public string? CodigoForwardingAgent { get; set; }

        public List<ListaGeneral> Almacenes { get; set; } = [];
        public string? Codigoalmacent { get; set; }

        public List<ListaGeneral> CondicionesDePago { get; set; } = [];
        public string? CodigoCondicionPago { get; set; }

        public List<ListaGeneral> PaisesFacturacion { get; set; } = [];
        public string? CodigoPaisFacturacion { get; set; }

        public List<ListaGeneral> AnnosCorrelativos { get; set; } = [];
        public int CodigoAnnocorrelativo { get; set; }

        public string? TiempoLlegada { get; set; }

        public string? OrderPrint { get; set; }

        public string? LeadTime { get; set; }

        public Orders Pedido { get; set; } = new Orders();

        public List<ListaGeneral> ModalidadTransporte { get; set; } = [];
        public string? CodigoModTransportte { get; set; }


        public List<ListaGeneral> ShipTo { get; set; }
        public string? CodigoShipTo { get; set; } = string.Empty;
        public List<ListaGeneral> CabecListaPrecios { get; set; }
        public string? CodigoCabecListaPrecios { get; set; } = string.Empty;

        public int? TipoPedido { get; set; }
    }


    public class FechasEstimadas
    {
        public DateTime FechaRequerida { get; set; }
        public DateTime FechaEstimadaDespechoETD { get; set; }
        public DateTime FechaEstimadaLlegadaETA { get; set; }
        public DateTime FechaCorte { get; set; }
        public int LeadTime { get; set; }
    }
}
