using PedidosWebUpgrade.Application.Common.Interfaces;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.DbContext;
using PedidosWebUpgrade.Infrastructure.Utilities;
using System.Data;
using System.Data.SqlClient;

namespace PedidosWebUpgrade.Infrastructure.Repository
{
    public class PreferenciaItemDestinoRepository: IPreferenciaItemDestinoRepository
    {
        private readonly ConfigVariables _configVariables;

        public PreferenciaItemDestinoRepository(ConfigVariables configVariables)
        {
            _configVariables = configVariables;
        }
        
        public async Task<List<PreferenciaItemDestino>> Listar(int IdPreferencia)
        {
            lDato _ldato = new lDato(_configVariables);
            List<PreferenciaItemDestino> _preferencia = new List<PreferenciaItemDestino>();
            DataResponse<List<PreferenciaItemDestino>> List_Response = new DataResponse<List<PreferenciaItemDestino>>();
            try
            {
                _ldato.Parametros.Add("@IDPREFERENCIA", (IdPreferencia == 0 ? Convert.DBNull : IdPreferencia));

                _ldato.Esquema.Add("Id", "id_preferencia_item_destino");
                _ldato.Esquema.Add("DestinoId", "id_destino");
                _ldato.Esquema.Add("Pais", "pais");
                _ldato.Esquema.Add("ProductId", "product_id");
                _ldato.Esquema.Add("Producto", "producto");

                List_Response = await _ldato.EjecutarReader(new PreferenciaItemDestino(), "CON_USP_CONSULTARPREFERENCIAITEMDESTINO", _ldato.Parametros, _ldato.Esquema);
                _preferencia = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PreferenciaItemDestinoRepository", "Listar()", ex.Message.ToString(), null, null, "CON_USP_CONSULTARPREFERENCIAITEMDESTINO");
                throw new ApplicationException("ERROR EN: Listar()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _preferencia;
        }

        public async Task<Dictionary<string, object>> Actualizar(PreferenciaItemDestino _Preferencia)
        {
            lDato _ldato = new lDato(_configVariables);
            try
            {
                List<SqlParameter> _parametros = new List<SqlParameter> { new SqlParameter() {ParameterName = "@IDPREFERENCIA",  Value =  _Preferencia.Id, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@IDPRODUCTO",  Value =  _Preferencia.ProductId ?? Convert.DBNull, SqlDbType = SqlDbType.VarChar, Size = 15},
                                                                          new SqlParameter() {ParameterName = "@IDDESTINO",  Value =  _Preferencia.DestinoId ?? Convert.DBNull, SqlDbType = SqlDbType.VarChar, Size = 6},
                                                                          new SqlParameter() {ParameterName = "@MSJ",  Direction = ParameterDirection.Output, SqlDbType = SqlDbType.VarChar, Size = -1},
                                                                          new SqlParameter() {ParameterName = "@RESULTADO", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.SmallInt},
                                                                         };
                _ldato.ParametrosSql = _parametros;
                var resultado = await _ldato.EjecutarNonQueryOutput("CON_USP_ACTUALIZARPREFERENCIAITEMDESTINO", _ldato.ParametrosSql);
                return resultado.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PreferenciaItemDestinoRepository", "Actualizar()", ex.ToString(), null, null, "CON_USP_ACTUALIZARPREFERENCIAITEMDESTINO");
                throw new ApplicationException("ERROR EN: Actualizar", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            ;
        }

        public async Task<Dictionary<string, object>> Eliminar(int IdPreferencia)
        {
            lDato _ldato = new lDato(_configVariables);
            try
            {
                List<SqlParameter> _parametros = new List<SqlParameter> {
                                                                          new SqlParameter() {ParameterName = "@IDPREFERENCIA",  Value =  IdPreferencia, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@RESULTADO",  Direction = ParameterDirection.Output, SqlDbType = SqlDbType.SmallInt},
                                                                          new SqlParameter() {ParameterName = "@MSJ", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.VarChar, Size = -1},
                                                                         };
                _ldato.ParametrosSql = _parametros;
                var resultado = await _ldato.EjecutarNonQueryOutput("CON_USP_ELIMINARPREFERENCIAITEMDESTINO", _ldato.ParametrosSql);
                return resultado.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ImportRepository", "Eliminar()", ex.ToString(), null, null, "CON_USP_ELIMINARPREFERENCIAITEMDESTINO");
                throw new ApplicationException("ERROR EN: Eliminar", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            ;
        }
    }
}
