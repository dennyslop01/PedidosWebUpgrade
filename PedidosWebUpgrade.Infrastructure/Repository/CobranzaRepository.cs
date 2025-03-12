using PedidosWebUpgrade.Application.Common.Interfaces;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.DbContext;
using PedidosWebUpgrade.Infrastructure.Utilities;

namespace PedidosWebUpgrade.Infrastructure.Repository
{
    public class CobranzaRepository: ICobranzaRepository
    {
        private readonly ConfigVariables _configVariables;

        public CobranzaRepository(ConfigVariables configVariables)
        {
            _configVariables = configVariables;
        }

        public List<EstadoCuenta> ConsultarEstadoCuenta(string CustomerId)
        {
            lDato _ldato = new lDato(_configVariables);
            List<EstadoCuenta> _Movimientos = [];
            DataResponse<List<EstadoCuenta>> List_Response = new DataResponse<List<EstadoCuenta>>();
            try
            {
                _ldato.Parametros.Add("@CUSTOMERID", (CustomerId == "" ? (object)DBNull.Value : CustomerId));
                _ldato.Esquema.Add("Id", "ID");
                _ldato.Esquema.Add("CustomerId", "CUSTOMER_ID");
                _ldato.Esquema.Add("NumeroDocumento", "DOC_NUMBER");
                _ldato.Esquema.Add("TipoDocumento", "DOC_TYPE");
                _ldato.Esquema.Add("Descripcion", "LEYENDA");
                _ldato.Esquema.Add("MontoPendiente", "INVOICE_BALANCE");
                _ldato.Esquema.Add("MontoDocumento", "INVOICE_AMOUNT");
                _ldato.Esquema.Add("FechaDocumento", "ISSUE_DATE");
                _ldato.Esquema.Add("DiasVencimiento", "EXP_DAYS");
                _ldato.Esquema.Add("TipoDocOriginal", "DOC_TYPE_ORIGINAL");
                _ldato.Esquema.Add("NroDocOriginal", "DOC_NUMBER_ORIGINAL");

                List_Response = _ldato.EjecutarReader(new EstadoCuenta(), "COB_USP_CONSULTARESTADOCTA", _ldato.Parametros, _ldato.Esquema);
                _Movimientos = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"CobranzaRepository", "ConsultarEstadoCuenta()", ex.Message.ToString(), null, null, "COB_USP_CONSULTARESTADOCTA");
                throw new ApplicationException("ERROR EN: ConsultarEstadoCuenta()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Movimientos;
        }

        public List<ListaGeneral> ConsultarTipoPago()
        {
            lDato _ldato = new lDato(_configVariables);
            List<ListaGeneral> _TipoPagos = [];
            DataResponse<List<ListaGeneral>> List_Response = new DataResponse<List<ListaGeneral>>();
            try
            {
                _ldato.Esquema.Add("IdTipo", "Id");
                _ldato.Esquema.Add("Descripcion", "tipo_pago");
                List_Response = _ldato.EjecutarReader(new ListaGeneral(), "COB_USP_CONSULTARTIPOPAGO", _ldato.Parametros, _ldato.Esquema);
                _TipoPagos = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"CobranzaRepository", "ConsultarTipoPago()", ex.Message.ToString(), null, null, "COB_USP_CONSULTARTIPOPAGO");
                throw new ApplicationException("ERROR EN: ConsultarTipoPago()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _TipoPagos;
        }

        public List<ListaGeneral> ConsultarMonedas()
        {
            lDato _ldato = new lDato(_configVariables);
            List<ListaGeneral> _Monedas = [];
            DataResponse<List<ListaGeneral>> List_Response = new DataResponse<List<ListaGeneral>>();
            try
            {
                _ldato.Esquema.Add("Codigo", "cod_moneda");
                _ldato.Esquema.Add("Descripcion", "cod_moneda");
                List_Response = _ldato.EjecutarReader(new ListaGeneral(), "COB_USP_CONSULTARMONEDA", _ldato.Parametros, _ldato.Esquema);
                _Monedas = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"CobranzaRepository", "ConsultarMonedas()", ex.Message.ToString(), null, null, "COB_USP_CONSULTARMONEDA");
                throw new ApplicationException("ERROR EN: ConsultarMonedas()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Monedas;
        }

        public List<ListaGeneral> ConsultarBancos()
        {
            lDato _ldato = new lDato(_configVariables);
            List<ListaGeneral> _Bancos = [];
            DataResponse<List<ListaGeneral>> List_Response = new DataResponse<List<ListaGeneral>>();
            try
            {
                _ldato.Esquema.Add("Codigo", "Codigo");
                _ldato.Esquema.Add("Descripcion", "descripcion");
                List_Response = _ldato.EjecutarReader(new ListaGeneral(), "COB_USP_CONSULTARBANCOS", _ldato.Parametros, _ldato.Esquema);
                _Bancos = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"CobranzaRepository", "ConsultarBancos()", ex.Message.ToString(), null, null, "COB_USP_CONSULTARBANCOS");
                throw new ApplicationException("ERROR EN: ConsultarBancos()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Bancos;
        }

        public List<TipoCambio> ConsultarTipoCambio(string moneda)
        {
            lDato _ldato = new lDato(_configVariables);
            List<TipoCambio> _TipoCambio = [];
            DataResponse<List<TipoCambio>> List_Response = new DataResponse<List<TipoCambio>>();
            try
            {
                _ldato.Parametros.Add("@CODMONEDA", (moneda == "" ? (object)DBNull.Value : moneda));
                _ldato.Esquema.Add("IdTipoCambio", "id");
                _ldato.Esquema.Add("MonedaOrigen", "cod_moneda_origen");
                _ldato.Esquema.Add("MonedaDestino", "cod_moneda_destino");
                _ldato.Esquema.Add("TasaCambio", "tasa_cambio");
                _ldato.Esquema.Add("Fecha", "fecha");
                List_Response = _ldato.EjecutarReader(new TipoCambio(), "COB_USP_CONSULTARTIPOCAMBIO", _ldato.Parametros, _ldato.Esquema);
                _TipoCambio = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"CobranzaRepository", "ConsultarTipoCambio()", ex.Message.ToString(), null, null, "COB_USP_CONSULTARTIPOCAMBIO");
                throw new ApplicationException("ERROR EN: ConsultarTipoCambio()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _TipoCambio;
        }
    }

}
