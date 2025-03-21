using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IClienteRepository
    {
        Task<List<Customer>> ObtenerClientes(string salesmanid, string warehouse, int FactPend, string customerid);

        Task<List<Customer>> ObtenerClientesPotencia(string customerid);

        Task<List<Customer>> ObtenerShipTo(string salesmanid, string warehouse, int FactPend, string customerid);

        Task<int> ActualizarFacturaPendiente(string CustomerId, bool valor);

        Task<int> VerificarFacturaPendiente(string CustomerId);

    }
}
