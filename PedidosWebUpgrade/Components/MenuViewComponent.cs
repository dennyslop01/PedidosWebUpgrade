using Microsoft.AspNetCore.Mvc;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.Repository;
using PedidosWebUpgrade.Infrastructure.Utilities;
using Shyjus.BrowserDetection;

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

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Menu> Model = new List<Menu>();
            try
            {
                Model = await new MenuRepository(_configVariables).ObtenerMenuUsuario(int.Parse(HttpContext.Session.GetString("idusuario")));
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, "Lo Sentimos, ha ocurrido un error!");
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "MenuController", "ChildActionOnly-MenuPrincipal()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            ;

            return View("_MenuPrincipal", Model);
        }
    }
}
