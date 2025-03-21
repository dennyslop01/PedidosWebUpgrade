using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.Repository;
using PedidosWebUpgrade.Infrastructure.Utilities;
using Shyjus.BrowserDetection;

namespace PedidosWebUpgrade.Web.Controllers
{
    [Authorize()]
    public class ClienteController : Controller
    {
        private readonly ConfigVariables _configVariables;
        private readonly IBrowserDetector _browserDetector;

        public ClienteController(ConfigVariables configVariables, IBrowserDetector browserDetector)
        {
            _configVariables = configVariables;
            _browserDetector = browserDetector;
        }
        
        [HttpGet()]
        public async Task<IActionResult> FacturasPendientes()
        {
            List<Customer> _Clientes = new List<Customer>();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Cliente/FacturasPendientes");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);


                _Clientes = await new ClienteRepository(_configVariables).ObtenerClientes(null, null, 1);

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ClienteController", "HttpGet-FacturasPendientes()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }


            return View(_Clientes);
        }

        [HttpPost()]
        public JsonResult ActualizarFactura(string CustomerId, string Valor)
        {
            int _result = 0;
            bool factpend = false;
            try
            {
                factpend = (Valor == "SI" ? false : true);
                _result = new ClienteRepository(_configVariables).ActualizarFacturaPendiente(CustomerId, factpend).Result;
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ClienteController", "HttpPost-ActualizarFactura()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _result, valor = factpend });
        }

        [HttpPost()]
        public JsonResult VerificarFacturaPendiente(string CustomerId)
        {
            int _result = 0;
            try
            {

                _result = new ClienteRepository(_configVariables).VerificarFacturaPendiente(CustomerId).Result;
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ClienteController", "HttpPost-ActualizarFactura()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _result });
        }
    }
}
