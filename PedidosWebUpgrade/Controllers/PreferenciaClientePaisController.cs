using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.Repository;
using PedidosWebUpgrade.Infrastructure.Utilities;
using Shyjus.BrowserDetection;

namespace PedidosWebUpgrade.Web.Controllers
{
    public class PreferenciaClientePaisController : Controller
    {
        private readonly ConfigVariables _configVariables;
        private readonly IBrowserDetector _browserDetector;

        public PreferenciaClientePaisController(ConfigVariables configVariables, IBrowserDetector browserDetector)
        {
            _configVariables = configVariables;
            _browserDetector = browserDetector;
        }
        
        public async Task<IActionResult> Listar()
        {
            List<PreferenciaClientePais> Modelo = new List<PreferenciaClientePais>();
            try
            {
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "PreferenciaClientePais/Listar");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                Modelo = await new PreferenciaClientePaisRepository(_configVariables).ObtenerPreferenciaClientePais();
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PreferenciaClientePaisController", "HttpPost-Listar", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }

        [HttpPost]
        public JsonResult Eliminar(string codCliente, string codPais)
        {
            int _result = 0;
            string _msj = string.Empty;
            try
            {
                var _resultQuery = new PreferenciaClientePaisRepository(_configVariables).EliminarPreferenciaClientePais(codCliente, codPais).Result;
                foreach (var item in _resultQuery)
                {
                    switch (item.Key)
                    {
                        case "@MSJ":
                            _msj = item.Value.ToString();
                            break;
                        case "@RESULTADO":
                            _result = Convert.ToInt32(item.Value);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PreferenciaClientePaisController", "HttpPost-Eliminar()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = (_result == 1), msj = _msj });
        }

        [HttpGet]
        public async Task<IActionResult> Detalle()
        {
            PreferenciaClientePais Modelo = new PreferenciaClientePais();
            try
            {
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "PreferenciaClientePais/Detalle");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                //Modelo.Paises = new CestaRepository().ObtenerF0005("00", "CN").Select(x => new ListaGeneral() { Codigo = x.Codigo.Trim(), Descripcion = x.Codigo.Trim() + " - " + x.Descripcion.Trim() }).ToList();

                List<ContadorPedidosPais> pedidosPais = await new ConfiguracionRepository(_configVariables).ConsultarPedidosPais();
                List<Customer> customers = await new ClienteRepository(_configVariables).ObtenerClientes(HttpContext.Session.GetString("idvendedor"), null, 2);

                Modelo.Paises = pedidosPais.Select(x => new ListaGeneral() { Codigo = x.CodigoPais, Descripcion = x.CodigoPais + " - " + x.Pais }).ToList();
                Modelo.Clientes = customers.Select(x => new ListaGeneral() { Codigo = x.CustomerId.Trim(), Descripcion = x.CustomerId.Trim() + " - " + x.Name.Trim() }).ToList();
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PreferenciaClientePaisController", "HttpGet-Detalle()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Detalle(PreferenciaClientePais Modelo)
        {
            int _result = 0;
            string _msj = string.Empty;
            try
            {
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "PreferenciaClientePais/Detalle");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                if (ModelState.IsValid)
                {
                    var _resultQuery = await new PreferenciaClientePaisRepository(_configVariables).ActualizarPreferenciaClientePais(Modelo);
                    foreach (var item in _resultQuery)
                    {
                        switch (item.Key)
                        {
                            case "@MSJ":
                                _msj = item.Value.ToString();
                                break;
                            case "@RESULTADO":
                                _result = Convert.ToInt32(item.Value);
                                break;
                            default:
                                break;
                        }
                    }
                    //OBTENER EL MENSAJE 
                    switch (_result)
                    {
                        case 1:
                            ViewBag.result = true;
                            ViewBag.msj = _msj;
                            break;
                        default:
                            ModelState.AddModelError(string.Empty, _msj);
                            break;
                    }
                }

                List<ContadorPedidosPais> pedidosPais = await new ConfiguracionRepository(_configVariables).ConsultarPedidosPais();
                List<Customer> customers = await new ClienteRepository(_configVariables).ObtenerClientes(HttpContext.Session.GetString("idvendedor"), null, 2);
                
                //Modelo.Paises = new CestaRepository().ObtenerF0005("00", "CN").Select(x => new ListaGeneral() { Codigo = x.Codigo.Trim(), Descripcion = x.Codigo.Trim() + " - " + x.Descripcion.Trim() }).ToList();
                Modelo.Paises = pedidosPais.Select(x => new ListaGeneral() { Codigo = x.CodigoPais, Descripcion = x.CodigoPais + " - " + x.Pais }).ToList();
                Modelo.Clientes = customers.Select(x => new ListaGeneral() { Codigo = x.CustomerId.Trim(), Descripcion = x.CustomerId.Trim() + " - " + x.Name.Trim() }).ToList();
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PreferenciaClientePaisController", "HttpGet-Detalle()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }
    }
}
