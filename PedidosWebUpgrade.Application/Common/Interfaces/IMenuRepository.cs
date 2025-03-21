using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IMenuRepository
    {
        Task<List<Menu>> ObtenerMenu(int IdMenu);

        Task<int> EliminarMenu(int IdMenu);

        Task<Dictionary<string, object>> ActualizarMenu(Menu _Menu);

        Task<List<Menu>> ObtenerMenuUsuario(int IdUsuario);
    }
}
