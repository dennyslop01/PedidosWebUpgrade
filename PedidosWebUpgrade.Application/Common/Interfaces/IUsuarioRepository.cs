using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IUsuarioRepository
    {
        Dictionary<string, object> ValidarUsuario(string login, string password);

        List<Usuario> ObtenerUsuario(int IdUsuario);

        List<Salesman> ObtenerSalesman(string IdSalesman);

        int EliminarUsuario(int IdUsuario);

        Dictionary<string, object> ActualizarUsuario(Usuario _usuario);

        List<Menu> ObtenerPermisos(int IdUsuario, string url);

        List<ListaGeneral> ObtenerTipoUsuario();
    }
}
