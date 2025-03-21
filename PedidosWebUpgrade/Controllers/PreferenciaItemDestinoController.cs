using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.Repository;
using PedidosWebUpgrade.Infrastructure.Utilities;
using Shyjus.BrowserDetection;

namespace PedidosWebUpgrade.Web.Controllers
{
    public class PreferenciaItemDestinoController : Controller
    {
        private readonly ConfigVariables _configVariables;
        private readonly IBrowserDetector _browserDetector;

        public PreferenciaItemDestinoController(ConfigVariables configVariables, IBrowserDetector browserDetector)
        {
            _configVariables = configVariables;
            _browserDetector = browserDetector;
        }

        // GET: PreferenciaItemDestino
        [HttpGet()]
        public async Task<IActionResult> Listar()
        {
            List<PreferenciaItemDestino> Modelo = new List<PreferenciaItemDestino>();
            try
            {
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse((HttpContext.Session.GetString("idusuario").ToString())), "PreferenciaItemDestino/Listar");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                Modelo = await new PreferenciaItemDestinoRepository(_configVariables).Listar(0);
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PreferenciaItemDestinoController", "HttpGet-Listar", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version, HttpContext.Session.GetString("idusuario").ToString());
            }
            return View(Modelo);
        }

        [HttpGet()]
        public async Task<IActionResult> Detalle(int IdPreferencia)
        {
            PreferenciaItemDestino _Modelo = new PreferenciaItemDestino();
            try
            {
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse((HttpContext.Session.GetString("idusuario").ToString())), "PreferenciaItemDestino/Detalle");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                if (IdPreferencia > 0)
                {
                    List<PreferenciaItemDestino> destinos = await new PreferenciaItemDestinoRepository(_configVariables).Listar(IdPreferencia);
                    _Modelo = destinos.FirstOrDefault();
                }

                List<Product> products = await new ProductoRepository(_configVariables).ObtenerTodoslosProductos(null);
                List<F0005> f005 = await new ConfiguracionRepository(_configVariables).ObtenerF0005("00", "CN", null);

                _Modelo.ListaProductos = products.Select(x => new ListaGeneral() { Codigo = x.Id, Descripcion = x.Id + " - " + x.Descripcion }).ToList();
                _Modelo.ListaPaises = f005.Select(x => new ListaGeneral() { Codigo = x.drky, Descripcion = x.drky + " - " + x.drdl01 }).ToList();
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PreferenciaItemDestinoController", "HttpGet-Detalle()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version, HttpContext.Session.GetString("idusuario").ToString());
            }
            return View(_Modelo);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public async Task<IActionResult> Detalle(PreferenciaItemDestino Modelo)
        {
            int _result = 0;
            string _msj = string.Empty;
            try
            {
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse((HttpContext.Session.GetString("idusuario").ToString())), "PreferenciaItemDestino/Detalle");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                if (ModelState.IsValid)
                {
                    var _resultQuery = await new PreferenciaItemDestinoRepository(_configVariables).Actualizar(Modelo);
                    foreach (var item in _resultQuery)
                    {
                        switch (item.Key)
                        {
                            case "@RESULTADO":
                                _result = Convert.ToInt32(item.Value);
                                break;
                            case "@MSJ":
                                _msj = item.Value.ToString();
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
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PreferenciaItemDestinoController", "HttpPost-Detalle()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }

        [HttpPost()]
        public JsonResult Eliminar(int IdPreferencia)
        {
            int _result = 0;
            string _msj = string.Empty;
            try
            {
                var _resultQuery = new PreferenciaItemDestinoRepository(_configVariables).Eliminar(IdPreferencia).Result;
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
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PreferenciaItemDestinoController", "HttpPost-Eliminar()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _result, msj = _msj });
        }
    }
}
