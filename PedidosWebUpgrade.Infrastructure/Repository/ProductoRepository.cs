using PedidosWebUpgrade.Application.Common.Interfaces;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.DbContext;
using PedidosWebUpgrade.Infrastructure.Utilities;

namespace PedidosWebUpgrade.Infrastructure.Repository
{
    public class ProductoRepository: IProductoRepository
    {
        private readonly ConfigVariables _configVariables;

        public ProductoRepository(ConfigVariables configVariables)
        {
            _configVariables = configVariables;
        }
        
        /// <summary>
         /// LISTADO DE CATEGORIAS DE PRODUCTOS
         /// </summary>
         /// <returns>List<ListaGeneral></returns>
        public async Task<List<ListaGeneral>> ObtenerCategorias()
        {
            lDato _ldato = new lDato(_configVariables);
            List<ListaGeneral> _Categorias = new List<ListaGeneral>();
            DataResponse<List<ListaGeneral>> List_Response = new DataResponse<List<ListaGeneral>>();
            try
            {

                _ldato.Esquema.Add("Codigo", "ID");
                _ldato.Esquema.Add("Descripcion", "DESCRIPCION");
                List_Response = await _ldato.EjecutarReader(new ListaGeneral(), "PED_USP_CONSULTARCATEGORIA", _ldato.Parametros, _ldato.Esquema);
                _Categorias = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ProductoRepository", "ObtenerCategorias()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARCATEGORIA");
                throw new ApplicationException("ERROR EN: ObtenerCategorias()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Categorias;
        }

        /// <summary>
        /// LISTADO DE PRODUCTOS
        /// </summary>
        /// <returns>List<ListaGeneral></returns>
        public async Task<List<Producto>> ObtenerProductos(string CodigoProducto, string CodigoSucursal, string CodigoCategoria, string CodigoCliente, string CodListPrecio)
        {
            lDato _ldato = new lDato(_configVariables);
            List<Producto> _Productos = new List<Producto>();
            DataResponse<List<Producto>> List_Response = new DataResponse<List<Producto>>();
            try
            {

                _ldato.Parametros.Add("@CODWAREHOUSE", (string.IsNullOrEmpty(CodigoSucursal) ? (object)DBNull.Value : CodigoSucursal));
                _ldato.Parametros.Add("@CODCATEGORIA", (string.IsNullOrEmpty(CodigoCategoria) ? (object)DBNull.Value : CodigoCategoria));
                _ldato.Parametros.Add("@CODPRODUCTO", (string.IsNullOrEmpty(CodigoProducto) ? (object)DBNull.Value : CodigoProducto));
                _ldato.Parametros.Add("@CODCLIENTE", (string.IsNullOrEmpty(CodigoCliente) ? (object)DBNull.Value : CodigoCliente));
                _ldato.Parametros.Add("@IDLISTAPRECIO", (string.IsNullOrEmpty(CodListPrecio) ? (object)DBNull.Value : CodListPrecio));

                _ldato.Esquema.Add("Codigo", "CODIGO");
                _ldato.Esquema.Add("Descripcion", "NOMBRE");
                _ldato.Esquema.Add("Stock", "INVENTARIO");
                _ldato.Esquema.Add("PrecioUno", "PRECIO");
                _ldato.Esquema.Add("Brand", "MARCA");
                _ldato.Esquema.Add("Warehouse", "WAREHOUSE");
                _ldato.Esquema.Add("Imagen", "NOMBRE_ICONO");
                _ldato.Esquema.Add("CantidadIngresada", "QTY_ORDER");
                _ldato.Esquema.Add("Moneda", "Moneda");
                List_Response = await _ldato.EjecutarReader(new Producto(), "PED_USP_CONSULTARPRODUCTO", _ldato.Parametros, _ldato.Esquema);
                _Productos = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ProductoRepository", "ObtenerProductos()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARPRODUCTO");
                throw new ApplicationException("ERROR EN: ObtenerProductos()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Productos;
        }

        public async Task<List<ListaGeneral>> ObtenerMarcas()
        {
            lDato _ldato = new lDato(_configVariables);
            List<ListaGeneral> _Marcas = new List<ListaGeneral>();
            DataResponse<List<ListaGeneral>> List_Response = new DataResponse<List<ListaGeneral>>();
            try
            {
                _ldato.Esquema.Add("Codigo", "CODIGO");
                _ldato.Esquema.Add("Descripcion", "DESCRIPCION");
                List_Response = await _ldato.EjecutarReader(new ListaGeneral(), "PED_USP_CONSULTARMARCAS", _ldato.Parametros, _ldato.Esquema);
                _Marcas = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ProductoRepository", "ObtenerMarcas()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARMARCAS");
                throw new ApplicationException("ERROR EN: ObtenerMarcas()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Marcas;
        }

        public async Task<int> ActualizarIconoMarca(string icono, string marca)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@BRAND", marca);
                _ldato.Parametros.Add("@ICONO", icono);
                _data = await _ldato.EjecutarScalarReader("PED_USP_ACTUALIZARICONOMARCA", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ProductoRepository", "ActualizarIconoMarca()", ex.Message.ToString(), null, null, "PED_USP_ACTUALIZARICONOMARCA");
            }
            return (int)_data.Valor;
        }

        public async Task<List<Product>> ObtenerTodoslosProductos(string IdProducto)
        {
            lDato _ldato = new lDato(_configVariables);
            List<Product> _Productos = new List<Product>();
            DataResponse<List<Product>> List_Response = new DataResponse<List<Product>>();
            try
            {
                _ldato.Parametros.Add("@IDPRODUCTO", string.IsNullOrWhiteSpace(IdProducto) ? Convert.DBNull : IdProducto);

                _ldato.Esquema.Add("Id", "product_id");
                _ldato.Esquema.Add("Descripcion", "description");
                _ldato.Esquema.Add("Stock", "stock");
                _ldato.Esquema.Add("Precio1", "price1");
                _ldato.Esquema.Add("Precio2", "price2");
                _ldato.Esquema.Add("Precio3", "price3");
                _ldato.Esquema.Add("Precio4", "price4");
                _ldato.Esquema.Add("Precio5", "price5");
                _ldato.Esquema.Add("Precio6", "price6");
                _ldato.Esquema.Add("Precio7", "price7");
                _ldato.Esquema.Add("Precio8", "price8");
                _ldato.Esquema.Add("Precio9", "price9");
                _ldato.Esquema.Add("Precio10", "price10");
                _ldato.Esquema.Add("UnidadPrimaria", "primary_unit");
                _ldato.Esquema.Add("UnidadAlternativa", "alternative_unit");
                _ldato.Esquema.Add("TasadeConversion", "conversion_rate");
                _ldato.Esquema.Add("Warehouse", "warehouse");
                _ldato.Esquema.Add("Brand", "brand");
                _ldato.Esquema.Add("Target", "target");
                _ldato.Esquema.Add("Estado", "estado");
                _ldato.Esquema.Add("Imshcm", "IMSHCM");
                _ldato.Esquema.Add("Imitm", "IMITM");
                _ldato.Esquema.Add("Imsrp9", "IMSRP9");

                List_Response = await _ldato.EjecutarReader(new Product(), "PED_USP_CONSULTARPRODUCTOSGENERAL", _ldato.Parametros, _ldato.Esquema);
                _Productos = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ProductoRepository", "ObtenerTodoslosProductos()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARPRODUCTOSGENERAL");
                throw new ApplicationException("ERROR EN: ObtenerTodoslosProductos()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Productos;
        }
    }
}
