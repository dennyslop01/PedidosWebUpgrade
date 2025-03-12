using PedidosWebUpgrade.Application.Common.Interfaces;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.DbContext;
using PedidosWebUpgrade.Infrastructure.Utilities;
using System.Data;
using System.Data.SqlClient;

namespace PedidosWebUpgrade.Infrastructure.Repository
{
    public class PreferenciaClientePaisRepository: IPreferenciaClientePaisRepository
    {
        private readonly ConfigVariables _configVariables;

        public PreferenciaClientePaisRepository(ConfigVariables configVariables)
        {
            _configVariables = configVariables;
        }

        public List<PreferenciaClientePais> ObtenerPreferenciaClientePais(int IdOrden)
        {
            lDato _ldato = new lDato(_configVariables);
            List<PreferenciaClientePais> _PreferenciaClientePais = new List<PreferenciaClientePais>();
            DataResponse<List<PreferenciaClientePais>> List_Response = new DataResponse<List<PreferenciaClientePais>>();
            try
            {
                _ldato.Parametros.Add("@ORDERID", IdOrden);
                _ldato.Esquema.Add("CodPais", "pais");
                _ldato.Esquema.Add("Pais", "Paisnombre");
                _ldato.Esquema.Add("CodCliente", "cliente");
                _ldato.Esquema.Add("Cliente", "Clinombre");

                List_Response = _ldato.EjecutarReader(new PreferenciaClientePais(), "PED_USP_CONSULTARPREFERENCIACLIENTEPAIS", _ldato.Parametros, _ldato.Esquema);
                _PreferenciaClientePais = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PreferenciaClientePaisRepository", "ObtenerPreferenciaClientePais()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARPREFERENCIACLIENTEPAIS");
                throw new ApplicationException("ERROR EN: ObtenerPreferenciaClientePais()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _PreferenciaClientePais;
        }

        public List<PreferenciaClientePais> ObtenerPreferenciaClientePais()
        {
            lDato _ldato = new lDato(_configVariables);
            List<PreferenciaClientePais> _PreferenciaClientePais = new List<PreferenciaClientePais>();
            DataResponse<List<PreferenciaClientePais>> List_Response = new DataResponse<List<PreferenciaClientePais>>();
            try
            {
                _ldato.Esquema.Add("CodPais", "pais");
                _ldato.Esquema.Add("Pais", "Paisnombre");
                _ldato.Esquema.Add("CodCliente", "cliente");
                _ldato.Esquema.Add("Cliente", "Clinombre");

                List_Response = _ldato.EjecutarReader(new PreferenciaClientePais(), "PED_USP_CONSULTARPREFERENCIACLIENTEPAIS", _ldato.Parametros, _ldato.Esquema);
                _PreferenciaClientePais = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PreferenciaClientePaisRepository", "ObtenerPreferenciaClientePais()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARPREFERENCIACLIENTEPAIS");
                throw new ApplicationException("ERROR EN: ObtenerPreferenciaClientePais()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _PreferenciaClientePais;
        }

        public Dictionary<string, object> ActualizarPreferenciaClientePais(PreferenciaClientePais preferenciaClientePais)
        {
            lDato _ldato = new lDato(_configVariables);
            try
            {
                List<SqlParameter> _parametros = new List<SqlParameter> { new SqlParameter() { ParameterName = "@IDPAIS", Value = preferenciaClientePais.CodPais, SqlDbType = SqlDbType.VarChar, Size = 8},
                                                                          new SqlParameter() {ParameterName = "@IDCOSTOMER",  Value =  preferenciaClientePais.CodCliente ?? Convert.DBNull, SqlDbType = SqlDbType.VarChar, Size = 8},
                                                                          new SqlParameter() {ParameterName = "@RESULTADO",  Direction = ParameterDirection.Output, SqlDbType = SqlDbType.SmallInt},
                                                                          new SqlParameter() {ParameterName = "@MSJ", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.VarChar, Size = -1},
                                                                         };
                _ldato.ParametrosSql = _parametros;
                var resultado = _ldato.EjecutarNonQueryOutput("PED_USP_ACTUALIZARPREFERENCIACLIENTEPAIS", _ldato.ParametrosSql);
                return resultado.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PreferenciaClientePaisRepository", "ActualizarPreferenciaClientePais()", ex.ToString(), null, null, "PED_USP_ACTUALIZARPREFERENCIACLIENTEPAIS");
                throw new ApplicationException("ERROR EN: ActualizarPreferenciaClientePais", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            ;
        }

        public Dictionary<string, object> EliminarPreferenciaClientePais(string codCliente, string codPais)
        {
            lDato _ldato = new lDato(_configVariables);
            try
            {
                List<SqlParameter> _parametros = new List<SqlParameter> {
                                                                          new SqlParameter() {ParameterName = "@IDPAIS",  Value =  codPais, SqlDbType = SqlDbType.VarChar, Size = 8},
                                                                          new SqlParameter() {ParameterName = "@IDCLIENTE",  Value =  codCliente, SqlDbType = SqlDbType.VarChar, Size = 8},
                                                                          new SqlParameter() {ParameterName = "@RESULTADO",  Direction = ParameterDirection.Output, SqlDbType = SqlDbType.SmallInt},
                                                                          new SqlParameter() {ParameterName = "@MSJ", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.VarChar, Size = -1},
                                                                         };
                _ldato.ParametrosSql = _parametros;
                var resultado = _ldato.EjecutarNonQueryOutput("PED_USP_ELIMINARPREFERENCIACLIENTEPAIS", _ldato.ParametrosSql);
                return resultado.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PreferenciaClientePaisRepository", "EliminarPreferenciaClientePais()", ex.ToString(), null, null, "PED_USP_ELIMINARPREFERENCIACLIENTEPAIS");
                throw new ApplicationException("ERROR EN: EliminarPreferenciaClientePais", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            ;
        }
    }
}
