using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IMenuRepository
    {
        List<Menu> ObtenerMenu(int IdMenu);

        int EliminarMenu(int IdMenu);

        Dictionary<string, object> ActualizarMenu(Menu _Menu);

        List<Menu> ObtenerMenuUsuario(int IdUsuario);
    }
}
