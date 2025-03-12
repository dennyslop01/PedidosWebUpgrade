using PedidosWebUpgrade.Application.Common.Interfaces;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.DbContext;
using PedidosWebUpgrade.Infrastructure.Utilities;
using System.Data;
using System.Data.SqlClient;

namespace PedidosWebUpgrade.Infrastructure.Repository
{
    public class UsuarioRepository: IUsuarioRepository
    {
        private readonly ConfigVariables _configVariables;

        public UsuarioRepository(ConfigVariables configVariables)
        {
            _configVariables = configVariables;
        }

        /// <summary>
        /// VALIDA SESION DE UN USUARIO DADO USUARIO Y PASSWORD
        /// </summary>
        /// <param name="login">string</param>
        /// <param name="password">string</param>
        /// <returns>Dictionary<string, object></returns>
        public Dictionary<string, object> ValidarUsuario(string login, string password)
        {
            lDato _ldato = new lDato(_configVariables);
            try
            {
                List<SqlParameter> _parametros = [ new SqlParameter() {ParameterName = "@USUARIO",  Value =  login, SqlDbType = SqlDbType.VarChar, Size = 20},
                                                                          new SqlParameter() {ParameterName = "@CLAVE",  Value =  password, SqlDbType = SqlDbType.VarChar, Size = 44},
                                                                          new SqlParameter() {ParameterName = "@RESULTADO", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.TinyInt},
                                                                          new SqlParameter() {ParameterName = "@IDUSUARIOOUT",  Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@MOTIVOOUT",  Direction = ParameterDirection.Output, SqlDbType = SqlDbType.VarChar, Size = 50},
                                                                          new SqlParameter() {ParameterName = "@IDVENDEDOROUT",  Direction = ParameterDirection.Output, SqlDbType = SqlDbType.VarChar, Size = 15},
                                                                          new SqlParameter() {ParameterName = "@IsLoginByUrl", Value = false  , SqlDbType = SqlDbType.Bit}
                                                                         ];
                _ldato.ParametrosSql = _parametros;
                var resultado = _ldato.EjecutarNonQueryOutput("CON_USP_AUTENTICARUSUARIO", _ldato.ParametrosSql);
                return resultado.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"UsuarioRepository", "ValidarUsuario()", ex.ToString(), null, null, "CON_USP_AUTENTICARUSUARIO");
                throw new ApplicationException("ERROR EN: ValidarUsuario", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            ;
        }
        /// <summary>
        /// RETORNA ENTIDAD USUARIO
        /// </summary>
        /// <param name="IdUsuario">int</param>
        /// <returns>Usuario</returns>
        public List<Usuario> ObtenerUsuario(int IdUsuario)
        {
            lDato _ldato = new lDato(_configVariables);
            List<Usuario> _Usuario = [];
            DataResponse<List<Usuario>> List_Response = new DataResponse<List<Usuario>>();
            try
            {
                _ldato.Parametros.Add("@IDUSUARIO", (IdUsuario == 0 ? (object)DBNull.Value : IdUsuario));
                _ldato.Esquema.Add("IdUsuario", "ID_USUARIO");
                _ldato.Esquema.Add("Cedula", "CEDULA");
                _ldato.Esquema.Add("Direccion", "DIRECCION");
                _ldato.Esquema.Add("Email", "EMAIL");
                _ldato.Esquema.Add("IdEstado", "ESTADO");
                _ldato.Esquema.Add("Login", "LOGIN");
                _ldato.Esquema.Add("Password", "PASSWORD");
                _ldato.Esquema.Add("PrimerApellido", "PRIMER_APELLIDO");
                _ldato.Esquema.Add("PrimerNombre", "PRIMER_NOMBRE");
                _ldato.Esquema.Add("SegundoNombre", "SEGUNDO_APELLIDO");
                _ldato.Esquema.Add("SegundoApellido", "SEGUNDO_NOMBRE");
                _ldato.Esquema.Add("Sexo", "SEXO");
                _ldato.Esquema.Add("Telefono", "TELEFONO");
                _ldato.Esquema.Add("usuario_auditoria", "USUARIO_AUDITORIA");
                _ldato.Esquema.Add("TipoUsuario", "TIPOUSUARIO");
                _ldato.Esquema.Add("Region", "REGION");
                _ldato.Esquema.Add("MailCoordinador", "MAIL_COORDINADOR");



                List_Response = _ldato.EjecutarReader(new Usuario(), "CON_USP_CONSULTARUSUARIO", _ldato.Parametros, _ldato.Esquema);
                _Usuario = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"UsuarioRepository", "ObtenerUsuario()", ex.Message.ToString(), null, null, "CON_USP_CONSULTARUSUARIO");
                throw new ApplicationException("ERROR EN: ObtenerUsuario()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Usuario;
        }

        /// <summary>
        /// RETORNA ENTIDAD SALESMAN
        /// </summary>
        /// <param name="IdSalesman">int</param>
        /// <returns>List<Salesman></returns>
        public List<Salesman> ObtenerSalesman(string IdSalesman)
        {
            lDato _ldato = new lDato(_configVariables);
            List<Salesman> _Salesman = [];
            DataResponse<List<Salesman>> List_Response = new DataResponse<List<Salesman>>();
            try
            {
                _ldato.Parametros.Add("@SALESMANID", IdSalesman);
                _ldato.Esquema.Add("Salesmanid", "SALESMAN_ID");
                _ldato.Esquema.Add("Name", "NAME");
                _ldato.Esquema.Add("Region", "REGION");
                _ldato.Esquema.Add("Mail", "EMAIL");
                _ldato.Esquema.Add("MailCoordinador", "MAIL_COORDINADOR");
                _ldato.Esquema.Add("Estado", "ESTADO");


                List_Response = _ldato.EjecutarReader(new Salesman(), "CON_USP_CONSULTARSALESMEN", _ldato.Parametros, _ldato.Esquema);
                _Salesman = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"UsuarioRepository", "ObtenerSalesman()", ex.Message.ToString(), null, null, "CON_USP_CONSULTARSALESMEN");
                throw new ApplicationException("ERROR EN: ObtenerSalesman()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Salesman;
        }
        public int EliminarUsuario(int IdUsuario)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@ID", IdUsuario);
                _data = _ldato.EjecutarScalarReader("CON_USP_ELIMINARUSUARIO", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"UsuarioRepository", "EliminarUsuario()", ex.Message.ToString(), null, null, "CON_USP_ELIMINARUSUARIO");
            }
            return (int)_data.Valor;
        }

        public Dictionary<string, object> ActualizarUsuario(Usuario _usuario)
        {
            lDato _ldato = new lDato(_configVariables);
            try
            {
                List<SqlParameter> _parametros = [ new SqlParameter() {ParameterName = "@IDUSUARIO",  Value =  _usuario.IdUsuario, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@CEDULA",  Value =  _usuario.Cedula, SqlDbType = SqlDbType.VarChar, Size = 15},
                                                                          new SqlParameter() {ParameterName = "@DIRECCION",  Value =  _usuario.Direccion, SqlDbType = SqlDbType.VarChar, Size = 500},
                                                                          new SqlParameter() {ParameterName = "@EMAIL",  Value =  _usuario.Email, SqlDbType = SqlDbType.VarChar, Size = 50},
                                                                          new SqlParameter() {ParameterName = "@ESTADO",  Value =  _usuario.IdEstado, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@LOGIN",  Value =  _usuario.Login, SqlDbType = SqlDbType.VarChar, Size = 50},
                                                                          new SqlParameter() {ParameterName = "@PASSWORD",  Value =  _usuario.Password, SqlDbType = SqlDbType.VarChar, Size = 256},
                                                                          new SqlParameter() {ParameterName = "@PRIMERAPELLIDO",  Value =  _usuario.PrimerApellido, SqlDbType = SqlDbType.VarChar, Size = 100},
                                                                          new SqlParameter() {ParameterName = "@PRIMERNOMBRE",  Value =  _usuario.PrimerNombre, SqlDbType = SqlDbType.VarChar, Size = 100},
                                                                          new SqlParameter() {ParameterName = "@SEGUNDOAPELLIDO",  Value =  _usuario.SegundoApellido, SqlDbType = SqlDbType.VarChar, Size = 100},
                                                                          new SqlParameter() {ParameterName = "@SEGUNDONOMBRE",  Value =  _usuario.SegundoNombre, SqlDbType = SqlDbType.VarChar, Size = 100},
                                                                          new SqlParameter() {ParameterName = "@SEXO",  Value =  _usuario.Sexo, SqlDbType = SqlDbType.VarChar, Size = 1},
                                                                          new SqlParameter() {ParameterName = "@TELEF",  Value =  _usuario.Telefono, SqlDbType = SqlDbType.VarChar, Size = 50},
                                                                          new SqlParameter() {ParameterName = "@MAILCOORDINADOR",  Value =  _usuario.MailCoordinador, SqlDbType = SqlDbType.VarChar, Size = 50},
                                                                          new SqlParameter() {ParameterName = "@TIPOUSUARIO",  Value =  _usuario.TipoUsuario, SqlDbType = SqlDbType.VarChar, Size = 5},
                                                                          new SqlParameter() {ParameterName = "@REGION",  Value =  _usuario.Region, SqlDbType = SqlDbType.VarChar, Size = 3},
                                                                          new SqlParameter() {ParameterName = "@IDNUSUARIOOUT",  Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@RESULTADO", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.TinyInt},
                                                                         ];
                _ldato.ParametrosSql = _parametros;
                var resultado = _ldato.EjecutarNonQueryOutput("CON_USP_ACTUALIZARUSUARIO", _ldato.ParametrosSql);
                return resultado.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"UsuarioRepository", "ActualizarUsuario()", ex.ToString(), null, null, "CON_USP_ACTUALIZARUSUARIO");
                throw new ApplicationException("ERROR EN: ActualizarUsuario", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            ;
        }

        public List<Menu> ObtenerPermisos(int IdUsuario, string url)
        {
            lDato _ldato = new lDato(_configVariables);
            List<Menu> _Menu = [];
            DataResponse<List<Menu>> List_Response = new DataResponse<List<Menu>>();
            try
            {
                _ldato.Parametros.Add("@IDUSUARIO", IdUsuario);
                _ldato.Parametros.Add("@URL", url);
                _ldato.Esquema.Add("IdMenu", "IDMENU");
                _ldato.Esquema.Add("PuedeCrear", "CREAR");
                _ldato.Esquema.Add("PuedeConsultar", "CONSULTAR");
                _ldato.Esquema.Add("PuedeActualizar", "ACTUALIZAR");
                _ldato.Esquema.Add("PuedeEliminar", "ELIMINAR");


                List_Response = _ldato.EjecutarReader(new Menu(), "CON_USP_CONSULTARPERMISOSUSUARIO", _ldato.Parametros, _ldato.Esquema);
                _Menu = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"UsuarioRepository", "ObtenerPermisos()", ex.Message.ToString(), null, null, "CON_USP_CONSULTARPERMISOSUSUARIO");
                throw new ApplicationException("ERROR EN: ObtenerPermisos()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Menu;
        }

        public List<ListaGeneral> ObtenerTipoUsuario()
        {
            lDato _ldato = new lDato(_configVariables);
            List<ListaGeneral> _TiposUsuario = [];
            DataResponse<List<ListaGeneral>> List_Response = new DataResponse<List<ListaGeneral>>();
            try
            {

                _ldato.Esquema.Add("Codigo", "TIPOUSUARIO");
                _ldato.Esquema.Add("Descripcion", "TIPOUSUARIO");
                List_Response = _ldato.EjecutarReader(new ListaGeneral(), "CON_USP_CONSULTARTIPOUSUARIO", _ldato.Parametros, _ldato.Esquema);
                _TiposUsuario = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"UsuarioRepository", "ObtenerTipoUsuario()", ex.Message.ToString(), null, null, "CON_USP_CONSULTARTIPOUSUARIO");
                throw new ApplicationException("ERROR EN: ObtenerTipoUsuario()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _TiposUsuario;
        }
    }
}
