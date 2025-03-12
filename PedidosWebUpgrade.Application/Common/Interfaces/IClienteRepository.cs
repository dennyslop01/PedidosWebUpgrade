using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IClienteRepository
    {
        List<Customer> ObtenerClientes(string salesmanid, string warehouse, int FactPend, string customerid);

        List<Customer> ObtenerClientesPotencia(string customerid);

        List<Customer> ObtenerShipTo(string salesmanid, string warehouse, int FactPend, string customerid);

        int ActualizarFacturaPendiente(string CustomerId, bool valor);

        int VerificarFacturaPendiente(string CustomerId);

    }
}
