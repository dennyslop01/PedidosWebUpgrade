using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.Repository;
using PedidosWebUpgrade.Infrastructure.Utilities;
using Shyjus.BrowserDetection;
using System.Security.Claims;
using System.Text.Json;

namespace PedidosWebUpgrade.Web.Controllers
{
    [Authorize()]
    public class PerfilController : Controller
    {
        private readonly ConfigVariables _configVariables;
        private readonly IBrowserDetector _browserDetector;
        private int _idUsuario = 0;

        public PerfilController(ConfigVariables configVariables, IBrowserDetector browserDetector)
        {
            _configVariables = configVariables;
            _browserDetector = browserDetector;
        }

        private void ValidarSession()
        {
            try
            {
                ClaimsPrincipal principal = HttpContext.User;
                if (principal.Identity != null)
                {
                    if (!principal.Identity.IsAuthenticated)
                        RedirectToAction("IniciarSesion", "Login");
                }
                _idUsuario = int.Parse(principal.FindFirst(ClaimTypes.UserData).Value);
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "ConfiguracionController", "HttpGet-Sistema()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
                RedirectToAction("IniciarSesion", "Login");
            }
        }

        [HttpGet()]
        public async Task<IActionResult> Listar()
        {
            List<Perfil> Modelo = new List<Perfil>();
            try
            {
                ValidarSession();
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(_idUsuario, "Perfil/Listar");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                Modelo = await new PerfilRepository(_configVariables).ObtenerPerfil(0);
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
                _result = new PerfilRepository(_configVariables).EliminarPerfil(IdPerfil).Result;

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilController", "HttpPost-EliminarPerfil()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _result });
        }

        [HttpGet()]
        public async Task<IActionResult> Detalle(int IdPerfil)
        {
            Perfil _Modelo = new Perfil();
            try
            {
                ValidarSession();
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(_idUsuario, "Perfil/Detalle");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                if (IdPerfil > 0)
                {
                    List<Perfil> perfil = await new PerfilRepository(_configVariables).ObtenerPerfil(IdPerfil);
                    _Modelo = perfil.FirstOrDefault();
                    _Modelo.OpcionesMenu = await new PerfilRepository(_configVariables).ObtenerPerfilMenu(IdPerfil, 0);
                }

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilController", "HttpGet-Detalle()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(_Modelo);
        }

        [HttpPost()]
        public async Task<IActionResult> Detalle(Perfil Modelo)
        {

            int _result = 0;
            string _msj = string.Empty;
            try
            {
                ValidarSession();
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(_idUsuario, "Perfil/Detalle");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                if (ModelState.IsValid)
                {
                    var _resultQuery = await new PerfilRepository(_configVariables).ActualizarPerfil(Modelo);
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
                    Modelo.OpcionesMenu = await new PerfilRepository(_configVariables).ObtenerPerfilMenu(Modelo.IdPerfil, 0);

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
                _result = new PerfilRepository(_configVariables).EliminarPerfilMenu(Idperfilmenu).Result;

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilController", "HttpPost-EliminarPerfilMenu()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _result });
        }

        [HttpGet()]
        public async Task<IActionResult> CargarOpcionesMenu(int Idperfil)
        {

            List<Menu> _Opciones = new List<Menu>();
            List<PerfilMenu> _OpcionesMenu = new List<PerfilMenu>();
            try
            {
                _Opciones = await new MenuRepository(_configVariables).ObtenerMenu(0);
                _OpcionesMenu = await new PerfilRepository(_configVariables).ObtenerPerfilMenu(Idperfil, 0);
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
        public async Task<IActionResult> RegistrarOpciones(string ObjJson)
        {
            bool _result = false;

            try
            {
                //JavaScriptSerializer serializer = new JavaScriptSerializer();
                List<PerfilMenu> _PerfilMenu = JsonSerializer.Deserialize<List<PerfilMenu>>(ObjJson);

                //INSERTAR LISTADO DE PRODUCTOS
                for (var i = 0; i < _PerfilMenu.Count; i++)
                {
                    _result = await new PerfilRepository(_configVariables).IncluirPerfilMenu(_PerfilMenu[i]);
                }
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilController", "HttpPost-RegistrarOpciones()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { estatus = _result });
        }

        [HttpGet()]
        public async Task<IActionResult> Ver(int IdUser)
        {
            Usuario _Modelo = new Usuario();
            try
            {
                ValidarSession();
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(_idUsuario, "Perfil/Detalle");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                List<Usuario> usuarios = await new UsuarioRepository(_configVariables).ObtenerUsuario(IdUser);
                _Modelo = usuarios.FirstOrDefault();
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilController", "HttpPost-Ver()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(_Modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Ver(Usuario Modelo)
        {
            Usuario _Modelo = new Usuario();
            int _result = 0;
            try
            {
                ValidarSession();
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(_idUsuario, "Perfil/Detalle");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                _result = new PerfilRepository(_configVariables).ActualizarMiPerfil(Modelo).Result;
                ViewBag.result = _result;
                List<Usuario> usuarios = await new UsuarioRepository(_configVariables).ObtenerUsuario(Convert.ToInt32(Modelo.IdUsuario));
                Modelo = usuarios.FirstOrDefault();
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PerfilController", "HttpPost-Ver()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }
    }
}
