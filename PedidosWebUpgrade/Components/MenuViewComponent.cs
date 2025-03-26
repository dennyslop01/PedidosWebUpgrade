using Microsoft.AspNetCore.Mvc;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.Repository;
using PedidosWebUpgrade.Infrastructure.Utilities;
using Shyjus.BrowserDetection;
using System.Security.Claims;

namespace PedidosWebUpgrade.Web.Components
{
    [ViewComponent(Name = "MenuView")]
    public class MenuViewComponent : ViewComponent
    {
        private readonly ConfigVariables _configVariables;
        private readonly IBrowserDetector _browserDetector;

        public MenuViewComponent(ConfigVariables configVariables, IBrowserDetector browserDetector)
        {
            _configVariables = configVariables;
            _browserDetector = browserDetector;
        }

        public IViewComponentResult Invoke(int numberOfItems)
        {
            List<Menu> Model = new List<Menu>();
            try
            {
                ClaimsPrincipal principal = HttpContext.User;

                int idUsuario = int.Parse(principal.FindFirst(ClaimTypes.UserData).Value);

                Model = new MenuRepository(_configVariables).ObtenerMenuUsuario(idUsuario).Result;
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, "Lo Sentimos, ha ocurrido un error!");
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "MenuController", "ChildActionOnly-MenuPrincipal()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            ;

            return View(Model);
        }
    }
}
