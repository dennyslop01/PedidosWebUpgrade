using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IVendedorRepository
    {
        Task<Dictionary<string, object>> ActualizarVendedor(Vendedor _Vendedor);

        Task<List<Vendedor>> ConsultarVendedores(int IdVendedor);

        Task<int> EliminarVendedor(int IdVendedor);
    }
}
