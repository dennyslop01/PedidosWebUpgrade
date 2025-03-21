using PedidosWebUpgrade.Application.Common.Interfaces;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.DbContext;
using PedidosWebUpgrade.Infrastructure.Utilities;

namespace PedidosWebUpgrade.Infrastructure.Repository
{
    public class CestaRepository: ICestaRepository
    {
        private readonly ConfigVariables _configVariables;
        public CestaRepository(ConfigVariables configVariables)
        {
            _configVariables = configVariables;
        }
        /// <summary>
        /// LISTADO DE CONDICIONES DE PAGO
        /// </summary>
        /// <returns>List<ListaGeneral></returns>
        public async Task<List<ListaGeneral>> ObtenerCondiciones()
        {
            lDato _ldato = new lDato(_configVariables);
            List<ListaGeneral> _Categorias = [];
            DataResponse<List<ListaGeneral>> List_Response = new DataResponse<List<ListaGeneral>>();
            try
            {

                _ldato.Esquema.Add("Codigo", "ID");
                _ldato.Esquema.Add("Descripcion", "DESCRIPCION");
                List_Response = await _ldato.EjecutarReader(new ListaGeneral(), "PED_USP_CONSULTARCONDICION", _ldato.Parametros, _ldato.Esquema);
                _Categorias = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "CestaRepository", "ObtenerCondiciones()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARCONDICION");
                throw new ApplicationException("ERROR EN: ObtenerCondiciones()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Categorias;
        }

        /// <summary>
        /// LISTADO DE PORCENTAJES DE DESCUENTO
        /// </summary>
        /// <returns>List<ListaGeneral></returns>
        public async Task<List<ListaGeneral>> ObtenerPorcentajes()
        {
            lDato _ldato = new lDato(_configVariables);
            List<ListaGeneral> _Categorias = [];
            DataResponse<List<ListaGeneral>> List_Response = new DataResponse<List<ListaGeneral>>();
            try
            {

                _ldato.Esquema.Add("IdTipo", "ID");
                _ldato.Esquema.Add("Descripcion", "DESCRIPCION");
                List_Response = await _ldato.EjecutarReader(new ListaGeneral(), "PED_USP_CONSULTARPORCENTAJEDCTO", _ldato.Parametros, _ldato.Esquema);
                _Categorias = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "CestaRepository", "ObtenerPorcentajes()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARPORCENTAJEDCTO");
                throw new ApplicationException("ERROR EN: ObtenerPorcentajes()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Categorias;
        }

        /// <summary>
        /// LISTADO DE PRODUCTOS EN CESTA
        /// </summary>
        /// <returns>List<Producto></returns>
        public async Task<List<Producto>> ObtenerProductosCesta(int IdOrden, string CodigoCliente, string IdListaPrecio)
        {
            lDato _ldato = new lDato(_configVariables);
            List<Producto> _Productos = [];
            DataResponse<List<Producto>> List_Response = new DataResponse<List<Producto>>();
            try
            {

                _ldato.Parametros.Add("@IDORDEN", IdOrden);
                _ldato.Parametros.Add("@CUSTOMERID", CodigoCliente);
                _ldato.Parametros.Add("@IDLISTAPRECIOX", string.IsNullOrWhiteSpace(IdListaPrecio) ? Convert.DBNull : IdListaPrecio);
                _ldato.Esquema.Add("Codigo", "CODIGO");
                _ldato.Esquema.Add("Descripcion", "NOMBRE");
                _ldato.Esquema.Add("Stock", "INVENTARIO");
                _ldato.Esquema.Add("PrecioUno", "PRECIO");
                _ldato.Esquema.Add("Brand", "MARCA");
                _ldato.Esquema.Add("Warehouse", "WAREHOUSE");
                _ldato.Esquema.Add("CantidadIngresada", "QTY");
                _ldato.Esquema.Add("Imagen", "NOMBRE_ICONO");
                List_Response = await _ldato.EjecutarReader(new Producto(), "PED_USP_CONSULTARPRODUCTOSCESTA", _ldato.Parametros, _ldato.Esquema);
                _Productos = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "CestaRepository", "ObtenerProductosCesta()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARPRODUCTOSCESTA");
                throw new ApplicationException("ERROR EN: ObtenerProductosCesta()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Productos;
        }


        public async Task<List<Producto>> ObtenerProductosCestaSeleccionados(int IdOrden)
        {
            lDato _ldato = new lDato(_configVariables);
            List<Producto> _Productos = [];
            DataResponse<List<Producto>> List_Response = new DataResponse<List<Producto>>();
            try
            {
                _ldato.Parametros.Add("@IDORDEN", IdOrden);
                _ldato.Esquema.Add("Codigo", "CODIGO");
                _ldato.Esquema.Add("Descripcion", "NOMBRE");
                _ldato.Esquema.Add("Stock", "INVENTARIO");
                _ldato.Esquema.Add("PrecioUno", "PRECIO");
                _ldato.Esquema.Add("Brand", "MARCA");
                _ldato.Esquema.Add("Warehouse", "WAREHOUSE");
                _ldato.Esquema.Add("CantidadIngresada", "QTY");
                _ldato.Esquema.Add("Imagen", "NOMBRE_ICONO");
                List_Response = await _ldato.EjecutarReader(new Producto(), "PED_USP_CONSULTARPRODUCTOSSELECCIONADOS", _ldato.Parametros, _ldato.Esquema);
                _Productos = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "CestaRepository", "ObtenerProductosCestaSeleccionados()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARPRODUCTOSSELECCIONADOS");
                throw new ApplicationException("ERROR EN: ObtenerProductosCestaSeleccionados()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Productos;
        }

        /// <summary>
        /// VACIAR EL PEDIDO DE UN CLIENTE
        /// </summary>
        /// <param name="IdOrden">int</param>
        /// <returns>bool</returns>
        public async Task<bool> VaciarCesta(int IdOrden)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@ORDERID", IdOrden);
                _data = await _ldato.EjecutarScalarReader("PED_USP_VACIARCESTA", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "CestaRepository", "VaciarCesta()", ex.Message.ToString(), null, null, "PED_USP_VACIARCESTA");
            }
            return (bool)_data.Valor;
        }


        public async Task<bool> ActualizarCorrelativo(string NroCorrelativo, int IdOrden)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@ORDERNUMBER", NroCorrelativo);
                _ldato.Parametros.Add("@ORDERID", IdOrden);
                _data = await _ldato.EjecutarScalarReader("PED_USP_ACTUALIZARCORRELATIVO", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "CestaRepository", "ActualizarCorrelativo()", ex.Message.ToString(), null, null, "PED_USP_ACTUALIZARCORRELATIVO");
            }
            return (bool)_data.Valor;
        }


        /// <summary>
        /// ELIMINA PRODUCTO DE LA CESTA
        /// </summary>
        /// <param name="IdOrden">int</param>
        ///<param name="CodProd">string</param>
        /// <returns>bool</returns>
        public async Task<bool> EliminarProducto(int IdOrden, string CodProd)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@ORDERID", IdOrden);
                _ldato.Parametros.Add("@PRODUCTID", CodProd);
                _data = await _ldato.EjecutarScalarReader("PED_USP_ELIMINARPRODUCTOCESTA", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "CestaRepository", "EliminarProducto()", ex.Message.ToString(), null, null, "PED_USP_ELIMINARPRODUCTOCESTA");
            }
            return (bool)_data.Valor;
        }


        public async Task<List<ListaGeneral>> ObtenerF0005(string drsy, string drrt)
        {
            lDato _ldato = new lDato(_configVariables);
            List<ListaGeneral> _ListF0005 = [];
            DataResponse<List<ListaGeneral>> List_Response = new DataResponse<List<ListaGeneral>>();
            try
            {

                _ldato.Parametros.Add("@DRSY", drsy);
                _ldato.Parametros.Add("@DRRT", drrt);
                _ldato.Esquema.Add("Codigo", "DRKY");
                _ldato.Esquema.Add("Descripcion", "DRDL01");
                List_Response = await _ldato.EjecutarReader(new ListaGeneral(), "CON_USP_CONSULTARF0005", _ldato.Parametros, _ldato.Esquema);
                _ListF0005 = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "CestaRepository", "ObtenerF0005()", ex.Message.ToString(), null, null, "CON_USP_CONSULTARF0005");
                throw new ApplicationException("ERROR EN: ObtenerF0005()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _ListF0005;
        }

        public async Task<List<FechasEstimadas>> ObtenerFechasEstimadas(int IdOorder)
        {
            lDato _ldato = new lDato(_configVariables);
            List<FechasEstimadas> _ListFechasEstimadas = [];
            DataResponse<List<FechasEstimadas>> List_Response = new DataResponse<List<FechasEstimadas>>();
            try
            {
                _ldato.Parametros.Add("@ORDERID", IdOorder);
                _ldato.Esquema.Add("FechaRequerida", "ETD");
                _ldato.Esquema.Add("FechaEstimadaDespechoETD", "ETD");
                _ldato.Esquema.Add("FechaEstimadaLlegadaETA", "ETA");
                _ldato.Esquema.Add("FechaCorte", "CUTDATE");
                _ldato.Esquema.Add("LeadTime", "leadtime");
                List_Response = await _ldato.EjecutarReader(new FechasEstimadas(), "PED_USP_CALCULARFECHASPROCESOLOGISTICO", _ldato.Parametros, _ldato.Esquema);
                //List_Response = _ldato.EjecutarReader(new FechasEstimadas(), "PED_USP_CALCULARFECHASESTIMADAS", _ldato.Parametros, _ldato.Esquema);
                _ListFechasEstimadas = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "CestaRepository", "ObtenerFechasEstimadas()", ex.Message.ToString(), null, null, "PED_USP_CALCULARFECHASESTIMADAS");
                throw new ApplicationException("ERROR EN: ObtenerFechasEstimadas()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _ListFechasEstimadas;
        }
    }
}