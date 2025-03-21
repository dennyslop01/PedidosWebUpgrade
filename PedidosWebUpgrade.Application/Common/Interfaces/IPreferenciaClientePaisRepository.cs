using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IPreferenciaClientePaisRepository
    {
        Task<List<PreferenciaClientePais>> ObtenerPreferenciaClientePais(int IdOrden);

        Task<List<PreferenciaClientePais>> ObtenerPreferenciaClientePais();

        Task<Dictionary<string, object>> ActualizarPreferenciaClientePais(PreferenciaClientePais preferenciaClientePais);

        Task<Dictionary<string, object>> EliminarPreferenciaClientePais(string codCliente, string codPais);
    }
}
