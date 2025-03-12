using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IPreferenciaItemDestinoRepository
    {
        List<PreferenciaItemDestino> Listar(int IdPreferencia);

        Dictionary<string, object> Actualizar(PreferenciaItemDestino _Preferencia);

        Dictionary<string, object> Eliminar(int IdPreferencia);
    }
}
