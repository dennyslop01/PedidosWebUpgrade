using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.Repository;
using PedidosWebUpgrade.Infrastructure.Utilities;
using Shyjus.BrowserDetection;
using System.Security.Claims;

namespace PedidosWebUpgrade.Web.Controllers
{
    [Authorize()]
    public class PrincipalController : Controller
    {
        private readonly ConfigVariables _configVariables;
        private readonly IBrowserDetector _browserDetector;

        public PrincipalController(ConfigVariables configVariables, IBrowserDetector browserDetector)
        {
            _configVariables = configVariables;
            _browserDetector = browserDetector;
        }
        
        // GET: Principal
        public async Task<IActionResult> Inicio()
        {
            ClaimsPrincipal principal = HttpContext.User;
            if (principal.Identity != null)
            {
                if (!principal.Identity.IsAuthenticated)
                    return RedirectToAction("IniciarSesion", "Login");
            }

            List<Menu> Model = new List<Menu>();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Principal/Inicio");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PrincipalController", "Inicio", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }

            return View();
        }

        /// <summary>
        /// GET: Cerrar sesión
        /// </summary>
        /// <returns>View("Login")</returns>
        [HttpGet()]
        public async Task<IActionResult> Cerrar()
        {
            HttpContext.Session.Clear();
            Response.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //FormsAuthentication.SignOut();
            return RedirectToAction("Inicio");
        }
    }
}
