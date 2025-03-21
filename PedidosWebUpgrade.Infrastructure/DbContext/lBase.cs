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
        }
        #endregion

        #region "-- PROPIEDADES --"
        public int ConnectionTimeout { get; set; }
        #endregion

        #region "-- METODOS --"
        public async Task<string> ObtenerStringConnection()
        {
            return stringConnection;
        }
        #endregion

        #region "-- IDisposable SUPPORT --"
        // IDisposable
        protected virtual async void Dispose(bool disposing)
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
        public async void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
