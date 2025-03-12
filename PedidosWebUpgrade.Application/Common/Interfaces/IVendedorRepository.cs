using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IVendedorRepository
    {
        Dictionary<string, object> ActualizarVendedor(Vendedor _Vendedor);

        List<Vendedor> ConsultarVendedores(int IdVendedor);

        int EliminarVendedor(int IdVendedor);
    }
}
