using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidosWebUpgrade.Models;
using Microsoft.IdentityModel.Tokens;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.Repository;
using PedidosWebUpgrade.Infrastructure.Utilities;
using Shyjus.BrowserDetection;
using System.Security.Claims;

namespace PedidosWebUpgrade.Controllers;

[Authorize()]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ConfigVariables _configVariables;
    private readonly IBrowserDetector _browserDetector;

    public HomeController(ILogger<HomeController> logger, ConfigVariables configVariables, IBrowserDetector browserDetector)
    {
        _logger = logger;
        _configVariables = configVariables;
        _browserDetector = browserDetector;
    }

    public async Task<IActionResult> Index()
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
            int idUsuario = int.Parse(principal.FindFirst(ClaimTypes.UserData).Value);

            Model = await new MenuRepository(_configVariables).ObtenerMenuUsuario(idUsuario);
            //Model = await new MenuRepository(_configVariables).ObtenerMenuUsuario(int.Parse(HttpContext.Session.GetString("idusuario")));
        }
        catch (Exception e)
        {
            ModelState.AddModelError(string.Empty, "Lo Sentimos, ha ocurrido un error!");
            CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "MenuController", "ChildActionOnly-MenuPrincipal()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
        }

        return View(Model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    

}
