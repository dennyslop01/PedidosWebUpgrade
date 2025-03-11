using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PedidosWebUpgrade.Infrastructure.Utilities
{
    public class CustomUtility
    {
        #region -- MÉTODOS PÚBLICOS --
        /// <summary>
        /// Registra en un archivo TXT los errores arrojados en la aplicación
        /// </summary>
        /// <param name="DirectorioLog">string</param>
        /// <param name="sClase">string</param>
        /// <param name="sMetodo">string</param>
        /// <param name="sMensaje">string</param>
        /// <param name="sNavegador">string</param>
        /// <param name="sVersion">string</param>
        /// <param name="sProcedimiento">string</param>
        public static void RegistrarExcepcion(string DirectorioLog, string sClase, string sMetodo, string sMensaje, string sNavegador = null, string sVersion = null, string sProcedimiento = null, string v = null)
        {
            DirectorioLog = System.IO.Directory.GetCurrentDirectory() + DirectorioLog;
            string str = string.Empty;

            if (!File.Exists(DirectorioLog))
            {
                File.Create(DirectorioLog).Close();
            }
            else
            {
                FileInfo fInfo = new FileInfo(DirectorioLog);
                if (fInfo.Length > 10485760)
                {
                    fInfo.MoveTo(DirectorioLog + "_" + DateTime.Now.Day.ToString("D2") + DateTime.Now.Month.ToString("D2") +
                                                       DateTime.Now.Year.ToString("D4") + DateTime.Now.Hour.ToString("D2") +
                                                       DateTime.Now.Minute.ToString("D2") + DateTime.Now.Second.ToString("D2"));
                    File.Delete(DirectorioLog);
                }
                else
                {
                    using (StreamReader sreader = new StreamReader(DirectorioLog))
                    {
                        str = sreader.ReadToEnd();
                    }
                }
            }

            using (StreamWriter writer = new StreamWriter(DirectorioLog, true))
            {
                writer.WriteLine("Fecha: " + DateTime.Now);
                writer.WriteLine("Clase: " + sClase);
                writer.WriteLine("Método: " + sMetodo);
                if (sNavegador != null && sVersion != null)
                {
                    writer.WriteLine("Navegador: " + sNavegador);
                    writer.WriteLine("Version: " + sVersion);
                }
                if (sProcedimiento != null)
                {
                    writer.WriteLine("Procedimiento: " + sProcedimiento);
                }
                writer.WriteLine("ERROR: " + sMensaje);
                writer.WriteLine(Environment.NewLine + "===========================================================================================================================" + Environment.NewLine);
                writer.WriteLine(str);
                writer.Flush();
                writer.Close();
            }
        }


        /// <summary>
        /// Formatea un numero al formato 123456.67
        /// </summary>
        /// <param name="sNumero">string</param>
        public static string FormaterNumero(string sNumero)
        {
            String formattedNumber = "0";
            NumberStyles style;
            CultureInfo culture;
            double number;

            if (sNumero.Trim() == "")
            {
                sNumero = "0";
            }

            style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
            culture = CultureInfo.CreateSpecificCulture("es-Es");
            if (Double.TryParse(sNumero, style, culture, out number))
            {
                formattedNumber = number.ToString().Replace(",", ".");
            }
            else
            {
                culture = CultureInfo.CreateSpecificCulture("en-En");
                if (Double.TryParse(sNumero, style, culture, out number))
                {
                    formattedNumber = number.ToString().Replace(",", ".");
                }
            }
            return formattedNumber;
        }
    }
    #endregion
}

