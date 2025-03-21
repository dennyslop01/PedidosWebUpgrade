using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface ICobranzaRepository
    {
        Task<List<EstadoCuenta>> ConsultarEstadoCuenta(string CustomerId);

        Task<List<ListaGeneral>> ConsultarTipoPago();

        Task<List<ListaGeneral>> ConsultarMonedas();

        Task<List<ListaGeneral>> ConsultarBancos();

        Task<List<TipoCambio>> ConsultarTipoCambio(string moneda);


    }
}
