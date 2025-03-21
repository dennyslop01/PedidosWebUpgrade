using PedidosWebUpgrade.Application.Common.Interfaces;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.DbContext;
using PedidosWebUpgrade.Infrastructure.Utilities;
using System.Data;
using System.Data.SqlClient;

namespace PedidosWebUpgrade.Infrastructure.Repository
{
    public class MenuRepository: IMenuRepository
    {
        private readonly ConfigVariables _configVariables;

        public MenuRepository(ConfigVariables configVariables)
        {
            _configVariables = configVariables;
        }

        public async Task<List<Menu>> ObtenerMenu(int IdMenu)
        {
            lDato _ldato = new lDato(_configVariables);
            List<Menu> _Menu = new List<Menu>();
            DataResponse<List<Menu>> List_Response = new DataResponse<List<Menu>>();
            try
            {
                _ldato.Parametros.Add("@IDMENU", (IdMenu == 0 ? (object)DBNull.Value : IdMenu));
                _ldato.Esquema.Add("IdMenu", "IDMENU");
                _ldato.Esquema.Add("IdPadre", "IDPADRE");
                _ldato.Esquema.Add("Descripcion", "DESCRIPCION");
                _ldato.Esquema.Add("Orden", "ORDEN");
                _ldato.Esquema.Add("Icon", "ICON");
                _ldato.Esquema.Add("Url", "URL");
                _ldato.Esquema.Add("Activo", "ACTIVO");
                _ldato.Esquema.Add("Visible", "VISIBLE");

                List_Response = await _ldato.EjecutarReader(new Menu(), "CON_USP_CONSULTARMENU", _ldato.Parametros, _ldato.Esquema);
                _Menu = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"MenuRepository", "ObtenerMenu()", ex.Message.ToString(), null, null, "CON_USP_CONSULTARMENU");
                throw new ApplicationException("ERROR EN: ObtenerMenu()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Menu;
        }

        public async Task<int> EliminarMenu(int IdMenu)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@IDMENU", IdMenu);
                _data = await _ldato.EjecutarScalarReader("CON_USP_ELIMINARMENU", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"MenuRepository", "EliminarMenu()", ex.Message.ToString(), null, null, "CON_USP_ELIMINARMENU");
            }
            return (int)_data.Valor;
        }

        public async Task<Dictionary<string, object>> ActualizarMenu(Menu _Menu)
        {
            lDato _ldato = new lDato(_configVariables);
            try
            {
                List<SqlParameter> _parametros = new List<SqlParameter> { new SqlParameter() {ParameterName = "@IDMENU",  Value =  _Menu.IdMenu, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@IDPADRE",  Value =  _Menu.IdPadre, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@DESCRIPCION",  Value =  _Menu.Descripcion, SqlDbType = SqlDbType.VarChar, Size = 250},
                                                                          new SqlParameter() {ParameterName = "@ORDEN",  Value =  _Menu.Orden, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@ICON",  Value =  _Menu.Icon, SqlDbType = SqlDbType.VarChar, Size = 50},
                                                                          new SqlParameter() {ParameterName = "@URL",  Value =  _Menu.Url, SqlDbType = SqlDbType.VarChar, Size = 350},
                                                                          new SqlParameter() {ParameterName = "@ACTIVO",  Value =  _Menu.Activo, SqlDbType = SqlDbType.Bit},
                                                                          new SqlParameter() {ParameterName = "@VISIBLE",  Value =  _Menu.Visible, SqlDbType = SqlDbType.Bit},
                                                                          new SqlParameter() {ParameterName = "@IDMENUOUT",  Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@RESULTADO", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.TinyInt},
                                                                         };
                _ldato.ParametrosSql = _parametros;
                var resultado = await _ldato.EjecutarNonQueryOutput("CON_USP_ACTUALIZARMENU", _ldato.ParametrosSql);
                return resultado.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"MenuRepository", "ActualizarMenu()", ex.ToString(), null, null, "CON_USP_ACTUALIZARMENU");
                throw new ApplicationException("ERROR EN: ActualizarMenu", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            ;
        }

        public async Task<List<Menu>> ObtenerMenuUsuario(int IdUsuario)
        {
            lDato _ldato = new lDato(_configVariables);
            List<Menu> _Menu = new List<Menu>();
            DataResponse<List<Menu>> List_Response = new DataResponse<List<Menu>>();
            try
            {
                _ldato.Parametros.Add("@IDUSUARIO", IdUsuario);
                _ldato.Esquema.Add("IdMenu", "IDMENU");
                _ldato.Esquema.Add("IdPadre", "IDPADRE");
                _ldato.Esquema.Add("Descripcion", "DESCRIPCION");
                _ldato.Esquema.Add("Orden", "ORDEN");
                _ldato.Esquema.Add("Icon", "ICON");
                _ldato.Esquema.Add("Url", "URL");

                List_Response = await _ldato.EjecutarReader(new Menu(), "CON_USP_CONSULTARMENU_USUARIO", _ldato.Parametros, _ldato.Esquema);
                _Menu = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"MenuRepository", "ObtenerMenuUsuario()", ex.Message.ToString(), null, null, "CON_USP_CONSULTARMENU_USUARIO");
                throw new ApplicationException("ERROR EN: ObtenerMenuUsuario()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Menu;
        }
    }
}
