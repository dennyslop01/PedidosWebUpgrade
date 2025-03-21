using Microsoft.IdentityModel.Tokens;
using PedidosWebUpgrade.Application.Common.Interfaces;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.DbContext;
using PedidosWebUpgrade.Infrastructure.Utilities;
using System.Data;
using System.Data.SqlClient;

namespace PedidosWebUpgrade.Infrastructure.Repository
{
    public class VendedorRepository: IVendedorRepository
    {
        private readonly ConfigVariables _configVariables;

        public VendedorRepository(ConfigVariables configVariables)
        {
            _configVariables = configVariables;
        }
        
        public async Task<Dictionary<string, object>> ActualizarVendedor(Vendedor _Vendedor)
        {
            lDato _ldato = new lDato(_configVariables);
            try
            {
                List<SqlParameter> _parametros = new List<SqlParameter> { new SqlParameter() {ParameterName = "@INVENDOR",  Value =  _Vendedor.IdVendedor, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@NOMBRE",  Value =  _Vendedor.Nombre ?? Convert.DBNull, SqlDbType = SqlDbType.VarChar, Size = 1000},
                                                                          new SqlParameter() {ParameterName = "@LINEA1",  Value =  _Vendedor.Linea1 ?? Convert.DBNull, SqlDbType = SqlDbType.VarChar, Size = 1000},
                                                                          new SqlParameter() {ParameterName = "@LINEA2",  Value =  _Vendedor.Linea2 ?? Convert.DBNull, SqlDbType = SqlDbType.VarChar, Size = 1000},
                                                                          new SqlParameter() {ParameterName = "@LINEA3",  Value =  _Vendedor.Linea3 ?? Convert.DBNull, SqlDbType = SqlDbType.VarChar, Size = 1000},
                                                                          new SqlParameter() {ParameterName = "@LINEA4",  Value =  _Vendedor.Linea4 ?? Convert.DBNull, SqlDbType = SqlDbType.VarChar, Size = 1000},
                                                                          new SqlParameter() {ParameterName = "@LINEA5",  Value =  _Vendedor.Linea5 ?? Convert.DBNull, SqlDbType = SqlDbType.VarChar, Size = 1000},
                                                                          new SqlParameter() {ParameterName = "@LOGOORDENPRODUCCION",  Value =  _Vendedor.LogoProforma ?? Convert.DBNull, SqlDbType = SqlDbType.VarChar, Size = 50},
                                                                          new SqlParameter() {ParameterName = "@LOGOPROFORMA",  Value =  _Vendedor.LogoProforma, SqlDbType = SqlDbType.VarChar, Size = 50},
                                                                          new SqlParameter() {ParameterName = "@IDVENDEDOROUT",  Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@RESULTADO", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.SmallInt},
                                                                         };
                _ldato.ParametrosSql = _parametros;
                var resultado = await _ldato.EjecutarNonQueryOutput("PED_USP_CREARVENDEDOR", _ldato.ParametrosSql);
                return resultado.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"VendedorRepository", "ActualizarVendedor()", ex.ToString(), null, null, "PED_USP_CREARVENDEDOR");
                throw new ApplicationException("ERROR EN: ActualizarVendedor", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            ;
        }

        public async Task<List<Vendedor>> ConsultarVendedores(int IdVendedor)
        {
            lDato _ldato = new lDato(_configVariables);
            List<Vendedor> _Vendedor = new List<Vendedor>();
            DataResponse<List<Vendedor>> List_Response = new DataResponse<List<Vendedor>>();
            try
            {
                _ldato.Parametros.Add("@IDvendedor", (IdVendedor == 0 ? (object)DBNull.Value : IdVendedor));
                _ldato.Esquema.Add("IdVendedor", "id_vendor");
                _ldato.Esquema.Add("Nombre", "nombre");
                _ldato.Esquema.Add("Linea1", "linea1");
                _ldato.Esquema.Add("Linea2", "linea2");
                _ldato.Esquema.Add("Linea3", "linea3");
                _ldato.Esquema.Add("Linea4", "linea4");
                _ldato.Esquema.Add("Linea5", "linea5");
                _ldato.Esquema.Add("LogoOrdenProduccion", "logo_orden_produccion");
                _ldato.Esquema.Add("LogoProforma", "logo_proforma");

                List_Response = await _ldato.EjecutarReader(new Vendedor(), "PED_USP_CONSULTARVENDEDORES", _ldato.Parametros, _ldato.Esquema);
                _Vendedor = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"VendedorRepository", "ConsultarVendedores()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARVENDEDORES");
                throw new ApplicationException("ERROR EN: ConsultarVendedores()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Vendedor;
        }

        public async Task<int> EliminarVendedor(int IdVendedor)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@IDVENDEDOR", IdVendedor);
                _data = await _ldato.EjecutarScalarReader("PED_USP_ELIMINARVENDEDOR", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"VendedorRepository", "EliminarVendedor()", ex.Message.ToString(), null, null, "PED_USP_ELIMINARVENDEDOR");
            }
            return int.Parse(_data.Valor.ToString());
        }
    }
}
