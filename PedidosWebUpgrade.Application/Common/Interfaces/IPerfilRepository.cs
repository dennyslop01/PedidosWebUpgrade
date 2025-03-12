using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IPerfilRepository
    {
        List<Perfil> ObtenerPerfil(int IdPerfil);

        int EliminarPerfil(int IdPerfil);

        Dictionary<string, object> ActualizarPerfil(Perfil _perfil);

        List<PerfilMenu> ObtenerPerfilMenu(int IdPerfil, int IdMenu);

        int EliminarPerfilMenu(int IdPerfilMenu);

        bool IncluirPerfilMenu(PerfilMenu _PerfilMenu);

        List<PerfilUsuario> ObtenerPerfilUsuario(int IdPerfil, decimal IdUsuario);

        int EliminarPerfilUsuario(int IdPerfilUsuario);

        bool IncluirPerfilUsuario(PerfilUsuario _PerfilUsuario);

        int ActualizarMiPerfil(Usuario MiPerfil);
    }
}
