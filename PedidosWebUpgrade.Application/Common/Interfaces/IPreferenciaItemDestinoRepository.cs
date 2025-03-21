using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IPreferenciaItemDestinoRepository
    {
        Task<List<PreferenciaItemDestino>> Listar(int IdPreferencia);

        Task<Dictionary<string, object>> Actualizar(PreferenciaItemDestino _Preferencia);

        Task<Dictionary<string, object>> Eliminar(int IdPreferencia);
    }
}
