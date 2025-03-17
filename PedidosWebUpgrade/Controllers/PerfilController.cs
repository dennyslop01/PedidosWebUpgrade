using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.Repository;
using PedidosWebUpgrade.Infrastructure.Utilities;
using Shyjus.BrowserDetection;
using System.Text.Json;

namespace PedidosWebUpgrade.Web.Controllers
{
    public class PerfilController : Controller
    {
        private readonly ConfigVariables _configVariables;
        private readonly IBrowserDetector _browserDetector;

        public PerfilController(ConfigVariables configVariables, IBrowserDetector browserDetector)
        {
            _configVariables = configVariables;
            _browserDetector = browserDetector;
        }
        
        [HttpGet()]
        public IActionResult Listar()
        {
            List<Perfil> Modelo = new List<Perfil>();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Perfil/Listar");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                Modelo = new PerfilRepository(_configVariables).ObtenerPerfil(0);
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilController", "HttpGet-Listar", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }

        [HttpPost()]
        public JsonResult EliminarPerfil(int IdPerfil)
        {
            int _result = 0;
            try
            {
                _result = new PerfilRepository(_configVariables).EliminarPerfil(IdPerfil);

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilController", "HttpPost-EliminarPerfil()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _result });
        }

        [HttpGet()]
        public IActionResult Detalle(int IdPerfil)
        {
            Perfil _Modelo = new Perfil();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Perfil/Detalle");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                if (IdPerfil > 0)
                {
                    _Modelo = new PerfilRepository(_configVariables).ObtenerPerfil(IdPerfil).FirstOrDefault();
                    _Modelo.OpcionesMenu = new PerfilRepository(_configVariables).ObtenerPerfilMenu(IdPerfil, 0);
                }

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilController", "HttpGet-Detalle()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(_Modelo);
        }

        [HttpPost()]
        public IActionResult Detalle(Perfil Modelo)
        {

            int _result = 0;
            string _msj = string.Empty;
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Perfil/Detalle");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                if (ModelState.IsValid)
                {
                    var _resultQuery = new PerfilRepository(_configVariables).ActualizarPerfil(Modelo);
                    foreach (var item in _resultQuery)
                    {
                        switch (item.Key)
                        {
                            case "@IDPERFILOUT":
                                Modelo.IdPerfil = Convert.ToInt32(item.Value);
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
                        case 2:
                            ModelState.AddModelError(string.Empty, "El nombre de perfil ya existe.");
                            break;
                        default:
                            ModelState.AddModelError(string.Empty, " Ha ocurrido un error, verifique los datos e intente de nuevo.");
                            break;
                    }
                    Modelo.OpcionesMenu = new PerfilRepository(_configVariables).ObtenerPerfilMenu(Modelo.IdPerfil, 0);

                }
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilController", "HttpPost-Detalle()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            ModelState.Clear();

            return View(Modelo);
        }

        [HttpPost()]
        public JsonResult EliminarPerfilMenu(int Idperfilmenu)
        {
            int _result = 0;
            try
            {
                _result = new PerfilRepository(_configVariables).EliminarPerfilMenu(Idperfilmenu);

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilController", "HttpPost-EliminarPerfilMenu()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _result });
        }

        [HttpGet()]
        public IActionResult CargarOpcionesMenu(int Idperfil)
        {

            List<Menu> _Opciones = new List<Menu>();
            List<PerfilMenu> _OpcionesMenu = new List<PerfilMenu>();
            try
            {
                _Opciones = new MenuRepository(_configVariables).ObtenerMenu(0);
                _OpcionesMenu = new PerfilRepository(_configVariables).ObtenerPerfilMenu(Idperfil, 0);
                _Opciones = _Opciones.Where(x => !_OpcionesMenu.Where(b => b.Idmenu == x.IdMenu).Any()).ToList();
                ViewBag.Idperfil = Idperfil;
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilController", "HttpGet()-CargarOpcionesMenu()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return PartialView("_OpcionesMenu", _Opciones);
        }

        [HttpPost()]
        public IActionResult RegistrarOpciones(string ObjJson)
        {
            bool _result = false;

            try
            {
                //JavaScriptSerializer serializer = new JavaScriptSerializer();
                List<PerfilMenu> _PerfilMenu = JsonSerializer.Deserialize<List<PerfilMenu>>(ObjJson);

                //INSERTAR LISTADO DE PRODUCTOS
                for (var i = 0; i < _PerfilMenu.Count; i++)
                {
                    _result = new PerfilRepository(_configVariables).IncluirPerfilMenu(_PerfilMenu[i]);
                }
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilController", "HttpPost-RegistrarOpciones()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { estatus = _result });
        }

        [HttpGet()]
        public IActionResult Ver(int IdUser)
        {
            Usuario _Modelo = new Usuario();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Perfil/Detalle");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                _Modelo = new UsuarioRepository(_configVariables).ObtenerUsuario(IdUser).FirstOrDefault();
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilController", "HttpPost-Ver()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(_Modelo);
        }

        [HttpPost]
        public IActionResult Ver(Usuario Modelo)
        {
            Usuario _Modelo = new Usuario();
            int _result = 0;
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Perfil/Detalle");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                _result = new PerfilRepository(_configVariables).ActualizarMiPerfil(Modelo);
                ViewBag.result = _result;
                Modelo = new UsuarioRepository(_configVariables).ObtenerUsuario(Convert.ToInt32(Modelo.IdUsuario)).FirstOrDefault();
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilController", "HttpPost-Ver()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }
    }
}
