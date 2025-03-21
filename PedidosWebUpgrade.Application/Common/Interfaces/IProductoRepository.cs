using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IProductoRepository
    {
        Task<List<ListaGeneral>> ObtenerCategorias();

        Task<List<Producto>> ObtenerProductos(string CodigoProducto, string CodigoSucursal, string CodigoCategoria, string CodigoCliente, string CodListPrecio);

        Task<List<ListaGeneral>> ObtenerMarcas();

        Task<int> ActualizarIconoMarca(string icono, string marca);

        Task<List<Product>> ObtenerTodoslosProductos(string IdProducto);
    }
}
