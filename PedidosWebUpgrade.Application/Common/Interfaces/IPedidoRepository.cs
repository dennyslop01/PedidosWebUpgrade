using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Domain.ViewModels;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IPedidoRepository
    {
        Task<Dictionary<string, object>> AgregarProducto(Producto Producto, string CodCliente, int IdOrden);

        Task<EncabezadoPedido> ObtenerEncabezadoOrden(int IdOrden, string IdCliente, string salesmanid);

        Task<bool> ProcesarOrden(int IdOrden, string condicion, int porcentaje, string fechapedido, string fecharequerida, string observacion, string montototal,
                                   string montodscto, string prepagado, string tasanegociacion, string idcarrier, string idpuertodescarga, string idincoterms,
                                   string referencia01, string tiempollegada, string ordenprint, int usuario, string fechacorte, string leadtime, string codModalidad,
                                   string FechaDespachoETD, string FechaLlegadaETA, int ForwardingAgent, string idlistaprecio, string idshipto, string idalmacen, string CodCondPago, string codPaisFacturacion, int codAnnoCorrelativo, int TipoPedido);


        Task<bool> AprobarOrden(int IdOrden);

        Task<List<Orders>> ConsultarPedidos(string CustomerId, string FechaDesde, string FechaHasta, int OrderId, string Estatus, int IdUsuario);

        Task<List<ListaGeneral>> ObtenerEstatus();

        Task<List<EstatusOrden>> ObtenerEstatusOrden();

        Task<List<SeguimientoOrden>> ObtenerSeguimiento(int IdOrden);

        Task<List<Descuento>> ObtenerCalculoDescuentoComercial(int IdOrden, float Porcentaje);

        Task<List<PedidoPDFViewModel>> ConsultarOrdenPdf(int IdOrden, int Tipo);

        Task<bool> EliminarPedido(int IdOrden, string IdMotivo, int IdUsuario);

        Task<Dictionary<string, object>> ReactivarPedido(int IdOrden, string IdMotivo, int IdUsuario);

        Task<Dictionary<string, object>> EnviarProforma(int IdOrden);

        Task<Dictionary<string, object>> AsignarPoNumber(int IdOrden);

        Task<List<HistoriaOrder>> ConsultarHistoricoPedidos(int IdOrden);

        Task<List<ListaGeneral>> ConsultarCondicionesDePago();

        Task<List<ListaGeneral>> ConsultarForwardingAgent();

        Task<List<ListaGeneral>> ConsultarPaisesFacturacion(string IdCli);

        Task<List<ListaGeneral>> ConsultarContadoresAnnoPais(string codigoPais);

        Task<bool> RecalcularPrecioOrden(int IdOrden, int IdUsuario);
    }
}
