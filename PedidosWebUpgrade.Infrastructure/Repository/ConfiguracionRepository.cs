using PedidosWebUpgrade.Application.Common.Interfaces;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.DbContext;
using PedidosWebUpgrade.Infrastructure.Utilities;
using System.Data;
using System.Data.SqlClient;

namespace PedidosWebUpgrade.Infrastructure.Repository
{
    public class ConfiguracionRepository: IConfiguracionRepository
    {
        private readonly ConfigVariables _configVariables;

        public ConfiguracionRepository(ConfigVariables configVariables)
        {
            _configVariables = configVariables;
        }

        public async Task<List<Configuracion>> ObtenerParametros(String categoria)
        {
            lDato _ldato = new lDato(_configVariables);
            DataResponse<List<Configuracion>> List_Response = new DataResponse<List<Configuracion>>();
            List<Configuracion> _result = [];

            try
            {
                _ldato.Parametros.Add("@CATEGORIA", categoria);
                _ldato.Parametros.Add("@PARAMETRO", DBNull.Value);
                _ldato.Esquema.Add("Codigo", "PARAMETRO");
                _ldato.Esquema.Add("Descripcion", "DESCRIPCION");
                _ldato.Esquema.Add("Valor", "VALOR");
                List_Response = await _ldato.EjecutarReader(new Configuracion(), "CON_USP_CONSULTARPARAMETRO", _ldato.Parametros, _ldato.Esquema);
                _result = List_Response.Valor;
                return _result;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionRepository", "ObtenerParametros()", ex.Message.ToString(), null, null, "CON_USP_CONSULTARPARAMETRO");
                throw new ApplicationException("ERROR EN: ConfiguracionRepository-ObtenerParametros()", ex);
            }
            finally
            {
                _ldato.Dispose();
            }
        }

        public async Task<List<Sistema>> ConsultarSistema()
        {
            lDato _ldato = new lDato(_configVariables);
            DataResponse<List<Sistema>> List_Response = new DataResponse<List<Sistema>>();
            List<Sistema> _result = [];

            try
            {

                _ldato.Esquema.Add("Id", "ID");
                _ldato.Esquema.Add("Estado", "ESTADO");
                _ldato.Esquema.Add("Motivo", "MOTIVO");
                _ldato.Esquema.Add("Usuario", "USUARIO");
                _ldato.Esquema.Add("Fecha", "FECHA");
                _ldato.Esquema.Add("Hora", "HORA");
                _ldato.Esquema.Add("Destinatarios", "DESTINATARIOS");
                List_Response = await _ldato.EjecutarReader(new Sistema(), "CON_USP_CONSULTARSISTEMA", _ldato.Parametros, _ldato.Esquema);
                _result = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionRepository", "ConsultarSistema()", ex.Message.ToString(), null, null, "CON_USP_CONSULTARSISTEMA");
                throw new ApplicationException("ERROR EN: ConfiguracionRepository-ConsultarSistema()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _result;
        }

        public async Task<Dictionary<string, object>> ActualizarSistema(Sistema _Sistema)
        {
            lDato _ldato = new lDato(_configVariables);
            try
            {
                List<SqlParameter> _parametros = [ new SqlParameter() {ParameterName = "@IDSISTEMA",  Value =  _Sistema.Id, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@ESTADO",  Value =  _Sistema.Estado, SqlDbType = SqlDbType.VarChar, Size = 50},
                                                                          new SqlParameter() {ParameterName = "@MOTIVO",  Value =  _Sistema.Motivo, SqlDbType = SqlDbType.VarChar, Size = 50},
                                                                          new SqlParameter() {ParameterName = "@USUARIO",  Value =  _Sistema.Usuario, SqlDbType = SqlDbType.VarChar, Size = 50},
                                                                          new SqlParameter() {ParameterName = "@DESTINATARIOS",  Value =  _Sistema.Destinatarios, SqlDbType = SqlDbType.VarChar, Size = 500},
                                                                          new SqlParameter() {ParameterName = "@IDSISTEMAOUT",  Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@RESULTADO", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.TinyInt},
                                                                         ];
                _ldato.ParametrosSql = _parametros;
                var resultado = await _ldato.EjecutarNonQueryOutput("CON_USP_ACTUALIZARSISTEMA", _ldato.ParametrosSql);
                return resultado.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionRepository", "ActualizarSistema()", ex.ToString(), null, null, "CON_USP_ACTUALIZARSISTEMA");
                throw new ApplicationException("ERROR EN: ActualizarSistema", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            ;
        }

        public async Task<List<ContadorPedidosPais>> ConsultarPedidosPais()
        {
            lDato _ldato = new lDato(_configVariables);
            DataResponse<List<ContadorPedidosPais>> List_Response = new DataResponse<List<ContadorPedidosPais>>();
            List<ContadorPedidosPais> _result = [];

            try
            {
                _ldato.Esquema.Add("CodigoPais", "cod_pais");
                _ldato.Esquema.Add("Contador", "contador");
                _ldato.Esquema.Add("Pais", "nombre_pais");
                _ldato.Esquema.Add("DiasTravesia", "dias_travesia");
                _ldato.Esquema.Add("AnnoCurso", "anno");
                List_Response = await _ldato.EjecutarReader(new ContadorPedidosPais(), "PED_USP_CONSULTARCONTADORPEDIDOPAIS", _ldato.Parametros, _ldato.Esquema);
                _result = List_Response.Valor;
                return _result;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionRepository", "ConsultarPedidosPais()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARCONTADORPEDIDOPAIS");
                throw new ApplicationException("ERROR EN: ConfiguracionRepository-ConsultarPedidosPais()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
        }

        public async Task<int> ActualizarContadorPais(ContadorPedidosPais _ContadorPais)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@CODPAIS", _ContadorPais.CodigoPais);
                _ldato.Parametros.Add("@CONTADOR", _ContadorPais.Contador);
                _ldato.Parametros.Add("@DIASTRAVESIA", _ContadorPais.DiasTravesia);
                _ldato.Parametros.Add("@ANNO", _ContadorPais.AnnoCurso);

                _data = await _ldato.EjecutarScalarReader("PED_USP_ACTUALIZARCONTADORPEDIDOPAIS", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionRepository", "ActualizarContadorPais()", ex.Message.ToString(), null, null, "PED_USP_ACTUALIZARCONTADORPEDIDOPAIS");
            }
            return (int)_data.Valor;
        }

        public async Task<int> CopiarContadoresAnno(int annoCurso, int AnnoNuevo)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@ANNOACTUAL", annoCurso);
                _ldato.Parametros.Add("@ANNONUEVO", AnnoNuevo);

                _data = await _ldato.EjecutarScalarReader("PED_USP_COPIARCONTADORNUEVOANNO", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionRepository", "CopiarContadoresAnno()", ex.Message.ToString(), null, null, "PED_USP_COPIARCONTADORNUEVOANNO");
            }
            return (int)_data.Valor;
        }

        public async Task<List<F0004>> ObtenerF0004(string Dtsy, string Dtrt)
        {
            lDato _ldato = new lDato(_configVariables);
            List<F0004> _ListF0004 = [];
            DataResponse<List<F0004>> List_Response = new DataResponse<List<F0004>>();
            try
            {
                _ldato.Parametros.Add("@DTSY", Dtsy ?? (object)DBNull.Value);
                _ldato.Parametros.Add("@DTRT", Dtrt ?? (object)DBNull.Value);
                _ldato.Esquema.Add("dtrt", "dtrt");
                _ldato.Esquema.Add("dtsy", "dtsy");
                _ldato.Esquema.Add("dtdl01", "dtdl01");
                List_Response = await _ldato.EjecutarReader(new F0004(), "CON_USP_CONSULTARF0004", _ldato.Parametros, _ldato.Esquema);
                _ListF0004 = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionRepository", "ObtenerF0004()", ex.Message.ToString(), null, null, "CON_USP_CONSULTARF0004");
                throw new ApplicationException("ERROR EN: ObtenerF0004()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _ListF0004;
        }

        public async Task<int> ActualizarF0004(F0004 _ModelF0004)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@DTRT", _ModelF0004.dtrt);
                _ldato.Parametros.Add("@DTSY", _ModelF0004.dtsy);
                _ldato.Parametros.Add("@DTDL01", _ModelF0004.dtdl01);

                _data = await _ldato.EjecutarScalarReader("CON_USP_ACTUALIZARF0004", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionRepository", "ActualizarF0004()", ex.Message.ToString(), null, null, "CON_USP_ACTUALIZARF0004");
            }
            return (int)_data.Valor;
        }

        public async Task<int> EliminarF0004(string Dtsy, string Dtrt)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@DTRT", Dtrt);
                _ldato.Parametros.Add("@DTSY", Dtsy);
                _data = await _ldato.EjecutarScalarReader("CON_USP_ELIMINARF0004", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionRepository", "EliminarF0004()", ex.Message.ToString(), null, null, "CON_USP_ELIMINARF0004");
            }
            return (int)_data.Valor;
        }

        public async Task<List<F0005>> ObtenerF0005(string drsy, string drrt, string drky)
        {
            lDato _ldato = new lDato(_configVariables);
            List<F0005> _ListF0005 = [];
            DataResponse<List<F0005>> List_Response = new DataResponse<List<F0005>>();
            try
            {
                _ldato.Parametros.Add("@DRSY", (string.IsNullOrEmpty(drsy) ? (object)DBNull.Value : drsy));
                _ldato.Parametros.Add("@DRRT", (string.IsNullOrEmpty(drrt) ? (object)DBNull.Value : drrt));
                _ldato.Parametros.Add("@DRKY", (string.IsNullOrEmpty(drky) ? (object)DBNull.Value : drky));
                _ldato.Esquema.Add("drky", "DRKY");
                _ldato.Esquema.Add("drrt", "DRRT");
                _ldato.Esquema.Add("drsy", "DRSY");
                _ldato.Esquema.Add("drdl01", "DRDL01");
                _ldato.Esquema.Add("drdl02", "DRDL02");
                _ldato.Esquema.Add("drsphd", "DRSPHD");
                List_Response = await _ldato.EjecutarReader(new F0005(), "CON_USP_CONSULTARF0005", _ldato.Parametros, _ldato.Esquema);
                _ListF0005 = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionRepository", "ObtenerF0005()", ex.Message.ToString(), null, null, "CON_USP_CONSULTARF0005");
                throw new ApplicationException("ERROR EN: ObtenerF0004()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _ListF0005;
        }

        public async Task<int> ActualizarF0005(F0005 _ModelF0005)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@DRRT", _ModelF0005.drrt);
                _ldato.Parametros.Add("@DRSY", _ModelF0005.drsy);
                _ldato.Parametros.Add("@DRKY", _ModelF0005.drky);
                _ldato.Parametros.Add("@DRDL01", _ModelF0005.drdl01);
                _ldato.Parametros.Add("@DRDL02", _ModelF0005.drdl02);
                _ldato.Parametros.Add("@DRSPHD", _ModelF0005.drsphd);

                _data = await _ldato.EjecutarScalarReader("CON_USP_ACTUALIZARF0005", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionRepository", "ActualizarF0005()", ex.Message.ToString(), null, null, "CON_USP_ACTUALIZARF0005");
            }
            return (int)_data.Valor;
        }

        public async Task<int> EliminarF0005(string Drrt, string Drsy, string Drky)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@DRRT", Drrt);
                _ldato.Parametros.Add("@DRSY", Drsy);
                _ldato.Parametros.Add("@DRKY", Drky);
                _data = await _ldato.EjecutarScalarReader("CON_USP_ELIMINARF0005", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionRepository", "EliminarF0004()", ex.Message.ToString(), null, null, "CON_USP_ELIMINARF0005");
            }
            return (int)_data.Valor;
        }

        public async Task<List<PreferenciaAlmacenClientePais>> ObtenerPreferenciaAlmacenClientePais(int IdPreferencia)
        {
            lDato _ldato = new lDato(_configVariables);
            List<PreferenciaAlmacenClientePais> _ListPreferencias = [];
            DataResponse<List<PreferenciaAlmacenClientePais>> List_Response = new DataResponse<List<PreferenciaAlmacenClientePais>>();
            try
            {
                _ldato.Parametros.Add("@IDPREFERENCIA", (IdPreferencia == 0) ? Convert.DBNull : IdPreferencia);
                _ldato.Esquema.Add("IdPreferencia", "id_preferencia");
                _ldato.Esquema.Add("IdAlmacen", "id_almacen");
                _ldato.Esquema.Add("IdCliente", "customer_id");
                _ldato.Esquema.Add("Codpais", "cod_pais");
                _ldato.Esquema.Add("NombrePais", "DescripcionPais");
                _ldato.Esquema.Add("NombreAlmacen", "DescripcionAlmacen");
                _ldato.Esquema.Add("NombreCliente", "DescripcionCliente");
                List_Response = await _ldato.EjecutarReader(new PreferenciaAlmacenClientePais(), "PED_USP_CONSULTARPREFERENCIAALMACENCLIENTEPAIS", _ldato.Parametros, _ldato.Esquema);
                _ListPreferencias = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionRepository", "ObtenerPreferenciaAlmacenClientePais()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARPREFERENCIAALMACENCLIENTEPAIS");
                throw new ApplicationException("ERROR EN: ObtenerPreferenciaAlmacenClientePais()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _ListPreferencias;
        }

        public async Task<Dictionary<string, object>> ActualizarPreferenciaAlmacenClientePais(PreferenciaAlmacenClientePais preferenciaAlmacenClientePais)
        {
            lDato _ldato = new lDato(_configVariables);
            try
            {
                List<SqlParameter> _parametros = [
                                                                          new SqlParameter() {ParameterName = "@IDPREFERENCIA",  Value =  preferenciaAlmacenClientePais.IdPreferencia, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@IDALMACEN",  Value =  preferenciaAlmacenClientePais.IdAlmacen, SqlDbType = SqlDbType.VarChar, Size = 50},
                                                                          new SqlParameter() {ParameterName = "@IDCLIENTE",  Value =  preferenciaAlmacenClientePais.IdCliente, SqlDbType = SqlDbType.VarChar, Size = 8},
                                                                          new SqlParameter() {ParameterName = "@CODPAIS",  Value =  preferenciaAlmacenClientePais.Codpais, SqlDbType = SqlDbType.VarChar, Size = 15},
                                                                          new SqlParameter() {ParameterName = "@RESULTADO", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.SmallInt},
                                                                         ];
                _ldato.ParametrosSql = _parametros;
                var resultado = await _ldato.EjecutarNonQueryOutput("PED_USP_ACTUALIZARPREFERENCIAALMACENCLIENTEPAIS", _ldato.ParametrosSql);
                return resultado.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionRepository", "ActualizarPreferenciaAlmacenClientePais()", ex.ToString(), null, null, "PED_USP_ACTUALIZARPREFERENCIAALMACENCLIENTEPAIS");
                throw new ApplicationException("ERROR EN: ActualizarPreferenciaAlmacenClientePais", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            ;
        }

        public async Task<int> EliminarPreferenciaAlmacenClientePais(int IdPreferencia)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@IDPREFERENCIA", IdPreferencia);
                _data = await _ldato.EjecutarScalarReader("PED_USP_ELIMINARPREFERENCIAALMACENCLIENTEPAIS", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionRepository", "EliminarPreferenciaAlmacenClientePais()", ex.Message.ToString(), null, null, "PED_USP_ELIMINARPREFERENCIAALMACENCLIENTEPAIS");
            }
            return (int)_data.Valor;
        }

        public async Task<Dictionary<string, object>> ActualizarAgenteAduanal(ForwardingAgent forwardingAgent)
        {
            lDato _ldato = new lDato(_configVariables);
            try
            {
                List<SqlParameter> _parametros = [ new SqlParameter() {ParameterName = "@IDAGENT",  Value =  forwardingAgent.AgenteId, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@NOMBRE",  Value =  string.IsNullOrWhiteSpace(forwardingAgent.Nombre) ? Convert.DBNull : forwardingAgent.Nombre, SqlDbType = SqlDbType.VarChar, Size = 100},
                                                                          new SqlParameter() {ParameterName = "@LINEA2",  Value =  string.IsNullOrWhiteSpace(forwardingAgent.Linea2) ? Convert.DBNull : forwardingAgent.Linea2, SqlDbType = SqlDbType.VarChar, Size = 100},
                                                                          new SqlParameter() {ParameterName = "@LINEA3",  Value =  string.IsNullOrWhiteSpace(forwardingAgent.Linea3) ? Convert.DBNull : forwardingAgent.Linea3, SqlDbType = SqlDbType.VarChar, Size = 100},
                                                                          new SqlParameter() {ParameterName = "@LINEA4",  Value =  string.IsNullOrWhiteSpace(forwardingAgent.Linea4) ? Convert.DBNull : forwardingAgent.Linea4, SqlDbType = SqlDbType.VarChar, Size = 100},
                                                                          new SqlParameter() {ParameterName = "@LINEA5",  Value =  string.IsNullOrWhiteSpace(forwardingAgent.Linea5) ? Convert.DBNull : forwardingAgent.Linea5, SqlDbType = SqlDbType.VarChar, Size = 100},
                                                                          new SqlParameter() {ParameterName = "@LINEA6",  Value =  string.IsNullOrWhiteSpace(forwardingAgent.Linea6) ? Convert.DBNull : forwardingAgent.Linea6, SqlDbType = SqlDbType.VarChar, Size = 100},
                                                                          new SqlParameter() {ParameterName = "@LINEA7",  Value =  string.IsNullOrWhiteSpace(forwardingAgent.Linea7) ? Convert.DBNull : forwardingAgent.Linea7, SqlDbType = SqlDbType.VarChar, Size = 100},
                                                                          new SqlParameter() {ParameterName = "@LINEA8",  Value =  string.IsNullOrWhiteSpace(forwardingAgent.Linea8) ? Convert.DBNull : forwardingAgent.Linea8, SqlDbType = SqlDbType.VarChar, Size = 100},
                                                                          new SqlParameter() {ParameterName = "@RESULT", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Bit},
                                                                         ];
                _ldato.ParametrosSql = _parametros;
                var resultado = await _ldato.EjecutarNonQueryOutput("PED_USP_CREARAGENTEADUANAL", _ldato.ParametrosSql);
                return resultado.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionRepository", "ActualizarAgenteAduanal()", ex.ToString(), null, null, "PED_USP_CREARAGENTEADUANAL");
                throw new ApplicationException("ERROR EN: ActualizarAgenteAduanal", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            ;
        }

        public async Task<List<ForwardingAgent>> ConsultarAgentesAduanales()
        {
            lDato _ldato = new lDato(_configVariables);
            List<ForwardingAgent> _ListForwardingAgente = [];
            DataResponse<List<ForwardingAgent>> List_Response = new DataResponse<List<ForwardingAgent>>();
            try
            {
                _ldato.Esquema.Add("AgenteId", "id_agent");
                _ldato.Esquema.Add("Nombre", "nombre");
                _ldato.Esquema.Add("Linea2", "linea2");
                _ldato.Esquema.Add("Linea3", "linea3");
                _ldato.Esquema.Add("Linea4", "linea4");
                _ldato.Esquema.Add("Linea5", "linea5");
                _ldato.Esquema.Add("Linea6", "linea6");
                _ldato.Esquema.Add("Linea7", "linea7");
                _ldato.Esquema.Add("Linea8", "linea8");
                List_Response = await _ldato.EjecutarReader(new ForwardingAgent(), "PED_USP_CONSULTARAGENTESADUANALES", _ldato.Esquema);
                _ListForwardingAgente = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionRepository", "ConsultarAgentesAduanales()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARAGENTESADUANALES");
                throw new ApplicationException("ERROR EN: ConsultarAgentesAduanales()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _ListForwardingAgente;
        }

        public async Task<ForwardingAgent> ConsultarUnAgenteAduanal(int AgentId)
        {
            lDato _ldato = new lDato(_configVariables);
            List<ForwardingAgent> _ForwardingAgente = [];
            DataResponse<List<ForwardingAgent>> Response = new DataResponse<List<ForwardingAgent>>();
            try
            {
                _ldato.Parametros.Add("@IDAGENT", AgentId);
                _ldato.Esquema.Add("AgenteId", "id_agent");
                _ldato.Esquema.Add("Nombre", "nombre");
                _ldato.Esquema.Add("Linea2", "linea2");
                _ldato.Esquema.Add("Linea3", "linea3");
                _ldato.Esquema.Add("Linea4", "linea4");
                _ldato.Esquema.Add("Linea5", "linea5");
                _ldato.Esquema.Add("Linea6", "linea6");
                _ldato.Esquema.Add("Linea7", "linea7");
                _ldato.Esquema.Add("Linea8", "linea8");
                Response = await _ldato.EjecutarReader(new ForwardingAgent(), "PED_USP_CONSULTARUNAGENTEADUANAL", _ldato.Parametros, _ldato.Esquema);
                _ForwardingAgente = Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionRepository", "ConsultarUnAgenteAduanal()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARUNAGENTEADUANAL");
                throw new ApplicationException("ERROR EN: ConsultarUnAgenteAduanal()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _ForwardingAgente.FirstOrDefault();
        }

        public async Task<int> EliminarAgenteAduanal(int IdAgent)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@IDAGENT", IdAgent);
                _data = await _ldato.EjecutarScalarReader("PED_USP_ELIMINARAGENTEADUANAL", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionRepository", "EliminarAgenteAduanal()", ex.Message.ToString(), null, null, "PED_USP_ELIMINARAGENTEADUANAL");
            }
            return (int)_data.Valor;
        }
    }
}
