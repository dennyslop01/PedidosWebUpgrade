using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface ICestaRepository
    {
        Task<List<ListaGeneral>> ObtenerCondiciones();

        Task<List<ListaGeneral>> ObtenerPorcentajes();

        Task<List<Producto>> ObtenerProductosCesta(int IdOrden, string CodigoCliente, string IdListaPrecio);

        Task<List<Producto>> ObtenerProductosCestaSeleccionados(int IdOrden);

        Task<bool> VaciarCesta(int IdOrden);

        Task<bool> ActualizarCorrelativo(string NroCorrelativo, int IdOrden);

        Task<bool> EliminarProducto(int IdOrden, string CodProd);

        Task<List<ListaGeneral>> ObtenerF0005(string drsy, string drrt);

        Task<List<FechasEstimadas>> ObtenerFechasEstimadas(int IdOorder);
    }
}
