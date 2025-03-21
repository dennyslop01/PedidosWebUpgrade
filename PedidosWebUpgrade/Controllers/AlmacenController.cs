using Microsoft.AspNetCore.Mvc;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.Repository;
using PedidosWebUpgrade.Infrastructure.Utilities;
using System.Collections.Specialized;
using System.Collections;
using Shyjus.BrowserDetection;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace PedidosWebUpgrade.Web.Controllers
{
    [Authorize()]
    public class AlmacenController : Controller
    {
        private readonly ConfigVariables _configVariables;
        private readonly IBrowserDetector _browserDetector;

        public AlmacenController(ConfigVariables configVariables, IBrowserDetector browserDetector)
        {
            _configVariables = configVariables;
            _browserDetector = browserDetector;
        }

        public async Task<IActionResult> Index()
        {
            List<Almacen> Modelo = [];
            try
            {
                Modelo = await new AlmacenRepository(_configVariables).ConsultarAlmacenes(0);
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,_configVariables.LogDirectory, "AlmacenController", "HttpPost-Listar", e.ToString(), string.Empty, string.Empty);
            }
            return View(Modelo);
        }

        public async Task<IActionResult> Listar()
        {
            List<Almacen> Modelo = new List<Almacen>();
            try
            {
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Almacen/Listar");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                Modelo = await new AlmacenRepository(_configVariables).ConsultarAlmacenes(0);
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "AlmacenController", "HttpPost-Listar", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }

        [HttpPost]
        public async Task<JsonResult> EliminarAlmacen(int Id)
        {
            int _result = 0;
            try
            {
                _result = await new AlmacenRepository(_configVariables).EliminarAlmacen(Id);

            }
            catch (Exception e)
            {
                //CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "AlmacenController", "HttpPost-EliminarVendedor()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = (_result == 1) });
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int Id)
        {
            Almacen _Modelo = new Almacen();
            try
            {
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Almacen/Detalle");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                if (Id > 0)
                {
                    List<Almacen> _ListAlmacen = await new AlmacenRepository(_configVariables).ConsultarAlmacenes(Id);

                    _Modelo = _ListAlmacen.FirstOrDefault();
                }

                _Modelo.ListaOrdenProdupcion = new VendedorRepository(_configVariables).ConsultarVendedores(0).Result.Select(x => new ListaGeneral { Codigo = x.IdVendedor.ToString(), Descripcion = x.Nombre }).ToList();
                _Modelo.ListProforma =         new VendedorRepository(_configVariables).ConsultarVendedores(0).Result.Select(x => new ListaGeneral { Codigo = x.IdVendedor.ToString(), Descripcion = x.Nombre }).ToList();
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "AlmacenController", "HttpGet-Detalle()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(_Modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Detalle(Almacen Modelo)
        {
            int _result = 0;
            string _msj = string.Empty;
            try
            {
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Almacen/Detalle");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                if (ModelState.IsValid)
                {
                    var _resultQuery = await new AlmacenRepository(_configVariables).ActualizarAlmacen(Modelo);
                    foreach (var item in _resultQuery)
                    {
                        switch (item.Key)
                        {
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

                }
                Modelo.ListaOrdenProdupcion = new VendedorRepository(_configVariables).ConsultarVendedores(0).Result.Select(x => new ListaGeneral { Codigo = x.IdVendedor.ToString(), Descripcion = x.Nombre }).ToList();
                Modelo.ListProforma = new VendedorRepository(_configVariables).ConsultarVendedores(0).Result.Select(x => new ListaGeneral { Codigo = x.IdVendedor.ToString(), Descripcion = x.Nombre }).ToList();
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "AlmacenController", "HttpGet-Detalle()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }

    }
}
