using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IListaPreciosRepository
    {
        List<F45520> ConsultarCabeceraListoPrecio();

        List<F45521> ConsultarDetalleListaPrecio(string phdoco, string phdcto);

        List<ListPreciosPrint> ConsultarDetallesReporteListaPrecios(string doco, string dcto, int IdUsuario);
    }
}
