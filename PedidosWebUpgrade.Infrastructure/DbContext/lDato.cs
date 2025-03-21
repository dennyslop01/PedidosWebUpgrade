using Microsoft.Extensions.Options;
using PedidosWebUpgrade.Infrastructure.Utilities;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace PedidosWebUpgrade.Infrastructure.DbContext
{
    public class lDato : lBase
    {
        private readonly ConfigVariables _configVariables;

        #region "-- ATRIBUTOS --"
        public Dictionary<string, object> Parametros { get; set; }

        public Dictionary<string, object> ParametrosOut { get; set; }

        public List<System.Data.SqlClient.SqlParameter> ParametrosSql { get; set; }

        public Dictionary<string, string> Esquema { get; set; }
        #endregion

        #region "-- METODOS GENERALES --"
        public void InstanciarAtributos()
        {
            ParametrosSql = [];
            Parametros = [];
            Esquema = [];
        }

        public SqlParameter ParametroInput(string _nombre, object _valor)
        {
            SqlParameter _parametro = new SqlParameter(_nombre, _valor);
            _parametro.Direction = ParameterDirection.Input;
            return _parametro;
        }

        public SqlParameter ParametroOutput(string _nombre)
        {
            SqlParameter _parametro = new SqlParameter(_nombre, DBNull.Value);
            _parametro.Direction = ParameterDirection.Output;
            _parametro.Size = 100;
            return _parametro;
        }

        public object ObtenerValorDelDiccionario(Dictionary<string, object> _diccionario, string _key, object _valorPorDefecto = null)
        {
            object _obj = new object();
            if (_valorPorDefecto != null)
            {
                _obj = _valorPorDefecto;
            }


            foreach (KeyValuePair<string, object> _item in _diccionario)
            {
                if (_item.Key == _key)
                {
                    _obj = _item.Value;
                }
            }
            return _obj;
        }
        #endregion

        #region "-- CONSTRUCTOR --"
        /// <summary>
        /// Crea acceso a datos según cadena de configuracion del config
        /// </summary>
        public lDato(ConfigVariables configVariables) : base(configVariables)
        {
            _configVariables = configVariables;
            InstanciarAtributos();
        }
        #endregion

        #region "-- METODOS ACTUALIZACION --"
        /// <summary>
        /// Ejecuta un Procedimiento Almacenado que no define parámetros de entrada/salida
        /// </summary>
        /// <param name="nombreProcedimiento">Nombre del procedimiento a ejecutar</param>
        /// <param name="timeout">Tiempo de espera que debe esperar la ejecución antes genenar un error</param>
        /// <returns>Objeto DataResponse(Integer) en su propiedad Valor contiene el resultado obtenido de la ejecución</returns>
        /// <remarks>Puede utilizar la propiedad CodigoRetorno del objeto DataResponse para obtener el Valor de Retorno enviado desde el SP</remarks>
        /// 
        public async Task<DataResponse<int>> EjecutarNonQuery(string nombreProcedimiento, int timeout = 30)
        {
            DataResponse<int> resultado = new DataResponse<int>();

            try
            {
                using (SqlConnection connection = new SqlConnection(this.stringConnection))
                {
                    connection.Open();
                    using (SqlCommand comando = new SqlCommand(nombreProcedimiento, connection))
                    {
                        comando.CommandTimeout = timeout;
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "ValorRetorno",
                            Direction = ParameterDirection.ReturnValue
                        });

                        resultado.Valor = await comando.ExecuteNonQueryAsync();
                        resultado.CodigoRetorno = (int)comando.Parameters["ValorRetorno"].Value;
                    }

                    connection.Close();
                }
                return resultado;
            }
            catch (SqlException ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarNonQuery", ex.Message.ToString(), null, null, nombreProcedimiento);
                throw ex;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarNonQuery", ex.Message.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Ejecuta un Procedimiento Almacenado el cual define diccionario de parámetros(Clave:Valor)
        /// </summary>
        /// <param name="nombreProcedimiento">Nombre del procedimiento a ejecutar</param>
        /// <param name="parametros">Parametros de entrada (NombreParametro,Valor)</param>
        /// <param name="timeout">Tiempo de espera que debe esperar la ejecución antes genenar un error</param>
        /// <returns>Objeto DataResponse (Integer) en su propiedad Valor contiene el resultado obtenido de la ejecución</returns>
        /// <remarks>Puede utilizar la propiedad CodigoRetorno del objeto DataResponse para obtener el Valor de Retorno enviado desde el SP</remarks>
        /// 
        public async Task<DataResponse<int>> EjecutarNonQuery(string nombreProcedimiento, Dictionary<string, object> parametros, int timeout = 30)
        {
            DataResponse<int> resultado = new DataResponse<int>();

            try
            {
                using (SqlConnection connection = new SqlConnection(this.stringConnection))
                {
                    connection.Open();
                    using (SqlCommand comando = new SqlCommand(nombreProcedimiento, connection))
                    {
                        comando.CommandTimeout = timeout;
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "ValorRetorno",
                            Direction = ParameterDirection.ReturnValue
                        });

                        if (parametros != null)
                        {
                            foreach (KeyValuePair<string, object> parametro in parametros)
                            {
                                comando.Parameters.AddWithValue(parametro.Key, parametro.Value);
                            }
                        }

                        resultado.Valor = await comando.ExecuteNonQueryAsync();
                        resultado.CodigoRetorno = (int)comando.Parameters["ValorRetorno"].Value;
                    }

                    connection.Close();
                }
                return resultado;
            }
            catch (SqlException ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarNonQuery", ex.Message.ToString(), null, null, nombreProcedimiento);
                throw ex;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarNonQuery", ex.Message.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Ejecuta un Procedimiento Almacenado el cual define lista de Parametros tipo SqlParameter 
        /// </summary>
        /// <param name="nombreProcedimiento">Nombre del procedimiento a ejecutar</param>
        /// <param name="parametros">Parametros de entrada</param>
        /// <param name="timeout">Tiempo de espera que debe esperar la ejecución antes genenar un error</param>
        /// <returns>Objeto DataResponse (Integer) en su propiedad Valor contiene el resultado obtenido de la ejecución</returns>
        /// <remarks>Puede utilizar la propiedad CodigoRetorno del objeto DataResponse para obtener el Valor de Retorno enviado desde el SP</remarks>
        /// 
        public async Task<DataResponse<int>> EjecutarNonQuery(string nombreProcedimiento, List<SqlParameter> parametros, int timeout = 30)
        {
            DataResponse<int> resultado = new DataResponse<int>();

            try
            {
                using (SqlConnection connection = new SqlConnection(this.stringConnection))
                {
                    connection.Open();
                    using (SqlCommand comando = new SqlCommand(nombreProcedimiento, connection))
                    {
                        comando.CommandTimeout = timeout;
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "ValorRetorno",
                            Direction = ParameterDirection.ReturnValue
                        });

                        if (parametros != null)
                        {
                            foreach (var parametro in parametros)
                            {
                                comando.Parameters.Add(parametro);
                            }
                        }

                        resultado.Valor = await comando.ExecuteNonQueryAsync();
                        resultado.CodigoRetorno = (int)comando.Parameters["ValorRetorno"].Value;
                    }

                    connection.Close();
                }
                return resultado;
            }
            catch (SqlException ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarNonQuery", ex.Message.ToString(), null, null, nombreProcedimiento);
                throw ex;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarNonQuery", ex.Message.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Ejecuta un Procedimiento Almacenado el cual define lista de Parametros y Salida de Valores Output
        /// </summary>
        /// <param name="nombreProcedimiento">Nombre del procedimiento a ejecutar</param>
        /// <param name="parametros">Parametros de entrada</param>
        /// <param name="timeout">Tiempo de espera que debe esperar la ejecución antes genenar un error</param>
        /// <returns>Objeto DataResponse (Diccionario(NombreParametro,Valor) en su propiedad Valor contiene el resultado obtenido de la ejecución</returns>
        /// <remarks>Puede utilizar la propiedad CodigoRetorno del objeto DataResponse para obtener el Valor de Retorno enviado desde el SP</remarks>
        ///
        public async Task<DataResponse<Dictionary<string, object>>> EjecutarNonQueryOutput(string nombreProcedimiento, List<SqlParameter> parametros, int timeout = 30)
        {
            DataResponse<Dictionary<string, object>> resultado = new DataResponse<Dictionary<string, object>>();

            resultado.Valor = [];

            try
            {
                using (SqlConnection connection = new SqlConnection(this.stringConnection))
                {
                    connection.Open();
                    using (SqlCommand comando = new SqlCommand(nombreProcedimiento, connection))
                    {
                        comando.CommandTimeout = timeout;
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "ValorRetorno",
                            Direction = ParameterDirection.ReturnValue
                        });

                        if (parametros != null)
                        {
                            foreach (var parametro in parametros)
                            {
                                comando.Parameters.Add(parametro);
                            }
                        }

                        await comando.ExecuteReaderAsync();
                        resultado.CodigoRetorno = (int)comando.Parameters["ValorRetorno"].Value;
                        if (parametros != null)
                        {
                            foreach (var parametro in parametros)
                            {
                                if (parametro.Direction == ParameterDirection.Output)
                                {
                                    resultado.Valor.Add(parametro.ParameterName, parametro.Value);
                                }
                            }
                        }

                    }

                    connection.Close();
                }
                return resultado;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region "-- METODOS READER(T) --"
        private void FillDataValue(object clspadre, string key, object valor)
        {
            object obj;
            obj = clspadre;
            string[] arr = key.Split('.');

            for (int i = 0; i <= arr.Length - 1; i++)
            {

                PropertyInfo classProperty = default(PropertyInfo);
                classProperty = obj.GetType().GetProperty(arr[i]);
                object instancia = classProperty.GetValue(obj, null);
                if (i != arr.Length - 1)
                {
                    if (instancia == null)
                    {
                        instancia = classProperty.PropertyType.Assembly.CreateInstance(classProperty.PropertyType.ToString());
                    }
                    classProperty.SetValue(obj, instancia, null);
                    obj = instancia;
                }
                else
                {
                    PropertyInfo instanciaProperty;
                    instanciaProperty = obj.GetType().GetProperty(arr[i]);

                    if (classProperty.PropertyType.IsEnum)
                    {
                        object _objenum = classProperty.PropertyType.Assembly.CreateInstance(classProperty.PropertyType.ToString());
                        foreach (int enumValue in Enum.GetValues(_objenum.GetType()))
                        {
                            if (Enum.GetName(_objenum.GetType(), enumValue) == (string)valor)
                            {
                                valor = enumValue;
                                break;
                            }
                        }
                    }

                    classProperty.SetValue(obj, valor, null);
                }
            }
        }

        /// <summary>
        /// Convierte un reader a un objeto Genérico del tipo T
        /// </summary>
        /// <typeparam name="T">Objeto genérico al cual se va convertir el reader</typeparam>
        /// <param name="clase">Tipo de Objeto que va a devolver</param>
        /// <param name="reader">SqlDataredear con los valors a buscar</param>
        /// <param name="esquema">Diccionario con la relación [Propiedad Clase: Columna reader] </param>
        /// <returns>FillData<T></returns>
        private T FillData<T>(T clase, IDataReader reader, Dictionary<string, string> esquema)
        {

            try
            {
                //'Recorremos el esquema de relación de propiedades de un objeto con su respectiva columna de Bd
                foreach (string key in esquema.Keys)
                {
                    string columna = esquema[key];
                    object dataValue = reader[columna];
                    FillDataValue(clase, key, dataValue);
                }

                return clase;
            }
            catch (SqlException ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", " FillData<T>", ex.Message.ToString(), null, null, null);
                throw ex;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", " FillData<T>", ex.Message.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// Ejecuta un SqlDataReader y devuelve una lista de genérica con los valores del reader 
        /// </summary>
        /// <typeparam name="T">Objeto genérico al cual se va convertir el reader</typeparam>
        /// <param name="entidad">Tipo de Objeto que va a devolver</param>
        /// <param name="nombreProcedimiento">Nombre del Procedimiento Almacenado a ejecutar</param>
        /// <param name="esquema">Diccionario con el esquema de relación [Propiedad de Clase, Columna del reader] </param>
        /// <param name="timeout">Tiempo de espera que debe esperar la ejecución antes genenar un error</param>
        /// <returns>Objeto DataResponse (List(of T)) en su propiedad Valor contiene el resultado obtenido de la ejecución</returns>
        /// <remarks>Puede utilizar la propiedad CodigoRetorno del objeto DataResponse para obtener el Valor de Retorno enviado desde el SP</remarks>
        ///
        public async Task<DataResponse<List<T>>> EjecutarReader<T>(T entidad, string nombreProcedimiento, Dictionary<string, string> esquema, int timeout = 30)
        {
            List<T> items = [];
            DataResponse<List<T>> resultado = new DataResponse<List<T>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(this.stringConnection))
                {
                    connection.Open();
                    using (SqlCommand comando = new SqlCommand(nombreProcedimiento))
                    {
                        comando.Connection = connection;
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.CommandTimeout = timeout;
                        comando.CommandText = nombreProcedimiento;
                        comando.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "ValorRetorno",
                            Direction = ParameterDirection.ReturnValue
                        });

                        using (SqlDataReader reader = await comando.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                T item = (T)entidad.GetType().Assembly.CreateInstance(entidad.GetType().ToString());

                                item = FillData(item, reader, esquema);
                                items.Add(item);
                            }
                            reader.Close();
                        }
                        resultado.CodigoRetorno = (int)comando.Parameters["ValorRetorno"].Value;
                        resultado.Valor = items;
                    }
                    connection.Close();
                }

                return resultado;
            }
            catch (SqlException ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarReader<T>", ex.Message.ToString(), null, null, nombreProcedimiento);
                throw ex;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarReader<T>", ex.Message.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Ejecuta un SqlDataReader y devuelve una lista de genérica con los valores del reader 
        /// </summary>
        /// <typeparam name="T">Objeto genérico al cual se va convertir el reader</typeparam>
        /// <param name="entidad">Tipo de Objeto que va a devolver</param>
        /// <param name="nombreProcedimiento">Nombre del Procedimiento Almacenado a ejecutar</param>
        /// <param name="parametros">Parámetros de entrada para el procedimiento Diccionario[NombreParametro:Valor] </param>
        /// <param name="esquema">Diccionario con el esquema de relación [Propiedad de Clase, Columna del reader] </param>
        /// <param name="timeout">Tiempo de espera que debe esperar la ejecución antes genenar un error</param>
        /// <returns>Objeto DataResponse (List(of T)) en su propiedad Valor contiene el resultado obtenido de la ejecución</returns>
        /// <remarks>Puede utilizar la propiedad CodigoRetorno del objeto DataResponse para obtener el Valor de Retorno enviado desde el SP</remarks>
        public async Task<DataResponse<List<T>>> EjecutarReader<T>(T entidad, string nombreProcedimiento, Dictionary<string, object> parametros, Dictionary<string, string> esquema, int timeout = 30)
        {
            List<T> items = [];
            DataResponse<List<T>> resultado = new DataResponse<List<T>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(this.stringConnection))
                {
                    connection.Open();
                    using (SqlCommand comando = new SqlCommand(nombreProcedimiento))
                    {
                        comando.Connection = connection;
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.CommandTimeout = timeout;
                        comando.CommandText = nombreProcedimiento;
                        comando.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "ValorRetorno",
                            Direction = ParameterDirection.ReturnValue
                        });

                        if (parametros != null)
                        {
                            foreach (KeyValuePair<string, object> parametro in parametros)
                            {
                                comando.Parameters.AddWithValue(parametro.Key, parametro.Value);
                            }
                        }

                        using (SqlDataReader reader = await comando.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                T item = (T)entidad.GetType().Assembly.CreateInstance(entidad.GetType().ToString());

                                item = FillData(item, reader, esquema);
                                items.Add(item);
                            }
                            reader.Close();
                        }

                        resultado.CodigoRetorno = (int)comando.Parameters["ValorRetorno"].Value;
                        resultado.Valor = items;
                    }
                    connection.Close();
                }

                return resultado;
            }
            catch (SqlException ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarReader<T>", ex.Message.ToString(), null, null, nombreProcedimiento);
                throw ex;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarReader<T>", ex.Message.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Ejecuta un SqlDataReader y devuelve una lista de genérica con los valores del reader 
        /// </summary>
        /// <typeparam name="T">Objeto genérico al cual se va convertir el reader</typeparam>
        /// <param name="entidad">Tipo de Objeto que va a devolver</param>
        /// <param name="nombreProcedimiento">Nombre del Procedimiento Almacenado a ejecutar</param>
        /// <param name="parametros">Parámetros de entrada para el procedimiento List(of SqlParameter) </param>
        /// <param name="esquema">Diccionario con el esquema de relación [Propiedad de Clase, Columna del reader] </param>
        /// <param name="timeout">Tiempo de espera que debe esperar la ejecución antes genenar un error</param>
        /// <returns>Objeto DataResponse (List(of T)) en su propiedad Valor contiene el resultado obtenido de la ejecución</returns>
        /// <remarks>Puede utilizar la propiedad CodigoRetorno del objeto DataResponse para obtener el Valor de Retorno enviado desde el SP</remarks>
        public async Task<DataResponse<List<T>>> EjecutarReader<T>(T entidad, string nombreProcedimiento, List<SqlParameter> parametros, Dictionary<string, string> esquema, int timeout = 30)
        {
            List<T> items = [];
            DataResponse<List<T>> resultado = new DataResponse<List<T>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(this.stringConnection))
                {
                    connection.Open();
                    using (SqlCommand comando = new SqlCommand(nombreProcedimiento))
                    {
                        comando.Connection = connection;
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.CommandTimeout = timeout;
                        comando.CommandText = nombreProcedimiento;
                        comando.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "ValorRetorno",
                            Direction = ParameterDirection.ReturnValue
                        });

                        if (parametros != null)
                        {
                            foreach (var parametro in parametros)
                            {
                                comando.Parameters.Add(parametro);
                            }
                        }

                        using (SqlDataReader reader = await comando.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                T item = (T)entidad.GetType().Assembly.CreateInstance(entidad.GetType().ToString());

                                item = FillData(item, reader, esquema);
                                items.Add(item);
                            }
                            reader.Close();
                        }

                        resultado.CodigoRetorno = (int)comando.Parameters["ValorRetorno"].Value;
                        resultado.Valor = items;
                    }
                    connection.Close();
                }

                return resultado;
            }
            catch (SqlException ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarReader<T>", ex.Message.ToString(), null, null, nombreProcedimiento);
                throw ex;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarReader<T>", ex.Message.ToString());
                throw ex;
            }
        }
        #endregion

        #region "-- METODOS READER DICTIONARY --"
        /// <summary>
        /// Ejecuta un SqlDataReader y devuelve una lista de genérica con los valores del reader 
        /// </summary>
        /// <param name="nombreProcedimiento">Nombre del Procedimiento Almacenado a ejecutar</param>
        /// <param name="esquema">Lista con las columnas del datareader a obtener </param>
        /// <param name="timeout">Tiempo de espera que debe esperar la ejecución antes genenar un error</param>
        /// <returns>Objeto DataResponse (Dictionario[Columna:Valor]) en su propiedad Valor contiene el resultado obtenido de la ejecución</returns>
        /// <remarks>Puede utilizar la propiedad CodigoRetorno del objeto DataResponse para obtener el Valor de Retorno enviado desde el SP</remarks>
        public async Task<DataResponse<Dictionary<string, object>>> EjecutarReaderDictionary(string nombreProcedimiento, List<string> esquema, int timeout = 30)
        {
            Dictionary<string, object> items = [];
            DataResponse<Dictionary<string, object>> resultado = new DataResponse<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(this.stringConnection))
                {
                    connection.Open();
                    using (SqlCommand comando = new SqlCommand(nombreProcedimiento))
                    {
                        comando.Connection = connection;
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.CommandTimeout = timeout;
                        comando.CommandText = nombreProcedimiento;
                        comando.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "ValorRetorno",
                            Direction = ParameterDirection.ReturnValue
                        });


                        using (SqlDataReader reader = await comando.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                foreach (var item in esquema)
                                {
                                    items.Add(item, reader[item]);
                                }
                            }
                            reader.Close();
                        }

                        resultado.CodigoRetorno = (int)comando.Parameters["ValorRetorno"].Value;
                        resultado.Valor = items;

                    }
                    connection.Close();
                }

                return resultado;
            }
            catch (SqlException ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarReaderDictionary", ex.Message.ToString(), null, null, nombreProcedimiento);
                throw ex;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarReaderDictionary", ex.Message.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Ejecuta un SqlDataReader y devuelve una lista de genérica con los valores del reader 
        /// </summary>
        /// <param name="nombreProcedimiento">Nombre del Procedimiento Almacenado a ejecutar</param>
        /// <param name="parametros">Parametros de entrada para el proceimiento. Diccionario[NombreParametro:Valor] </param>
        /// <param name="esquema">Lista con las columnas del datareader a obtener </param>
        /// <param name="timeout">Tiempo de espera que debe esperar la ejecución antes genenar un error</param>
        /// <returns>Objeto DataResponse (Dictionario[Columna:Valor]) en su propiedad Valor contiene el resultado obtenido de la ejecución</returns>
        /// <remarks>Puede utilizar la propiedad CodigoRetorno del objeto DataResponse para obtener el Valor de Retorno enviado desde el SP</remarks>
        public async Task<DataResponse<Dictionary<string, object>>> EjecutarReaderDictionary(string nombreProcedimiento, Dictionary<string, object> parametros, List<string> esquema, int timeout = 30)
        {
            Dictionary<string, object> items = [];
            DataResponse<Dictionary<string, object>> resultado = new DataResponse<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(this.stringConnection))
                {
                    connection.Open();
                    using (SqlCommand comando = new SqlCommand(nombreProcedimiento))
                    {
                        comando.Connection = connection;
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.CommandTimeout = timeout;
                        comando.CommandText = nombreProcedimiento;
                        comando.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "ValorRetorno",
                            Direction = ParameterDirection.ReturnValue
                        });


                        if (parametros != null)
                        {
                            foreach (KeyValuePair<string, object> parametro in parametros)
                            {
                                comando.Parameters.AddWithValue(parametro.Key, parametro.Value);
                            }
                        }

                        using (SqlDataReader reader = await comando.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                foreach (var item in esquema)
                                {
                                    items.Add(item, reader[item]);
                                }
                            }
                            reader.Close();
                        }

                        resultado.CodigoRetorno = (int)comando.Parameters["ValorRetorno"].Value;
                        resultado.Valor = items;
                    }
                    connection.Close();
                }

                return resultado;
            }
            catch (SqlException ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarReaderDictionary", ex.Message.ToString(), null, null, nombreProcedimiento);
                throw ex;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarReaderDictionary", ex.Message.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Ejecuta un SqlDataReader y devuelve una lista de genérica con los valores del reader 
        /// </summary>
        /// <param name="nombreProcedimiento">Nombre del Procedimiento Almacenado a ejecutar</param>
        /// <param name="parametros">Parametros de entrada para el proceimiento. Diccionario[NombreParametro:Valor] </param>
        /// <param name="esquema">Lista con las columnas del datareader a obtener </param>
        /// <param name="timeout">Tiempo de espera que debe esperar la ejecución antes genenar un error</param>
        /// <returns>Objeto DataResponse (Dictionario[Columna:Valor]) en su propiedad Valor contiene el resultado obtenido de la ejecución</returns>
        /// <remarks>Puede utilizar la propiedad CodigoRetorno del objeto DataResponse para obtener el Valor de Retorno enviado desde el SP</remarks>
        public async Task<DataResponse<Dictionary<string, object>>> EjecutarReaderDictionary(string nombreProcedimiento, List<SqlParameter> parametros, List<string> esquema, int timeout = 30)
        {
            Dictionary<string, object> items = [];
            DataResponse<Dictionary<string, object>> resultado = new DataResponse<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(this.stringConnection))
                {
                    connection.Open();
                    using (SqlCommand comando = new SqlCommand(nombreProcedimiento))
                    {
                        comando.Connection = connection;
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.CommandTimeout = timeout;
                        comando.CommandText = nombreProcedimiento;
                        comando.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "ValorRetorno",
                            Direction = ParameterDirection.ReturnValue
                        });


                        if (parametros != null)
                        {
                            foreach (var parametro in parametros)
                            {
                                comando.Parameters.Add(parametro);
                            }
                        }

                        using (SqlDataReader reader = await comando.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                foreach (var item in esquema)
                                {
                                    items.Add(item, reader[item]);
                                }
                            }
                            reader.Close();
                        }

                        resultado.CodigoRetorno = (int)comando.Parameters["ValorRetorno"].Value;
                        resultado.Valor = items;
                    }
                    connection.Close();
                }

                return resultado;
            }
            catch (SqlException ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarReaderDictionary", ex.Message.ToString(), null, null, nombreProcedimiento);
                throw ex;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarReaderDictionary", ex.Message.ToString());
                throw ex;
            }
        }
        #endregion

        #region "-- METODOS XMLREADER --"
        /// <summary>
        /// Ejecuta un SqlDataReader y devuelve un Xelement con los valores del reader 
        /// </summary>
        /// <param name="nombreProcedimiento">Nombre del Procedimiento Almacenado a ejecutar</param>
        /// <param name="timeout">Tiempo de espera que debe esperar la ejecución antes genenar un error</param>
        /// <returns>Objeto DataResponse (Xelement) en su propiedad Valor contiene el resultado obtenido de la ejecución</returns>
        /// <remarks>Puede utilizar la propiedad CodigoRetorno del objeto DataResponse para obtener el Valor de Retorno enviado desde el SP</remarks>
        ///
        public async Task<DataResponse<XElement>> EjecutarXmlReader(string nombreProcedimiento, int timeout = 30)
        {
            XElement items = default(XElement);
            DataResponse<XElement> resultado = new DataResponse<XElement>();

            try
            {
                using (SqlConnection connection = new SqlConnection(this.stringConnection))
                {
                    connection.Open();
                    using (SqlCommand comando = new SqlCommand(nombreProcedimiento))
                    {
                        comando.Connection = connection;
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.CommandTimeout = timeout;
                        comando.CommandText = nombreProcedimiento;
                        comando.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "ValorRetorno",
                            Direction = ParameterDirection.ReturnValue
                        });

                        using (XmlReader reader = await comando.ExecuteXmlReaderAsync())
                        {
                            items = XElement.Load(reader);
                            reader.Close();
                        }

                        resultado.CodigoRetorno = (int)comando.Parameters["ValorRetorno"].Value;
                        resultado.Valor = items;
                    }
                    connection.Close();
                }

                return resultado;
            }
            catch (SqlException ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarXmlReader", ex.Message.ToString(), null, null, nombreProcedimiento);
                throw ex;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarXmlReader", ex.Message.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Ejecuta un SqlDataReader y devuelve un Xelement con los valores del reader 
        /// </summary>
        /// <param name="nombreProcedimiento">Nombre del Procedimiento Almacenado a ejecutar</param>
        /// <param name="parametros">Parametros de entrada para el proceimiento. Diccionario[NombreParametro:Valor] </param>
        /// <param name="timeout">Tiempo de espera que debe esperar la ejecución antes genenar un error</param>
        /// <returns>Objeto DataResponse (Xelement) en su propiedad Valor contiene el resultado obtenido de la ejecución</returns>
        /// <remarks>Puede utilizar la propiedad CodigoRetorno del objeto DataResponse para obtener el Valor de Retorno enviado desde el SP</remarks>
        public async Task<DataResponse<XElement>> EjecutarXmlReader(string nombreProcedimiento, Dictionary<string, object> parametros, int timeout = 30)
        {
            XElement items = default(XElement);
            DataResponse<XElement> resultado = new DataResponse<XElement>();

            try
            {
                using (SqlConnection connection = new SqlConnection(this.stringConnection))
                {
                    connection.Open();
                    using (SqlCommand comando = new SqlCommand(nombreProcedimiento))
                    {
                        comando.Connection = connection;
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.CommandTimeout = timeout;
                        comando.CommandText = nombreProcedimiento;
                        comando.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "ValorRetorno",
                            Direction = ParameterDirection.ReturnValue
                        });

                        if (parametros != null)
                        {
                            foreach (KeyValuePair<string, object> parametro in parametros)
                            {
                                comando.Parameters.AddWithValue(parametro.Key, parametro.Value);
                            }
                        }

                        using (XmlReader reader = await comando.ExecuteXmlReaderAsync())
                        {
                            items = XElement.Load(reader);
                            reader.Close();
                        }

                        resultado.CodigoRetorno = (int)comando.Parameters["ValorRetorno"].Value;
                        resultado.Valor = items;
                    }
                    connection.Close();
                }

                return resultado;
            }
            catch (SqlException ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarXmlReader", ex.Message.ToString(), null, null, nombreProcedimiento);
                throw ex;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarXmlReader", ex.Message.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Ejecuta un SqlDataReader y devuelve un Xelement con los valores del reader 
        /// </summary>
        /// <param name="nombreProcedimiento">Nombre del Procedimiento Almacenado a ejecutar</param>
        /// <param name="parametros">Parametros de entrada para el proceimiento. List(of Sqlparameter) </param>
        /// <param name="timeout">Tiempo de espera que debe esperar la ejecución antes genenar un error</param>
        /// <returns>Objeto DataResponse (Xelement) en su propiedad Valor contiene el resultado obtenido de la ejecución</returns>
        /// <remarks>Puede utilizar la propiedad CodigoRetorno del objeto DataResponse para obtener el Valor de Retorno enviado desde el SP</remarks>
        ///
        public async Task<DataResponse<XElement>> EjecutarXmlReader(string nombreProcedimiento, List<SqlParameter> parametros, int timeout = 30)
        {
            XElement items = default(XElement);
            DataResponse<XElement> resultado = new DataResponse<XElement>();

            try
            {
                using (SqlConnection connection = new SqlConnection(this.stringConnection))
                {
                    connection.Open();
                    using (SqlCommand comando = new SqlCommand(nombreProcedimiento))
                    {
                        comando.Connection = connection;
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.CommandTimeout = timeout;
                        comando.CommandText = nombreProcedimiento;
                        comando.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "ValorRetorno",
                            Direction = ParameterDirection.ReturnValue
                        });

                        if (parametros != null)
                        {
                            foreach (var parametro in parametros)
                            {
                                comando.Parameters.Add(parametro);
                            }
                        }

                        using (XmlReader reader = await comando.ExecuteXmlReaderAsync())
                        {
                            items = XElement.Load(reader);
                            reader.Close();
                        }

                        resultado.CodigoRetorno = (int)comando.Parameters["ValorRetorno"].Value;
                        resultado.Valor = items;
                    }
                    connection.Close();
                }

                return resultado;
            }
            catch (SqlException ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarXmlReader", ex.Message.ToString(), null, null, nombreProcedimiento);
                throw ex;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarXmlReader", ex.Message.ToString());
                throw ex;
            }
        }
        #endregion

        #region "-- METODOS SCALARREADER --"
        /// <summary>
        /// Ejecuta un Procedimiento Almacenado y devuelve un valor de tipo Object
        /// </summary>
        /// <param name="nombreProcedimiento">Nombre del Procedimiento Almacenado a ejecutar</param>
        /// <param name="timeout">Tiempo de espera que debe esperar la ejecución antes genenar un error</param>
        /// <returns>Objeto DataResponse (Object) en su propiedad Valor contiene el resultado obtenido de la ejecución</returns>
        /// <remarks>Puede utilizar la propiedad CodigoRetorno del objeto DataResponse para obtener el Valor de Retorno enviado desde el SP</remarks>
        ///
        public async Task<DataResponse<object>> EjecutarScalarReader(string nombreProcedimiento, int timeout = 30)
        {
            object items = new object();
            DataResponse<object> resultado = new DataResponse<object>();

            try
            {
                using (SqlConnection connection = new SqlConnection(this.stringConnection))
                {
                    connection.Open();
                    using (SqlCommand comando = new SqlCommand(nombreProcedimiento))
                    {
                        comando.Connection = connection;
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.CommandTimeout = timeout;
                        comando.CommandText = nombreProcedimiento;
                        comando.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "ValorRetorno",
                            Direction = ParameterDirection.ReturnValue
                        });

                        using (SqlDataReader reader = await comando.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                items = reader[0];
                            }
                            reader.Close();
                        }

                        resultado.CodigoRetorno = (int)comando.Parameters["ValorRetorno"].Value;
                        resultado.Valor = items;
                    }
                    connection.Close();
                }

                return resultado;
            }
            catch (SqlException ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarScalarReader", ex.Message.ToString(), null, null, nombreProcedimiento);
                throw ex;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarScalarReader", ex.Message.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Ejecuta un Procedimiento Almacenado y devuelve un valor de tipo Object
        /// </summary>
        /// <param name="nombreProcedimiento">Nombre del Procedimiento Almacenado a ejecutar</param>
        /// <param name="parametros">Parametros de entrada para el proceimiento. Diccionario[NombreParametro:Valor] </param>
        /// <param name="timeout">Tiempo de espera que debe esperar la ejecución antes genenar un error</param>
        /// <returns>Objeto DataResponse (Object) en su propiedad Valor contiene el resultado obtenido de la ejecución</returns>
        /// <remarks>Puede utilizar la propiedad CodigoRetorno del objeto DataResponse para obtener el Valor de Retorno enviado desde el SP</remarks>
        ///
        public async Task<DataResponse<object>> EjecutarScalarReader(string nombreProcedimiento, Dictionary<string, object> parametros, int timeout = 30)
        {
            object items = new object();
            DataResponse<object> resultado = new DataResponse<object>();

            try
            {
                using (SqlConnection connection = new SqlConnection(this.stringConnection))
                {
                    connection.Open();
                    using (SqlCommand comando = new SqlCommand(nombreProcedimiento))
                    {
                        comando.Connection = connection;
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.CommandTimeout = timeout;
                        comando.CommandText = nombreProcedimiento;
                        comando.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "ValorRetorno",
                            Direction = ParameterDirection.ReturnValue
                        });

                        if (parametros != null)
                        {
                            foreach (KeyValuePair<string, object> parametro in parametros)
                            {
                                comando.Parameters.AddWithValue(parametro.Key, parametro.Value);
                            }
                        }


                        using (SqlDataReader reader = await comando.ExecuteReaderAsync())
                        {

                            if (reader.Read())
                            {
                                items = reader[0];
                            }

                            reader.Close();
                        }

                        resultado.CodigoRetorno = (int)comando.Parameters["ValorRetorno"].Value;
                        resultado.Valor = items;
                    }
                    connection.Close();
                }

                return resultado;
            }
            catch (SqlException ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarScalarReader", ex.Message.ToString(), null, null, nombreProcedimiento);
                throw ex;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarScalarReader", ex.Message.ToString());
                throw ex;
            }
        }

        public async Task<DataResponse<object>> EjecutarScalar(string nombreProcedimiento, Dictionary<string, object> parametros, int timeout = 30)
        {
            object items = new object();
            DataResponse<object> resultado = new DataResponse<object>();

            try
            {
                using (SqlConnection connection = new SqlConnection(this.stringConnection))
                {
                    connection.Open();
                    using (SqlCommand comando = new SqlCommand(nombreProcedimiento))
                    {
                        comando.Connection = connection;
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.CommandTimeout = timeout;
                        comando.CommandText = nombreProcedimiento;
                        comando.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "ValorRetorno",
                            Direction = ParameterDirection.ReturnValue
                        });

                        if (parametros != null)
                        {
                            foreach (KeyValuePair<string, object> parametro in parametros)
                            {
                                comando.Parameters.AddWithValue(parametro.Key, parametro.Value);
                            }
                        }

                        using (SqlDataReader reader = await comando.ExecuteReaderAsync())
                        {

                            if (reader.Read())
                            {
                                items = reader[0];

                            }

                            reader.Close();
                        }

                        resultado.CodigoRetorno = (int)comando.Parameters["ValorRetorno"].Value;
                        resultado.Valor = items;
                    }
                    connection.Close();
                }

                return resultado;
            }
            catch (SqlException ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarScalar", ex.Message.ToString(), null, null, nombreProcedimiento);
                throw ex;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "lDato", "EjecutarScalar", ex.Message.ToString());
                throw ex;
            }
        }
        #endregion

    }

}
