using PedidosWebUpgrade.Application.Common.Interfaces;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.DbContext;
using PedidosWebUpgrade.Infrastructure.Utilities;

namespace PedidosWebUpgrade.Infrastructure.Repository
{
    public class ClienteRepository: IClienteRepository
    {
        private readonly ConfigVariables _configVariables;

        public ClienteRepository(ConfigVariables configVariables)
        {
            _configVariables = configVariables;
        }

        /// <summary>
        /// LISTADO DE CLIENTES
        /// </summary>
        /// <returns>List<ListaGeneral></returns>
        public List<Customer> ObtenerClientes(string salesmanid, string warehouse, int FactPend, string customerid = null)
        {
            lDato _ldato = new lDato(_configVariables);
            List<Customer> _Clientes = [];
            DataResponse<List<Customer>> List_Response = new DataResponse<List<Customer>>();
            try
            {
                _ldato.Parametros.Add("@SALESMANID", salesmanid ?? (object)DBNull.Value);
                _ldato.Parametros.Add("@WAREHOUSE", warehouse ?? (object)DBNull.Value);
                _ldato.Parametros.Add("@FACTPENDIENTES", (FactPend == 2 ? (object)DBNull.Value : FactPend)); //0: no, 1:si; 2: todos
                _ldato.Parametros.Add("@CUSTOMERID", customerid ?? (object)DBNull.Value);
                _ldato.Esquema.Add("CustomerId", "CODIGO");
                _ldato.Esquema.Add("Name", "NOMBRE");
                _ldato.Esquema.Add("CreditLimit", "CREDIT_LIMIT");
                _ldato.Esquema.Add("Region", "REGION");
                _ldato.Esquema.Add("SalesmanId", "SALESMAN_ID");
                _ldato.Esquema.Add("Warehouse", "WAREHOUSE");
                _ldato.Esquema.Add("DiasRequerido", "DIASREQUERIDO");
                _ldato.Esquema.Add("WarehouseFacturacion", "WAREHOUSE_FACTURACION");
                _ldato.Esquema.Add("ListaPrecio", "LISTA_PRECIO");
                _ldato.Esquema.Add("FacturasPendientes", "FACTURAS_PENDIENTES");
                _ldato.Esquema.Add("Email", "EMAIL");
                _ldato.Esquema.Add("CPGP", "CPGP");
                _ldato.Esquema.Add("CodCondicionPago", "COD_PAGO");
                _ldato.Esquema.Add("IdListaPrecio", "ID_LISTA_PRECIO");
                List_Response = _ldato.EjecutarReader(new Customer(), "PED_USP_CONSULTARCLIENTE", _ldato.Parametros, _ldato.Esquema);
                _Clientes = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ClienteRepository", "ObtenerClientes()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARCLIENTE");
                throw new ApplicationException("ERROR EN: ObtenerClientes()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Clientes;
        }

        public List<Customer> ObtenerClientesPotencia(string customerid = null)
        {
            lDato _ldato = new lDato(_configVariables);
            List<Customer> _Clientes = [];
            DataResponse<List<Customer>> List_Response = new DataResponse<List<Customer>>();
            try
            {

                _ldato.Parametros.Add("@CUSTOMERID", customerid ?? (object)DBNull.Value);
                _ldato.Esquema.Add("CustomerId", "CODIGO");
                _ldato.Esquema.Add("Name", "NOMBRE");
                _ldato.Esquema.Add("CreditLimit", "CREDIT_LIMIT");
                _ldato.Esquema.Add("Region", "REGION");
                _ldato.Esquema.Add("SalesmanId", "SALESMAN_ID");
                _ldato.Esquema.Add("Warehouse", "WAREHOUSE");
                _ldato.Esquema.Add("DiasRequerido", "DIASREQUERIDO");
                _ldato.Esquema.Add("WarehouseFacturacion", "WAREHOUSE_FACTURACION");
                _ldato.Esquema.Add("ListaPrecio", "LISTA_PRECIO");
                _ldato.Esquema.Add("FacturasPendientes", "FACTURAS_PENDIENTES");
                _ldato.Esquema.Add("Email", "EMAIL");
                _ldato.Esquema.Add("CPGP", "CPGP");
                _ldato.Esquema.Add("CodCondicionPago", "COD_PAGO");
                List_Response = _ldato.EjecutarReader(new Customer(), "PED_USP_CONSULTARCLIENTEPOTENCIA", _ldato.Parametros, _ldato.Esquema);
                _Clientes = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ClienteRepository", "ObtenerClientes()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARCLIENTE");
                throw new ApplicationException("ERROR EN: ObtenerClientes()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Clientes;
        }

        public List<Customer> ObtenerShipTo(string salesmanid, string warehouse, int FactPend, string customerid = null)
        {
            lDato _ldato = new lDato(_configVariables);
            List<Customer> _Clientes = [];
            DataResponse<List<Customer>> List_Response = new DataResponse<List<Customer>>();
            try
            {
                /*
                _ldato.Parametros.Add("@SALESMANID", salesmanid ?? (object)DBNull.Value);
                _ldato.Parametros.Add("@WAREHOUSE", warehouse ?? (object)DBNull.Value);
                _ldato.Parametros.Add("@FACTPENDIENTES", (FactPend == 2 ? (object)DBNull.Value : FactPend)); //0: no, 1:si; 2: todos
                _ldato.Parametros.Add("@CUSTOMERID", customerid ?? (object)DBNull.Value);
                */
                _ldato.Esquema.Add("CustomerId", "CODIGO");
                _ldato.Esquema.Add("Name", "NOMBRE");
                /*_ldato.Esquema.Add("CreditLimit", "CREDIT_LIMIT");
                _ldato.Esquema.Add("Region", "REGION");
                _ldato.Esquema.Add("SalesmanId", "SALESMAN_ID");
                _ldato.Esquema.Add("Warehouse", "WAREHOUSE");
                _ldato.Esquema.Add("DiasRequerido", "DIASREQUERIDO");
                _ldato.Esquema.Add("WarehouseFacturacion", "WAREHOUSE_FACTURACION");
                _ldato.Esquema.Add("ListaPrecio", "LISTA_PRECIO");
                _ldato.Esquema.Add("FacturasPendientes", "FACTURAS_PENDIENTES");
                _ldato.Esquema.Add("Email", "EMAIL"); */
                List_Response = _ldato.EjecutarReader(new Customer(), "PED_USP_CONSULTARSHIPTO", _ldato.Parametros, _ldato.Esquema);
                _Clientes = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ClienteRepository", "ObtenerShipTo()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARSHIPTO");
                throw new ApplicationException("ERROR EN: ObtenerShipTo()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Clientes;
        }



        public int ActualizarFacturaPendiente(string CustomerId, bool valor)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@CUSTOMERID", CustomerId);
                _ldato.Parametros.Add("@VALOR", valor);
                _data = _ldato.EjecutarScalarReader("PED_USP_ACTUALIZARFACTURAPENDIENTE", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ClienteRepository", "ActualizarFacturaPendiente()", ex.Message.ToString(), null, null, "PED_USP_ACTUALIZARFACTURAPENDIENTE");
            }
            return (int)_data.Valor;
        }


        public int VerificarFacturaPendiente(string CustomerId)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@CUSTOMERID", CustomerId);
                _data = _ldato.EjecutarScalarReader("PED_USP_VERIFICARFACTURAPENDIENTE", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ClienteRepository", "ActualizarFacturaPendiente()", ex.Message.ToString(), null, null, "PED_USP_ACTUALIZARFACTURAPENDIENTE");
            }
            return (int)_data.Valor;
        }





    }

}
