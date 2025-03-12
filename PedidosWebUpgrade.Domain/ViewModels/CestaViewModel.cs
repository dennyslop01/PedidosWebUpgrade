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
            this.ShipTo = new List<ListaGeneral>();
            this.CabecListaPrecios = new List<ListaGeneral>();
            this.CondicionesDePago = new List<ListaGeneral>();
            this.PaisesFacturacion = new List<ListaGeneral>();
            this.AnnosCorrelativos = new List<ListaGeneral>();
        }
        public EncabezadoPedido Encabezado { get; set; } = new EncabezadoPedido();
        public string? NombreCliente { get; set; }
        public List<ListaGeneral> Condicion { get; set; } = new List<ListaGeneral>();
        public List<ListaGeneral> Descuento { get; set; } = new List<ListaGeneral>();
        public string? FechaPedido { get; set; }
        public string? FechaCorte { get; set; }
        public string? FechaRequerida { get; set; }
        public string? FechaEstimadaDespachoETD { get; set; }
        public string? FechaEstimadaLlegadaETA { get; set; }
        public string? Observaciones { get; set; }
        public List<Producto> Productos { get; set; } = new List<Producto>();
        public string? CodigoCondicion { get; set; }
        public int CodigoDescuento { get; set; }
        public string? CodigoCliente { get; set; }
        public bool Prepagado { get; set; }

        public string? TasaNegociacion { get; set; }

        public List<ListaGeneral> Carriers { get; set; } = new List<ListaGeneral>();
        public string? CodigoCarrier { get; set; }

        public List<ListaGeneral> PuertosDescargas { get; set; } = new List<ListaGeneral>();
        public string? CodigoPuerto { get; set; }

        public List<ListaGeneral> IncosTerms { get; set; } = new List<ListaGeneral>();
        public string? CodigoIncoTerms { get; set; }


        public List<ListaGeneral> Referencias01 { get; set; } = new List<ListaGeneral>();
        public string? CodigoReferencia01 { get; set; }

        public List<ListaGeneral> ForwardingAgents { get; set; } = new List<ListaGeneral>();
        public string? CodigoForwardingAgent { get; set; }

        public List<ListaGeneral> Almacenes { get; set; } = new List<ListaGeneral>();
        public string? Codigoalmacent { get; set; }

        public List<ListaGeneral> CondicionesDePago { get; set; } = new List<ListaGeneral>();
        public string? CodigoCondicionPago { get; set; }

        public List<ListaGeneral> PaisesFacturacion { get; set; } = new List<ListaGeneral>();
        public string? CodigoPaisFacturacion { get; set; }

        public List<ListaGeneral> AnnosCorrelativos { get; set; } = new List<ListaGeneral>();
        public int CodigoAnnocorrelativo { get; set; }

        public string? TiempoLlegada { get; set; }

        public string? OrderPrint { get; set; }

        public string? LeadTime { get; set; }

        public Orders Pedido { get; set; } = new Orders();

        public List<ListaGeneral> ModalidadTransporte { get; set; } = new List<ListaGeneral>();
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
