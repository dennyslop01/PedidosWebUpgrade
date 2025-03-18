using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Org.BouncyCastle.Crypto.Engines;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Domain.ViewModels;
using PedidosWebUpgrade.Infrastructure.Repository;
using PedidosWebUpgrade.Infrastructure.Utilities;
using Shyjus.BrowserDetection;

namespace PedidosWebUpgrade.Web.Controllers
{
    [Authorize()]
    public class CobranzaController : Controller
    {
        private readonly ConfigVariables _configVariables;
        private readonly IBrowserDetector _browserDetector;
        protected ICompositeViewEngine _viewEngine;

        public CobranzaController(ConfigVariables configVariables, IBrowserDetector browserDetector, ICompositeViewEngine viewEngine)
        {
            _configVariables = configVariables;
            _browserDetector = browserDetector;
            _viewEngine = viewEngine;
        }
        
        [HttpGet()]
        public IActionResult EstadoCuenta()
        {
            EstadoCuentaViewModel Modelo = new EstadoCuentaViewModel();

            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Cobranza/EstadoCuenta");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                Modelo.Clientes = new ClienteRepository(_configVariables).ObtenerClientes((string)HttpContext.Session.GetString("idvendedor"), null, 2).Select(x => new ListaGeneral() { Codigo = x.CustomerId, Descripcion = x.CustomerId + " - " + x.Name }).ToList();
                Modelo.Clientes.Insert(0, new ListaGeneral { Codigo = "", Descripcion = "SELECCIONE" });
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"CobranzaController", "HttpGet-EstadoCuena()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);

        }

        /// <summary>
        /// CONVERTIR UNA PARTIAL VIEW EN STRING
        /// </summary>
        /// <param name="viewName">string</param>
        /// <param name="model">object</param>
        /// <returns>StringWriter</returns>
        private string ConvertViewToString(string viewName, object model)
        {
            viewName = viewName ?? ControllerContext.ActionDescriptor.ActionName;
            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                IView view = _viewEngine.FindView(ControllerContext, viewName, true).View;
                ViewContext viewContext = new ViewContext(ControllerContext, view, ViewData, TempData, sw, new HtmlHelperOptions());

                view.RenderAsync(viewContext).Wait();

                return sw.GetStringBuilder().ToString();
            }
        }

        [HttpPost()]
        public IActionResult BuscarMovimientos(string CodCli)
        {
            List<EstadoCuenta> _Movimientos = new List<EstadoCuenta>();
            string viewContent = string.Empty;

            try
            {
                _Movimientos = new CobranzaRepository(_configVariables).ConsultarEstadoCuenta(CodCli);
                viewContent = ConvertViewToString("_Movimientos", _Movimientos);

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"CobranzaController", "HttpPost()-BuscarMovimientos()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = viewContent });
        }

        public IActionResult EnviarEstadoCuenta(string CodCli)
        {

            bool _resultemail = false;
            List<EstadoCuenta> _Movimientos = new List<EstadoCuenta>();

            try
            {
                _Movimientos = new CobranzaRepository(_configVariables).ConsultarEstadoCuenta(CodCli);
                _resultemail = new EmailRepository(_configVariables).SendMailEstadoCuenta((string)HttpContext.Session.GetString("idvendedor"), CodCli, _Movimientos);
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"CobranzaController", "HttpPost()-EnviarEstadoCuenta()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _resultemail });
        }


        [HttpGet]
        public IActionResult CobrarDocumento(string IdCustomer)
        {
            CobranzaClienteViewModel _Modelo = new CobranzaClienteViewModel();
            try
            {

                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Cobranza/CobrarDocumento");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                //DATOS DEL CLIENTE
                _Modelo.IdCliente = new ClienteRepository(_configVariables).ObtenerClientes((string)HttpContext.Session.GetString("idvendedor"), null, 2).Where(x => x.CustomerId == IdCustomer).FirstOrDefault().CustomerId;
                _Modelo.Cliente = new ClienteRepository(_configVariables).ObtenerClientes((string)HttpContext.Session.GetString("idvendedor"), null, 2).Where(x => x.CustomerId == IdCustomer).FirstOrDefault().Name;

                //TIPOS DE PAGO
                _Modelo.PagoCliente.TiposPagos = new CobranzaRepository(_configVariables).ConsultarTipoPago();

                //TIPOS DE MONEDAS
                _Modelo.PagoCliente.TiposMonedas = new CobranzaRepository(_configVariables).ConsultarMonedas();

                //BANCOS
                _Modelo.PagoCliente.Bancos = new CobranzaRepository(_configVariables).ConsultarBancos();

                //MOVIMIENTOS DEL CLIENTE
                _Modelo.Movimientos = new CobranzaRepository(_configVariables).ConsultarEstadoCuenta(IdCustomer);
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"CobranzaController", "HttpGet()-CobrarDocumento()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(_Modelo);

        }

        [HttpPost]
        public IActionResult ObtenerTasaCambio(string moneda)
        {
            TipoCambio _Modelo = new TipoCambio();
            string _tasacambio = "0";
            try
            {
                _Modelo = new CobranzaRepository(_configVariables).ConsultarTipoCambio(moneda).FirstOrDefault();
                if (_Modelo != null) { _tasacambio = _Modelo.TasaCambio; }

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"CobranzaController", "HttpPost()-ObtenerTasaCambio()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }

            return Json(new { tasaCambio = _tasacambio });

        }
    }
}
