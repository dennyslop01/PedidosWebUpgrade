using Microsoft.IdentityModel.Tokens;
using PedidosWebUpgrade.Application.Common.Interfaces;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.DbContext;
using PedidosWebUpgrade.Infrastructure.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedidosWebUpgrade.Infrastructure.Repository
{
    public class EmpresaRepository: IEmpresaRepository
    {
        private readonly ConfigVariables _configVariables;

        public EmpresaRepository(ConfigVariables configVariables)
        {
            _configVariables = configVariables;
        }
        
        /// <summary>
        /// LISTADO DE SUCURSALES DE LA EMPRESA
        /// </summary>
        /// <returns>List<ListaGeneral></returns>
        public List<ListaGeneral> ObtenerSucursales(string salesmanid)
        {
            lDato _ldato = new lDato(_configVariables);
            List<ListaGeneral> _Sucursales = [];
            DataResponse<List<ListaGeneral>> List_Response = new DataResponse<List<ListaGeneral>>();
            try
            {
                _ldato.Parametros.Add("@SALESMANID", salesmanid);
                _ldato.Esquema.Add("Codigo", "ID");
                _ldato.Esquema.Add("Descripcion", "DESCRIPCION");
                List_Response = _ldato.EjecutarReader(new ListaGeneral(), "PED_USP_CONSULTARSUCURSAL", _ldato.Parametros, _ldato.Esquema);
                _Sucursales = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"EmpresaRepository", "ObtenerSucursales()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARSUCURSAL");
                throw new ApplicationException("ERROR EN: ObtenerSucursales()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Sucursales;
        }

        public List<Compannia> ObtenerEmpresa()
        {
            lDato _ldato = new lDato(_configVariables);
            List<Compannia> _Empresa = [];
            DataResponse<List<Compannia>> List_Response = new DataResponse<List<Compannia>>();
            try
            {

                _ldato.Esquema.Add("Codigo", "CODIGO");
                _ldato.Esquema.Add("Nombre", "NOMBRE");
                _ldato.Esquema.Add("Rif", "RIF");
                _ldato.Esquema.Add("Capital", "CAPITAL");
                _ldato.Esquema.Add("RutaLogo", "RUTA_LOGO");
                _ldato.Esquema.Add("DirectorioLogo", "DIRECTORIO_LOGO");
                _ldato.Esquema.Add("DirectorioBanner", "DIRECTORIO_BANNER");
                _ldato.Esquema.Add("Direccion", "DIRECCION");
                _ldato.Esquema.Add("NombreCorto", "NOMBRE_CORTO");
                _ldato.Esquema.Add("Moneda", "MONEDA");

                List_Response = _ldato.EjecutarReader(new Compannia(), "CON_USP_CONSULTARCOMPANNIA", _ldato.Parametros, _ldato.Esquema);
                _Empresa = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"EmpresaRepository", "ObtenerEmpresa()", ex.Message.ToString(), null, null, "CON_USP_CONSULTARCOMPANNIA");
                throw new ApplicationException("ERROR EN: ObtenerEmpresa()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Empresa;
        }
    }
}
