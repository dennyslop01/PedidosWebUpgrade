using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.Repository;
using PedidosWebUpgrade.Infrastructure.Utilities;
using Shyjus.BrowserDetection;

namespace PedidosWebUpgrade.Web.Controllers
{
    [Authorize()]
    public class MenuController : Controller
    {
        private readonly ConfigVariables _configVariables;
        private readonly IBrowserDetector _browserDetector;

        public MenuController(ConfigVariables configVariables, IBrowserDetector browserDetector)
        {
            _configVariables = configVariables;
            _browserDetector = browserDetector;
        }

        //[ChildActionOnly]
        //public async Task<IActionResult> MenuPrincipal()
        //{
            
        //}

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            List<Menu> Modelo = new List<Menu>();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Menu/Listar");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                Modelo = new MenuRepository(_configVariables).ObtenerMenu(0);
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"MenuController", "HttpGet-Listar", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }

        [HttpPost]
        public JsonResult EliminarMenu(int IdMenu)
        {
            int _result = 0;
            try
            {
                _result = new MenuRepository(_configVariables).EliminarMenu(IdMenu).Result;

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"MenuController", "HttpPost-EliminarMenu()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _result });
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int IdMenu)
        {
            Menu _Modelo = new Menu();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Menu/Detalle");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                if (IdMenu > 0)
                {
                    _Modelo = new MenuRepository(_configVariables).ObtenerMenu(IdMenu).FirstOrDefault();

                }
                _Modelo.Padres = new MenuRepository(_configVariables).ObtenerMenu(0).Where(x => x.IdPadre == 0).Select(x => new ListaGeneral() { IdTipo = x.IdMenu, Descripcion = x.Descripcion }).ToList();
                _Modelo.Padres.Add(new ListaGeneral() { IdTipo = 0, Descripcion = "SIN PADRE" });

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"MenuController", "HttpGet-Detalle()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(_Modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Detalle(Menu Modelo)
        {
            int _IdMenu = 0;
            int _result = 0;
            string _msj = string.Empty;
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Menu/Detalle");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                if (ModelState.IsValid)
                {

                    var _resultQuery = new MenuRepository(_configVariables).ActualizarMenu(Modelo);
                    foreach (var item in _resultQuery)
                    {
                        switch (item.Key)
                        {
                            case "@IDMENUOUT":
                                _IdMenu = Convert.ToInt32(item.Value);
                                Modelo.IdMenu = _IdMenu;
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
                            break;
                        default:
                            ModelState.AddModelError(string.Empty, " Ha ocurrido un error, verifique los datos e intente de nuevo.");
                            break;
                    }
                    //LLENAR LISTA DE PADRES
                    Modelo.Padres = new MenuRepository(_configVariables).ObtenerMenu(0).Where(x => x.IdPadre == 0).Select(x => new ListaGeneral() { IdTipo = x.IdMenu, Descripcion = x.Descripcion }).ToList();
                    Modelo.Padres.Add(new ListaGeneral() { IdTipo = 0, Descripcion = "SIN PADRE" });

                }
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilController", "HttpPost-Detalle()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }
    }
}
