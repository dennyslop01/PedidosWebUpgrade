using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface ICobranzaRepository
    {
        List<EstadoCuenta> ConsultarEstadoCuenta(string CustomerId);

        List<ListaGeneral> ConsultarTipoPago();

        List<ListaGeneral> ConsultarMonedas();

        List<ListaGeneral> ConsultarBancos();

        List<TipoCambio> ConsultarTipoCambio(string moneda);


    }
}
