using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IPreferenciaClientePaisRepository
    {
        List<PreferenciaClientePais> ObtenerPreferenciaClientePais(int IdOrden);

        List<PreferenciaClientePais> ObtenerPreferenciaClientePais();

        Dictionary<string, object> ActualizarPreferenciaClientePais(PreferenciaClientePais preferenciaClientePais);

        Dictionary<string, object> EliminarPreferenciaClientePais(string codCliente, string codPais);
    }
}
