using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IProductoRepository
    {
        List<ListaGeneral> ObtenerCategorias();

        List<Producto> ObtenerProductos(string CodigoProducto, string CodigoSucursal, string CodigoCategoria, string CodigoCliente, string CodListPrecio);

        List<ListaGeneral> ObtenerMarcas();

        int ActualizarIconoMarca(string icono, string marca);

        List<Product> ObtenerTodoslosProductos(string IdProducto);


    }
}
