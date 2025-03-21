using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IListaPreciosRepository
    {
        Task<List<F45520>> ConsultarCabeceraListoPrecio();

        Task<List<F45521>> ConsultarDetalleListaPrecio(string phdoco, string phdcto);

        Task<List<ListPreciosPrint>> ConsultarDetallesReporteListaPrecios(string doco, string dcto, int IdUsuario);
    }
}
