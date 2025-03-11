using Microsoft.Extensions.Options;
using PedidosWebUpgrade.Infrastructure.Utilities;

namespace PedidosWebUpgrade.Infrastructure.DbContext
{
    public class lBase : IDisposable
    {
        private readonly ConfigVariables _configVariables;


        #region "-- ATRIBUTOS --"
        private bool disposedValue = false;
        protected string? stringConnection = "";
        #endregion

        #region "-- CONSTRUCTORES --"
        public lBase(ConfigVariables configVariables)
        {
            _configVariables = configVariables;
            stringConnection = _configVariables.ConnectionString;

            //if (ConfigurationManager.ConnectionStrings.Count > 0)
            //{
            //    stringConnection = ConfigurationManager.ConnectionStrings["Principal"].ConnectionString;
            //}
        }

        public lBase(int timeout)
        {
            ConnectionTimeout = timeout;

            //if (ConfigurationManager.ConnectionStrings.Count > 0)
            //{
            //    this.stringConnection = ConfigurationManager.ConnectionStrings["Principal"].ConnectionString;
            //}
        }

        public lBase(string stringConnection)
        {
            //if (ConfigurationManager.ConnectionStrings.Count > 0)
            //{
            //    this.stringConnection = ConfigurationManager.ConnectionStrings[stringConnection].ConnectionString;
            //}

            if (string.IsNullOrEmpty(this.stringConnection))
            {
                this.stringConnection = stringConnection;
            }

        }

        public lBase(string stringConnection, int timeout)
        {
            ConnectionTimeout = timeout;

            //if (ConfigurationManager.ConnectionStrings.Count > 0)
            //{
            //    this.stringConnection = ConfigurationManager.ConnectionStrings[stringConnection].ConnectionString;
            //}

            if (string.IsNullOrEmpty(this.stringConnection))
            {
                this.stringConnection = stringConnection;
            }

        }
        #endregion

        #region "-- PROPIEDADES --"
        public int ConnectionTimeout { get; set; }
        #endregion

        #region "-- METODOS --"
        public string ObtenerStringConnection()
        {
            return stringConnection;
        }
        #endregion

        #region "-- IDisposable SUPPORT --"
        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: free other state (managed objects).
                }

                // TODO: free your own state (unmanaged objects).
                // TODO: set large fields to null.
            }
            this.disposedValue = true;
        }

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
