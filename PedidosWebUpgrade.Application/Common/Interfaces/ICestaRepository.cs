using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface ICestaRepository
    {
        List<ListaGeneral> ObtenerCondiciones();

        List<ListaGeneral> ObtenerPorcentajes();

        List<Producto> ObtenerProductosCesta(int IdOrden, string CodigoCliente, string IdListaPrecio);

        List<Producto> ObtenerProductosCestaSeleccionados(int IdOrden);

        bool VaciarCesta(int IdOrden);

        bool ActualizarCorrelativo(string NroCorrelativo, int IdOrden);

        bool EliminarProducto(int IdOrden, string CodProd);

        List<ListaGeneral> ObtenerF0005(string drsy, string drrt);

        List<FechasEstimadas> ObtenerFechasEstimadas(int IdOorder);
    }
}
