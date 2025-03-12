using PedidosWebUpgrade.Application.Common.Interfaces;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.DbContext;
using PedidosWebUpgrade.Infrastructure.Utilities;
using System.Data;
using System.Data.SqlClient;

namespace PedidosWebUpgrade.Infrastructure.Repository
{
    public class PerfilRepository: IPerfilRepository
    {
        private readonly ConfigVariables _configVariables;

        public PerfilRepository(ConfigVariables configVariables)
        {
            _configVariables = configVariables;
        }
        
        public List<Perfil> ObtenerPerfil(int IdPerfil)
        {
            lDato _ldato = new lDato(_configVariables);
            List<Perfil> _Perfil = new List<Perfil>();
            DataResponse<List<Perfil>> List_Response = new DataResponse<List<Perfil>>();
            try
            {
                _ldato.Parametros.Add("@IDPERFIL", (IdPerfil == 0 ? (object)DBNull.Value : IdPerfil));
                _ldato.Esquema.Add("IdPerfil", "IDPERFIL");
                _ldato.Esquema.Add("Nombre", "NOMBRE");
                _ldato.Esquema.Add("Descripcion", "DESCRIPCION");
                _ldato.Esquema.Add("Activo", "ACTIVO");

                List_Response = _ldato.EjecutarReader(new Perfil(), "CON_USP_CONSULTARPERFIL", _ldato.Parametros, _ldato.Esquema);
                _Perfil = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilRepository", "ObtenerPerfil()", ex.Message.ToString(), null, null, "CON_USP_CONSULTARPERFIL");
                throw new ApplicationException("ERROR EN: ObtenerPerfil()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Perfil;
        }

        public int EliminarPerfil(int IdPerfil)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@IDPERFIL", IdPerfil);
                _data = _ldato.EjecutarScalarReader("CON_USP_ELIMINARPERFIL", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilRepository", "EliminarPerfil()", ex.Message.ToString(), null, null, "CON_USP_ELIMINARPERFIL");
            }
            return (int)_data.Valor;
        }

        public Dictionary<string, object> ActualizarPerfil(Perfil _perfil)
        {
            lDato _ldato = new lDato(_configVariables);
            try
            {
                List<SqlParameter> _parametros = new List<SqlParameter> { new SqlParameter() {ParameterName = "@IDPERFIL",  Value =  _perfil.IdPerfil, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@NOMBRE",  Value =  _perfil.Nombre, SqlDbType = SqlDbType.VarChar, Size = 50},
                                                                          new SqlParameter() {ParameterName = "@DESCRIPCION",  Value =  _perfil.Descripcion, SqlDbType = SqlDbType.VarChar, Size = 150},
                                                                          new SqlParameter() {ParameterName = "@ACTIVO",  Value =  _perfil.Activo, SqlDbType = SqlDbType.Bit},
                                                                          new SqlParameter() {ParameterName = "@IDPERFILOUT",  Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@RESULTADO", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.TinyInt},
                                                                         };
                _ldato.ParametrosSql = _parametros;
                var resultado = _ldato.EjecutarNonQueryOutput("CON_USP_ACTUALIZARPERFIL", _ldato.ParametrosSql);
                return resultado.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilRepository", "ActualizaRPerfil()", ex.ToString(), null, null, "CON_USP_ACTUALIZARPERFIL");
                throw new ApplicationException("ERROR EN: ActualizaRPerfil", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            ;
        }

        public List<PerfilMenu> ObtenerPerfilMenu(int IdPerfil, int IdMenu)
        {
            lDato _ldato = new lDato(_configVariables);
            List<PerfilMenu> _PerfilMenu = new List<PerfilMenu>();
            DataResponse<List<PerfilMenu>> List_Response = new DataResponse<List<PerfilMenu>>();
            try
            {
                _ldato.Parametros.Add("@IDPERFIL", (IdPerfil == 0 ? (object)DBNull.Value : IdPerfil));
                _ldato.Parametros.Add("@IDMENU", (IdMenu == 0 ? (object)DBNull.Value : IdMenu));
                _ldato.Esquema.Add("Idperfilmenu", "IDPERFILMENU");
                _ldato.Esquema.Add("Idperfil", "IDPERFIL");
                _ldato.Esquema.Add("Idmenu", "IDMENU");
                _ldato.Esquema.Add("Descripcion", "DESCRIPCION");
                _ldato.Esquema.Add("Url", "URL");


                List_Response = _ldato.EjecutarReader(new PerfilMenu(), "CON_USP_CONSULTARPERFILMENU", _ldato.Parametros, _ldato.Esquema);
                _PerfilMenu = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilRepository", "ObtenerPerfilMenu()", ex.Message.ToString(), null, null, "CON_USP_CONSULTARPERFILMENU");
                throw new ApplicationException("ERROR EN: ObtenerPerfilMenu()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _PerfilMenu;
        }

        public int EliminarPerfilMenu(int IdPerfilMenu)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@IDPERFILMENU", IdPerfilMenu);
                _data = _ldato.EjecutarScalarReader("CON_USP_ELIMINARPERFILMENU", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilRepository", "EliminarPerfilMenu()", ex.Message.ToString(), null, null, "CON_USP_ELIMINARPERFILMENU");
            }
            return (int)_data.Valor;
        }

        public bool IncluirPerfilMenu(PerfilMenu _PerfilMenu)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@IDPERFIL", _PerfilMenu.Idperfil);
                _ldato.Parametros.Add("@IDMENU", _PerfilMenu.Idmenu);
                _data = _ldato.EjecutarScalarReader("CON_USP_INCLUIRPERFILMENU", _ldato.Parametros);

            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilRepository", "IncluirPerfilMenu()", ex.ToString(), null, null, "CON_USP_INCLUIRPERFILMENU");
                throw new ApplicationException("ERROR EN: IncluirPerfilMenu", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            ;
            return (bool)_data.Valor;
        }

        public List<PerfilUsuario> ObtenerPerfilUsuario(int IdPerfil, decimal IdUsuario)
        {
            lDato _ldato = new lDato(_configVariables);
            List<PerfilUsuario> _PerfilUsuario = new List<PerfilUsuario>();
            DataResponse<List<PerfilUsuario>> List_Response = new DataResponse<List<PerfilUsuario>>();
            try
            {
                _ldato.Parametros.Add("@IDUSUARIO", (IdUsuario == 0 ? (object)DBNull.Value : IdUsuario));
                _ldato.Parametros.Add("@IDPERFIL", (IdPerfil == 0 ? (object)DBNull.Value : IdPerfil));

                _ldato.Esquema.Add("IdPerfilUsuario", "IDPERFILUSUARIO");
                _ldato.Esquema.Add("IdPerfil", "IDPERFIL");
                _ldato.Esquema.Add("IdUsuario", "IDUSUARIO");
                _ldato.Esquema.Add("NombrePerfil", "DESCRIPCION");


                List_Response = _ldato.EjecutarReader(new PerfilUsuario(), "CON_USP_CONSULTARPERFILUSUARIO", _ldato.Parametros, _ldato.Esquema);
                _PerfilUsuario = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilRepository", "ObtenerPerfilUsuario()", ex.Message.ToString(), null, null, "CON_USP_CONSULTARPERFILUSUARIO");
                throw new ApplicationException("ERROR EN: ObtenerPerfilUsuario()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _PerfilUsuario;
        }

        public int EliminarPerfilUsuario(int IdPerfilUsuario)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@IDPERFILUSUARIO", IdPerfilUsuario);
                _data = _ldato.EjecutarScalarReader("CON_USP_ELIMINARPERFILUSUARIO", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilRepository", "EliminarPerfilUsuario()", ex.Message.ToString(), null, null, "CON_USP_ELIMINARPERFILUSUARIO");
            }
            return (int)_data.Valor;
        }

        public bool IncluirPerfilUsuario(PerfilUsuario _PerfilUsuario)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@IDPERFIL", _PerfilUsuario.IdPerfil);
                _ldato.Parametros.Add("@IDUSUARIO", _PerfilUsuario.IdUsuario);
                _data = _ldato.EjecutarScalarReader("CON_USP_INCLUIRPERFILUSUARIO", _ldato.Parametros);

            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilRepository", "IncluirPerfilUsuario()", ex.ToString(), null, null, "CON_USP_INCLUIRPERFILUSUARIO");
                throw new ApplicationException("ERROR EN: IncluirPerfilUsuario", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            ;
            return (bool)_data.Valor;
        }

        public int ActualizarMiPerfil(Usuario MiPerfil)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@IDUSUARIO", MiPerfil.IdUsuario);
                _ldato.Parametros.Add("@CONTRASENA", MiPerfil.ConfirmarPassword);
                _data = _ldato.EjecutarScalarReader("CON_USP_ACTUALIZARMIPERFIL", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilRepository", "ActualizarMiPerfil()", ex.Message.ToString(), null, null, "CON_USP_ACTUALIZARMIPERFIL");
            }
            return (int)_data.Valor;
        }

    }
}
