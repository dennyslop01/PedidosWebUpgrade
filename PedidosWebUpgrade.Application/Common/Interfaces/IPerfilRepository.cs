using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IPerfilRepository
    {
        Task<List<Perfil>> ObtenerPerfil(int IdPerfil);

        Task<int> EliminarPerfil(int IdPerfil);

        Task<Dictionary<string, object>> ActualizarPerfil(Perfil _perfil);

        Task<List<PerfilMenu>> ObtenerPerfilMenu(int IdPerfil, int IdMenu);

        Task<int> EliminarPerfilMenu(int IdPerfilMenu);

        Task<bool> IncluirPerfilMenu(PerfilMenu _PerfilMenu);

        Task<List<PerfilUsuario>> ObtenerPerfilUsuario(int IdPerfil, decimal IdUsuario);

        Task<int> EliminarPerfilUsuario(int IdPerfilUsuario);

        Task<bool> IncluirPerfilUsuario(PerfilUsuario _PerfilUsuario);

        Task<int> ActualizarMiPerfil(Usuario MiPerfil);
    }
}
