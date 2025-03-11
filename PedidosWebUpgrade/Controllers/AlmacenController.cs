using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.Repository;
using PedidosWebUpgrade.Infrastructure.Utilities;

namespace PedidosWebUpgrade.Web.Controllers
{
    public class AlmacenController : Controller
    {
        private readonly ConfigVariables _configVariables;
        public AlmacenController(ConfigVariables configVariables)
        {
            _configVariables = configVariables;
        }

        public IActionResult Index()
        {
            CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "AlmacenController", "HttpPost-Listar", "e.ToString()", string.Empty, string.Empty);

            List<Almacen> Modelo = new List<Almacen>();
            try
            {
                Modelo = new AlmacenRepository(_configVariables).ConsultarAlmacenes(0);
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "AlmacenController", "HttpPost-Listar", e.ToString(), string.Empty, string.Empty);
            }
            return View(Modelo);
        }
    }
}
