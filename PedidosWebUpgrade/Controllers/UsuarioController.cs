using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.Repository;
using PedidosWebUpgrade.Infrastructure.Utilities;
using Shyjus.BrowserDetection;
using System.Text.Json;

namespace PedidosWebUpgrade.Web.Controllers
{
    [Authorize()]
    public class UsuarioController : Controller
    {
        private readonly ConfigVariables _configVariables;
        private readonly IBrowserDetector _browserDetector;

        public UsuarioController(ConfigVariables configVariables, IBrowserDetector browserDetector)
        {
            _configVariables = configVariables;
            _browserDetector = browserDetector;
        }
        
        [HttpGet()]
        public async Task<IActionResult> Listar()
        {
            List<Usuario> Modelo = new List<Usuario>();
            try
            {
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Usuario/Listar");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                Modelo = new UsuarioRepository(_configVariables).ObtenerUsuario(0).Result;
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"UsuarioController", "HttpPost-Listar", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }

        [HttpPost]
        public JsonResult EliminarUsuario(int IdUsuario)
        {
            int _result = 0;
            try
            {
                _result = new UsuarioRepository(_configVariables).EliminarUsuario(IdUsuario).Result;

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"UsuarioController", "HttpPost-EliminarUsuario()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _result });
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int IdUser)
        {
            Usuario _Modelo = new Usuario();
            try
            {
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Usuario/Detalle");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                if (IdUser > 0)
                {
                    List<Usuario> usuarios = await new UsuarioRepository(_configVariables).ObtenerUsuario(IdUser);
                    _Modelo = usuarios.FirstOrDefault();
                    _Modelo.Perfiles = new PerfilRepository(_configVariables).ObtenerPerfilUsuario(0, IdUser).Result;
                }

                _Modelo.TiposUsuarios = await new UsuarioRepository(_configVariables).ObtenerTipoUsuario();

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"UsuarioController", "HttpGet-Detalle()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(_Modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Detalle(Usuario Modelo)
        {
            int _IdUsuario = 0;
            int _result = 0;
            string _msj = string.Empty;
            try
            {
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Usuario/Detalle");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                if (ModelState.IsValid)
                {
                    var _resultQuery = await new UsuarioRepository(_configVariables).ActualizarUsuario(Modelo);
                    foreach (var item in _resultQuery)
                    {
                        switch (item.Key)
                        {
                            case "@IDNUSUARIOOUT":
                                _IdUsuario = Convert.ToInt32(item.Value);
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
                            ModelState.AddModelError(string.Empty, "El número de cédula ya existe.");
                            break;
                        default:
                            ModelState.AddModelError(string.Empty, " Ha ocurrido un error, verifique los datos e intente de nuevo.");
                            break;
                    }

                    Modelo.Perfiles = new PerfilRepository(_configVariables).ObtenerPerfilUsuario(0, Modelo.IdUsuario).Result;

                }

                Modelo.TiposUsuarios = await new UsuarioRepository(_configVariables).ObtenerTipoUsuario();
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"UsuarioController", "HttpGet-Detalle()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }

        [HttpPost()]
        public JsonResult EliminarPerfilUsuario(int IdperfilUsuario)
        {
            int _result = 0;
            try
            {
                _result = new PerfilRepository(_configVariables).EliminarPerfilUsuario(IdperfilUsuario).Result;

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"UsuarioController", "HttpPost-EliminarPerfilUsuario()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _result });
        }

        [HttpGet()]
        public async Task<IActionResult> CargarPerfiles(int IdUsuario)
        {

            List<Perfil> _Perfiles = new List<Perfil>();
            List<PerfilUsuario> _PerfilUsuario = new List<PerfilUsuario>();
            try
            {
                _Perfiles = new PerfilRepository(_configVariables).ObtenerPerfil(0).Result;
                _PerfilUsuario = new PerfilRepository(_configVariables).ObtenerPerfilUsuario(0, IdUsuario).Result;
                _Perfiles = _Perfiles.Where(x => !_PerfilUsuario.Where(b => b.IdPerfil == x.IdPerfil).Any()).ToList();
                ViewBag.IdUsuario = IdUsuario;
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"UsuarioController", "HttpGet()-CargarPerfiles()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return PartialView("_PerfilesUsuario", _Perfiles);
        }

        [HttpPost()]
        public async Task<IActionResult> RegistrarPerfiles(string ObjJson)
        {
            bool _result = false;

            try
            {
                List<PerfilUsuario> _PerfilUsuario = JsonSerializer.Deserialize<List<PerfilUsuario>>(ObjJson);

                //INSERTAR LISTADO DE PRODUCTOS
                for (var i = 0; i < _PerfilUsuario.Count; i++)
                {
                    _result = new PerfilRepository(_configVariables).IncluirPerfilUsuario(_PerfilUsuario[i]).Result;
                }
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"UsuarioController", "HttpPost-RegistrarPerfiles()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { estatus = _result });
        }
    }
}
