using PedidosWebUpgrade.Application.Common.Interfaces;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.DbContext;
using PedidosWebUpgrade.Infrastructure.Utilities;
using System.Data;
using System.Data.SqlClient;

namespace PedidosWebUpgrade.Infrastructure.Repository
{
    public class AlmacenRepository : IAlmacenRepository
    {
        private readonly ConfigVariables _configVariables;

        public AlmacenRepository(ConfigVariables configVariables)
        {
            _configVariables = configVariables;
        }

        public async Task<Dictionary<string, object>> ActualizarAlmacen(Almacen _Almacen)
        {
            lDato _ldato = new lDato(_configVariables);
            try
            {
                List<SqlParameter> _parametros = [
                                                                          new SqlParameter() {ParameterName = "@ID",  Value =  _Almacen.Id, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@IDALMACEN",  Value =  _Almacen.IdAlmacen, SqlDbType = SqlDbType.VarChar, Size = 50},
                                                                          new SqlParameter() {ParameterName = "@DESCRIPCION",  Value =  _Almacen.Descripcion ?? Convert.DBNull, SqlDbType = SqlDbType.VarChar, Size = 500},
                                                                          new SqlParameter() {ParameterName = "@MODO",  Value =  _Almacen.Modo ?? Convert.DBNull, SqlDbType = SqlDbType.VarChar, Size = 50},
                                                                          new SqlParameter() {ParameterName = "@IDVENDEDORORDEN",  Value =  _Almacen.IdVendedorOrden, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@IDVENDEDORPROFORMA",  Value =  _Almacen.IdVendedorProforma, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@RESULTADO", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.SmallInt},
                                                                         ];
                _ldato.ParametrosSql = _parametros;
                var resultado = await _ldato.EjecutarNonQueryOutput("PED_USP_CREARALMACEN", _ldato.ParametrosSql);
                return resultado.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "AlmacenRepository", "ActualizarAlmacen()", ex.ToString(), null, null, "PED_USP_CREARALMACEN");
                throw new ApplicationException("ERROR EN: ActualizarAlmacen", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            ;
        }
        public async Task<List<Almacen>> ConsultarAlmacenes(int Id)
        {
            lDato _ldato = new lDato(_configVariables);
            List<Almacen> _Almacen = [];
            DataResponse<List<Almacen>> List_Response = new DataResponse<List<Almacen>>();
            try
            {
                _ldato.Parametros.Add("@ID", (Id == 0 ? (object)DBNull.Value : Id));
                _ldato.Esquema.Add("Id", "id");
                _ldato.Esquema.Add("IdAlmacen", "id_almacen");
                _ldato.Esquema.Add("Descripcion", "descripcion");
                _ldato.Esquema.Add("Modo", "modo");
                _ldato.Esquema.Add("IdVendedorOrden", "id_vendor_orden");
                _ldato.Esquema.Add("IdVendedorProforma", "id_vendor_proforma");
                _ldato.Esquema.Add("NombreVendedorProforma", "NOMBREVENDEDORPROFORMA");
                _ldato.Esquema.Add("NombreVendedorOrderProdupcion", "NOMBREVENDEDORORDEN");

                List_Response = await _ldato.EjecutarReader(new Almacen(), "PED_USP_CONSULTARARALMACENES", _ldato.Parametros, _ldato.Esquema);
                _Almacen = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "AlmacenRepository", "ConsultarAlmacenes()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARARALMACENES");
                throw new ApplicationException("ERROR EN: ConsultarAlmacenes()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Almacen;
        }
        public async Task<int> EliminarAlmacen(int Id)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@ID", Id);
                _data = await _ldato.EjecutarScalarReader("PED_USP_ELIMINARALMACEN", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "AlmacenRepository", "EliminarAlmacen()", ex.Message.ToString(), null, null, "PED_USP_ELIMINARALMACEN");
            }
            return short.Parse(_data.Valor.ToString());
        }
    }
}
