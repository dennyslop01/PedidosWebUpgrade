using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.Repository;
using PedidosWebUpgrade.Infrastructure.Utilities;
using Shyjus.BrowserDetection;

namespace PedidosWebUpgrade.Web.Controllers
{
    public class ProductoController : Controller
    {
        private readonly ConfigVariables _configVariables;
        private readonly IBrowserDetector _browserDetector;
        private readonly IWebHostEnvironment _env;

        public ProductoController(ConfigVariables configVariables, IBrowserDetector browserDetector, IWebHostEnvironment env)
        {
            _configVariables = configVariables;
            _browserDetector = browserDetector;
            _env = env;
        }

        [HttpGet()]
        public IActionResult AsociarIconoMarca()
        {
            List<ListaGeneral> _Marcas = new List<ListaGeneral>();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Producto/AsociarIconoMarca");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                _Marcas = new ProductoRepository(_configVariables).ObtenerMarcas();
                _Marcas.Insert(0, new ListaGeneral { Codigo = "", Descripcion = "SELECCIONE" });
                ViewData["marcas"] = new SelectList(from s in _Marcas select new { Codigo = s.Codigo, Descripcion = s.Codigo + " - " + s.Descripcion }, "Codigo", "Descripcion");
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "ProductoController", "HttpGet-AsociarIconoMarca()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View();
        }

        [HttpPost()]
        public IActionResult AsociarIconoMarca(string ListMarcas, IFormFile ImageFile)
        {
            List<ListaGeneral> _Marcas = new List<ListaGeneral>();
            string _nombreimg = string.Empty;
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Producto/AsociarIconoMarca");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);



                _Marcas = new ProductoRepository(_configVariables).ObtenerMarcas();
                _Marcas.Insert(0, new ListaGeneral { Codigo = "", Descripcion = "SELECCIONE" });
                ViewData["marcas"] = new SelectList(from s in _Marcas select new { Codigo = s.Codigo, Descripcion = s.Codigo + " - " + s.Descripcion }, "Codigo", "Descripcion");

                if (!string.IsNullOrEmpty(ListMarcas))
                {
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        _nombreimg = string.Format("{0:}{1}", ListMarcas, Path.GetExtension(ImageFile.FileName));
                        var _resultQuery = new ProductoRepository(_configVariables).ActualizarIconoMarca(_nombreimg, ListMarcas);

                        if (_resultQuery == 1)
                        {
                            var path = _env.WebRootPath + _configVariables.RepoImg;


                            //VERIFICAR QUE EXISTA Y ELIMINARLA
                            string _archivo = string.Format("{0}\\{1}", path, _nombreimg);
                            if (System.IO.File.Exists(_archivo))
                            {
                                System.IO.File.Delete(_archivo);
                            }


                            //ImageFile.CopyTo(path);
                            ViewBag.Mensaje = "¡Información guardada!";
                        }

                    }
                    else
                    {
                        //DEBE SELECCIONAR UNA IMAGEN
                        ModelState.AddModelError(string.Empty, "Debe seleccionar una imagen");
                    }
                }
                else
                {
                    //DEBE SELECCIONAR UNA MARCA
                    ModelState.AddModelError(string.Empty, "Debe seleccionar una marca");
                }


            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "ProductoController", "HttpPost-AsociarIconoMarca()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
                //ViewBag.Mensaje = "Debe seleccionar una marca";
                ModelState.AddModelError(string.Empty, "Ha ocurrido un error, intente de nuevo");
            }
            return View(_Marcas);
        }
    }
}
