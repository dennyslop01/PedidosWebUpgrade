using PedidosWebUpgrade.Application.Common.Interfaces;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.DbContext;
using PedidosWebUpgrade.Infrastructure.Utilities;

namespace PedidosWebUpgrade.Infrastructure.Repository
{
    public class ListaPreciosRepository: IListaPreciosRepository
    {
        private readonly ConfigVariables _configVariables;

        public ListaPreciosRepository(ConfigVariables configVariables)
        {
            _configVariables = configVariables;
        }

        public async Task<List<F45520>> ConsultarCabeceraListoPrecio()
        {
            lDato _ldato = new lDato(_configVariables);
            List<F45520> Lista = new List<F45520>();
            DataResponse<List<F45520>> List_Response = new DataResponse<List<F45520>>();
            try
            {
                _ldato.Esquema.Add("Codigo", "codido");
                _ldato.Esquema.Add("Descripcion", "descripcion");
                _ldato.Esquema.Add("Phdoco", "phdoco");
                _ldato.Esquema.Add("Phdcto", "phdcto");
                _ldato.Esquema.Add("Phco", "phco");
                _ldato.Esquema.Add("Phprclst", "phprclst");
                _ldato.Esquema.Add("Phpldesc", "phpldesc");
                _ldato.Esquema.Add("Pheftj", "pheftj");
                _ldato.Esquema.Add("Phcrcd", "phcrcd");
                List_Response = await _ldato.EjecutarReader(new F45520(), "PED_USP_CONSULTARCABECERALISTAPRECIO", _ldato.Parametros, _ldato.Esquema);
                Lista = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ListaPreciosRepository", "ConsultarCabeceraListoPrecio()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARCABECERALISTAPRECIO");
                throw new ApplicationException("ERROR EN: ConsultarCabeceraListoPrecio()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return Lista;
        }

        public async Task<List<F45521>> ConsultarDetalleListaPrecio(string phdoco, string phdcto)
        {
            lDato _ldato = new lDato(_configVariables);
            List<F45521> Lista = new List<F45521>();
            DataResponse<List<F45521>> List_Response = new DataResponse<List<F45521>>();
            try
            {
                _ldato.Parametros.Add("@PHDOCO", string.IsNullOrWhiteSpace(phdoco) ? Convert.DBNull : phdoco);
                _ldato.Parametros.Add("@PHDCTO", string.IsNullOrWhiteSpace(phdcto) ? Convert.DBNull : phdcto);

                _ldato.Esquema.Add("Pdlitm", "pdlitm");
                _ldato.Esquema.Add("Descripcion", "descripcion");
                _ldato.Esquema.Add("Pduom", "pduom");
                _ldato.Esquema.Add("Pduprc", "pduprc");
                _ldato.Esquema.Add("Pdplcamt2", "pdplcamt2");
                _ldato.Esquema.Add("Pdplcamt3", "pdplcamt3");
                List_Response = await _ldato.EjecutarReader(new F45521(), "PED_USP_CONSULTARDETALLELISTAPRECIO", _ldato.Parametros, _ldato.Esquema);
                Lista = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ListaPreciosRepository", "ConsultarCabeceraListoPrecio()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARDETALLELISTAPRECIO");
                throw new ApplicationException("ERROR EN: ConsultarDetalleListaPrecio()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return Lista;
        }

        public async Task<List<ListPreciosPrint>> ConsultarDetallesReporteListaPrecios(string doco, string dcto, int IdUsuario)
        {
            lDato _ldato = new lDato(_configVariables);
            List<ListPreciosPrint> Lista = new List<ListPreciosPrint>();
            DataResponse<List<ListPreciosPrint>> List_Response = new DataResponse<List<ListPreciosPrint>>();
            try
            {
                _ldato.Parametros.Add("@DOCO ", string.IsNullOrWhiteSpace(doco) ? Convert.DBNull : doco);
                _ldato.Parametros.Add("@DCTO", string.IsNullOrWhiteSpace(dcto) ? Convert.DBNull : dcto);
                _ldato.Parametros.Add("@USERID", IdUsuario);

                _ldato.Esquema.Add("Heder_Line1", "header_line_1");
                _ldato.Esquema.Add("Header_Line2", "header_line_2");
                _ldato.Esquema.Add("Header_Line3", "header_line_3");
                _ldato.Esquema.Add("Clasificacion", "clasificacion");
                _ldato.Esquema.Add("ProductId", "product_id");
                _ldato.Esquema.Add("Product_Brand_Line1", "product_brand_line_1");
                _ldato.Esquema.Add("Product_Brand_Line2", "product_brand_line_2");
                _ldato.Esquema.Add("Product_Brand_Line3", "product_brand_line_3");
                _ldato.Esquema.Add("CasesOf_Line1", "casesof_line_1");
                _ldato.Esquema.Add("CasesOf_Line2", "casesof_line_2");
                _ldato.Esquema.Add("CasesOf_Line3", "casesof_line_3");
                _ldato.Esquema.Add("Price", "price");
                _ldato.Esquema.Add("Price_Header", "price_header");
                _ldato.Esquema.Add("Note1", "note_1");
                _ldato.Esquema.Add("Note2", "note_2");
                _ldato.Esquema.Add("Note3", "note_3");
                _ldato.Esquema.Add("Note4", "note_4");
                _ldato.Esquema.Add("Note5", "note_5");
                _ldato.Esquema.Add("Contact_1", "contact_1");
                _ldato.Esquema.Add("Contact_2", "contact_2");
                _ldato.Esquema.Add("Contact_3", "contact_3");
                _ldato.Esquema.Add("Contact_4", "contact_4");
                _ldato.Esquema.Add("List_Id", "list_id");

                List_Response = await _ldato.EjecutarReader(new ListPreciosPrint(), "PED_USP_IMPRIMIRLISTAPRECIO", _ldato.Parametros, _ldato.Esquema);
                Lista = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ListaPreciosRepository", "ConsultarDetallesReporteListaPrecios()", ex.Message.ToString(), null, null, "PED_USP_IMPRIMIRLISTAPRECIO");
                throw new ApplicationException("ERROR EN: ConsultarDetallesReporteListaPrecios()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return Lista;
        }
    }
}
