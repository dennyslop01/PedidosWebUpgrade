using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Dictionary<string, object>> ValidarUsuario(string login, string password);

        Task<List<Usuario>> ObtenerUsuario(int IdUsuario);

        Task<List<Salesman>> ObtenerSalesman(string IdSalesman);

        Task<int> EliminarUsuario(int IdUsuario);

        Task<Dictionary<string, object>> ActualizarUsuario(Usuario _usuario);

        Task<List<Menu>> ObtenerPermisos(int IdUsuario, string url);

        Task<List<ListaGeneral>> ObtenerTipoUsuario();
    }
}
