using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Domain.ViewModels;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IPedidoRepository
    {
        Dictionary<string, object> AgregarProducto(Producto Producto, string CodCliente, int IdOrden);

        EncabezadoPedido ObtenerEncabezadoOrden(int IdOrden, string IdCliente, string salesmanid);

        bool ProcesarOrden(int IdOrden, string condicion, int porcentaje, string fechapedido, string fecharequerida, string observacion, string montototal,
                                   string montodscto, string prepagado, string tasanegociacion, string idcarrier, string idpuertodescarga, string idincoterms,
                                   string referencia01, string tiempollegada, string ordenprint, int usuario, string fechacorte, string leadtime, string codModalidad,
                                   string FechaDespachoETD, string FechaLlegadaETA, int ForwardingAgent, string idlistaprecio, string idshipto, string idalmacen, string CodCondPago, string codPaisFacturacion, int codAnnoCorrelativo, int TipoPedido);


        bool AprobarOrden(int IdOrden);

        List<Orders> ConsultarPedidos(string CustomerId, string FechaDesde, string FechaHasta, int OrderId, string Estatus, int IdUsuario);

        List<ListaGeneral> ObtenerEstatus();

        List<EstatusOrden> ObtenerEstatusOrden();

        List<SeguimientoOrden> ObtenerSeguimiento(int IdOrden);

        List<Descuento> ObtenerCalculoDescuentoComercial(int IdOrden, float Porcentaje);

        List<PedidoPDFViewModel> ConsultarOrdenPdf(int IdOrden, int Tipo);

        bool EliminarPedido(int IdOrden, string IdMotivo, int IdUsuario);

        Dictionary<string, object> ReactivarPedido(int IdOrden, string IdMotivo, int IdUsuario);

        Dictionary<string, object> EnviarProforma(int IdOrden);

        Dictionary<string, object> AsignarPoNumber(int IdOrden);

        List<HistoriaOrder> ConsultarHistoricoPedidos(int IdOrden);

        List<ListaGeneral> ConsultarCondicionesDePago();

        List<ListaGeneral> ConsultarForwardingAgent();

        List<ListaGeneral> ConsultarPaisesFacturacion(string IdCli);

        List<ListaGeneral> ConsultarContadoresAnnoPais(string codigoPais);

        bool RecalcularPrecioOrden(int IdOrden, int IdUsuario);
    }
}
