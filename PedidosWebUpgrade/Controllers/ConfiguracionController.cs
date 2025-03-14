using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Domain.ViewModels;
using PedidosWebUpgrade.Infrastructure.Repository;
using PedidosWebUpgrade.Infrastructure.Utilities;
using Shyjus.BrowserDetection;

namespace PedidosWebUpgrade.Web.Controllers
{
    public class ConfiguracionController : Controller
    {
        private readonly ConfigVariables _configVariables;
        private readonly IBrowserDetector _browserDetector;
        protected ICompositeViewEngine _viewEngine;

        public ConfiguracionController(ConfigVariables configVariables, IBrowserDetector browserDetector, ICompositeViewEngine viewEngine)
        {
            _configVariables = configVariables;
            _browserDetector = browserDetector;
            _viewEngine = viewEngine;
        }

        [Authorize()]
        [HttpGet()]
        public ActionResult Sistema()
        {
            Sistema _Modelo = new Sistema();
            try
            {

                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Configuracion/Sistema");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                _Modelo = new ConfiguracionRepository(_configVariables).ConsultarSistema().FirstOrDefault();

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpGet-Sistema()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(_Modelo);
        }

        [Authorize()]
        [HttpPost]
        public ActionResult Sistema(Sistema Modelo)
        {
            int _result = 0;
            bool _resultemail = false;

            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Configuracion/Sistema");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                if (ModelState.IsValid)
                {
                    Modelo.Usuario = HttpContext.Session.GetString("login");
                    var _resultQuery = new ConfiguracionRepository(_configVariables).ActualizarSistema(Modelo);
                    foreach (var item in _resultQuery)
                    {
                        switch (item.Key)
                        {
                            case "@IDSISTEMAOUT":
                                Modelo.Id = Convert.ToInt32(item.Value);
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
                            //ENVIAR NOTIFICACION DE CORREO
                            if (!string.IsNullOrEmpty(Modelo.Destinatarios))
                            {
                                _resultemail = new EmailRepository(_configVariables).SendMailAdmSistema(Modelo.Estado, Modelo.Destinatarios, Modelo.Usuario);
                            }
                            break;
                        default:
                            ModelState.AddModelError(string.Empty, " Ha ocurrido un error, verifique los datos e intente de nuevo.");
                            break;
                    }
                }

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpPost-Sistema()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }

        [Authorize()]
        [HttpGet()]
        public ActionResult ContadorPedidos()
        {
            List<ContadorPedidosPais> _Modelo = new List<ContadorPedidosPais>();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Configuracion/ContadorPedidos");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                _Modelo = new ConfiguracionRepository(_configVariables).ConsultarPedidosPais();
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpGet-RegistrarContadorPedidos()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(_Modelo);
        }

        [Authorize()]
        [HttpGet()]
        public ActionResult ConsultarUnContadorPedido(string CodigoPais, int Anno)
        {
            ContadorPedidosPais _Modelo = new ContadorPedidosPais();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Configuracion/ContadorPedidos");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);
                if (!string.IsNullOrWhiteSpace(CodigoPais) && Anno > 0)
                {
                    _Modelo = new ConfiguracionRepository(_configVariables).ConsultarPedidosPais().FirstOrDefault(x => x.CodigoPais.Trim().ToUpper() == CodigoPais.Trim().ToUpper() && x.AnnoCurso == Anno);
                }
                else
                {
                    _Modelo.AnnoCurso = DateTime.Now.Year + 1;
                }
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpGet-RegistrarContadorPedidos()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(_Modelo);
        }

        [Authorize()]
        [HttpPost()]
        public ActionResult ConsultarUnContadorPedido(ContadorPedidosPais Modelo)
        {
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Configuracion/ContadorPedidos");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                if (ModelState.IsValid)
                {
                    var _resultQuery = new ConfiguracionRepository(_configVariables).ActualizarContadorPais(Modelo);
                    if (_resultQuery > 0)
                    {
                        ViewBag.Mensaje = "¡Información guardada Correctamente!";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "¡Ha ocurrido un error, intente de nuevo!");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "¡Debe llenar los campos!");
                }
                Modelo.Paises = new ConfiguracionRepository(_configVariables).ConsultarPedidosPais().Select(x => new ListaGeneral() { Codigo = x.CodigoPais, Descripcion = x.Pais }).ToList();
                ViewBag.action = "POST";
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpGet-RegistrarContadorPedidos()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }


        [Authorize()]
        [HttpGet()]
        public ActionResult CopiarContadores()
        {

            ContadorPedidosPais _Modelo = new ContadorPedidosPais();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Configuracion/CopiarContadores");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                _Modelo.AnnoCurso = DateTime.Now.Year;
                _Modelo.AnnoNuevo = DateTime.Now.Year + 1;
                ViewBag.Mensaje = string.Empty;
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpGet-CopiarContadores()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(_Modelo);
        }

        [Authorize()]
        [HttpPost()]
        public ActionResult CopiarContadores(ContadorPedidosPais Modelo)
        {
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Configuracion/CopiarContadores");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                if (Modelo.AnnoCurso > 0 && Modelo.AnnoNuevo > 0)
                {
                    var _resultQuery = new ConfiguracionRepository(_configVariables).CopiarContadoresAnno(Modelo.AnnoCurso, Modelo.AnnoNuevo);
                    if (_resultQuery > 0)
                    {
                        ViewBag.Mensaje = "¡Información guardada Correctamente!";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "¡Ha ocurrido un error, intente de nuevo!");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "¡Ambos años Deben ser Mayores a Cero!");
                }
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpGet-CopiarContadores()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }

        [Authorize()]
        [HttpPost()]
        public JsonResult ObtenerContadorPais(int Anno, string CodigoPais)
        {
            int contador = 0;
            var resultquery = new ConfiguracionRepository(_configVariables).ConsultarPedidosPais().Where(x => x.AnnoCurso == Anno && x.CodigoPais == CodigoPais).FirstOrDefault();
            if (resultquery != null) { contador = resultquery.Contador; }
            return Json(new { contador });
        }

        [Authorize()]
        [HttpGet()]
        public ActionResult ConsultarF0004()
        {
            List<F0004> Modelo = new List<F0004>();

            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Configuracion/ConsultarF0004");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);


                Modelo = new ConfiguracionRepository(_configVariables).ObtenerF0004(null, null);


            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpGet-ConsultarF0004()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }

        [Authorize()]
        [HttpGet()]
        public ActionResult DetalleF0004(string dtsy, string dtrt)
        {
            F0004 _Modelo = new F0004();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Configuracion/DetalleF0004");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                if (!string.IsNullOrEmpty(dtsy) && !string.IsNullOrEmpty(dtrt))
                {
                    _Modelo = new ConfiguracionRepository(_configVariables).ObtenerF0004(dtsy, dtrt).FirstOrDefault();
                }

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpGet-DetalleF0004()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(_Modelo);
        }

        [Authorize()]
        [HttpPost()]
        public ActionResult DetalleF0004(F0004 Modelo)
        {

            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Configuracion/DetalleF0004");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);


                var _resultQuery = new ConfiguracionRepository(_configVariables).ActualizarF0004(Modelo);
                if (_resultQuery > 0)
                {
                    ViewBag.result = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "¡Ha ocurrido un error, intente de nuevo!");
                }

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpPost-DetalleF0004()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            ModelState.Clear();
            return View(Modelo);

        }

        [HttpPost()]
        public JsonResult EliminarF0004(string dtsy, string dtrt)
        {
            int _result = 0;
            try
            {
                _result = new ConfiguracionRepository(_configVariables).EliminarF0004(dtsy, dtrt);

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpPost-EliminarF0004()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _result });
        }

        [Authorize()]
        [HttpGet()]
        public ActionResult ConsultarF0005()
        {
            ConsultarF0005ViewModel Modelo = new ConsultarF0005ViewModel();

            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Configuracion/ConsultarF0005");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                //LLENADO DE  LISTAS
                Modelo.Productos = new ConfiguracionRepository(_configVariables).ObtenerF0004(null, null).Select(x => new ListaGeneral() { Codigo = x.dtsy, Descripcion = x.dtsy }).ToList();
                Modelo.Productos.Insert(0, new ListaGeneral { Codigo = "", Descripcion = "SELECCIONE" });

                Modelo.CodigoUsuarios = new ConfiguracionRepository(_configVariables).ObtenerF0004(null, null).Select(x => new ListaGeneral() { Codigo = x.dtrt, Descripcion = x.dtrt }).ToList();
                Modelo.CodigoUsuarios.Insert(0, new ListaGeneral { Codigo = "", Descripcion = "SELECCIONE" });

                Modelo.ListF0005 = new ConfiguracionRepository(_configVariables).ObtenerF0005(null, null, null);


            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpGet-ConsultarF0005()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }

        [HttpPost()]
        public ActionResult BuscarF0005(string CodigoProducto, string CodigoUsuario)
        {
            List<F0005> _ListF0005 = new List<F0005>();
            string viewContent = string.Empty;
            try
            {
                _ListF0005 = new ConfiguracionRepository(_configVariables).ObtenerF0005(CodigoProducto, CodigoUsuario, null);
                viewContent = ConvertViewToString("_ListF0005", _ListF0005);

                //GUARDAR LOS VALORES DE BUSQUEDA
                TempData["CodigoProducto"] = CodigoProducto;
                TempData["CodigoUsuario"] = CodigoUsuario;

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpPost()-BuscarF0005()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = viewContent });
        }

        [HttpPost()]
        public JsonResult EliminarF0005(string drrt, string drsy, string drky)
        {
            int _result = 0;
            try
            {
                _result = new ConfiguracionRepository(_configVariables).EliminarF0005(drrt, drsy, drky);

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpPost-EliminarF0005()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _result });
        }

        [Authorize()]
        [HttpGet()]
        public ActionResult DetalleF0005(string drsy, string drrt, string drky)
        {
            F0005 _Modelo = new F0005();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Configuracion/DetalleF0005");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                if (!string.IsNullOrEmpty(drsy) && !string.IsNullOrEmpty(drrt))
                {
                    _Modelo = new ConfiguracionRepository(_configVariables).ObtenerF0005(drsy, drrt, drky).FirstOrDefault();
                    ViewBag.disabled = true;
                }

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpGet-DetalleF0005()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(_Modelo);
        }

        [Authorize()]
        [HttpPost()]
        public ActionResult DetalleF0005(F0005 Modelo)
        {
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Configuracion/DetalleF0005");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                //VERIFICAR VALORES EN TABLA F0004
                List<F0004> _F0004 = new ConfiguracionRepository(_configVariables).ObtenerF0004(Modelo.drsy, Modelo.drrt).ToList();
                if (_F0004.Count > 0)
                {

                    var _resultQuery = new ConfiguracionRepository(_configVariables).ActualizarF0005(Modelo);
                    if (_resultQuery > 0)
                    {
                        ViewBag.result = true;
                        ViewBag.disabled = true;
                    }
                    else
                    {
                        ViewBag.disabled = false;
                        ModelState.AddModelError(string.Empty, "¡Ha ocurrido un error, intente de nuevo!");
                    }
                }
                else
                {
                    ViewBag.disabled = false;
                    ModelState.AddModelError(string.Empty, "Los códigos de producto y de usuario no existen.");
                }



            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpPost-DetalleF0005()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            //ModelState.Clear();
            return View(Modelo);

        }

        [Authorize()]
        [HttpGet()]
        public ActionResult ConsultarPreferenciaAlmacenClientePais()
        {
            PrefeAlmClienPaisViewModel Modelo = new PrefeAlmClienPaisViewModel();

            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Configuracion/ConsultarPreferenciaAlmacenClientePais");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                Modelo.PrefAlmClientPaisList = new ConfiguracionRepository(_configVariables).ObtenerPreferenciaAlmacenClientePais(0);
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpGet-ConsultarPreferenciaAlmacenClientePais()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }

        [HttpPost()]
        public JsonResult EliminarPreferenciaAlmacenClientePais(int IdPreferencia)
        {
            int _result = 0;
            try
            {
                _result = new ConfiguracionRepository(_configVariables).EliminarPreferenciaAlmacenClientePais(IdPreferencia);
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpPost-EliminarPreferenciaAlmacenClientePais()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = (_result == 1) });
        }

        [Authorize()]
        [HttpGet()]
        public ActionResult DetallePreferenciaAlmacenClientePais(int IdPreferencia)
        {
            PrefeAlmClienPaisViewModel Modelo = new PrefeAlmClienPaisViewModel();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Configuracion/DetallePreferenciaAlmacenClientePais");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                if (IdPreferencia > 0)
                {
                    PreferenciaAlmacenClientePais model = new PreferenciaAlmacenClientePais();
                    model = new ConfiguracionRepository(_configVariables).ObtenerPreferenciaAlmacenClientePais(IdPreferencia).FirstOrDefault();
                    Modelo.IdPreferencia = model.IdPreferencia;
                    Modelo.IdAlmacen = model.IdAlmacen;
                    Modelo.IdCliente = model.IdCliente;
                    Modelo.Codpais = model.Codpais;
                    ViewBag.disabled = true;
                }
                Modelo.ListaAlmacenes = new AlmacenRepository(_configVariables).ConsultarAlmacenes(0).Select(x => new ListaGeneral() { Codigo = x.IdAlmacen, Descripcion = x.IdAlmacen + " - " + x.Descripcion }).ToList();
                //Modelo.ListaClientes = new ClienteRepository().ObtenerClientes((string)Session["idvendedor"], string.Empty, 2).Select(x => new ListaGeneral() { Codigo = x.CustomerId, Descripcion = x.CustomerId + " - " + x.Name }).ToList();
                Modelo.ListaClientes = new ClienteRepository(_configVariables).ObtenerClientesPotencia(null).Select(x => new ListaGeneral() { Codigo = x.CustomerId.Trim(), Descripcion = x.CustomerId.Trim() + " - " + x.Name.Trim() }).ToList();
                Modelo.ListaPaises = new ConfiguracionRepository(_configVariables).ConsultarPedidosPais().Select(x => new ListaGeneral() { Codigo = x.CodigoPais, Descripcion = x.CodigoPais + " - " + x.Pais }).ToList();
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpGet-DetallePreferenciaAlmacenClientePais()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }

        [Authorize()]
        [HttpPost()]
        public ActionResult DetallePreferenciaAlmacenClientePais(PreferenciaAlmacenClientePais preferenciaAlmacenClientePais)
        {
            PrefeAlmClienPaisViewModel _Modelo = new PrefeAlmClienPaisViewModel();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Configuracion/DetallePreferenciaAlmacenClientePais");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                int _result = 0;
                string _msj = string.Empty;
                if (ModelState.IsValid)
                {
                    var _resultQuery = new ConfiguracionRepository(_configVariables).ActualizarPreferenciaAlmacenClientePais(preferenciaAlmacenClientePais);
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
                        case 2:
                            ModelState.AddModelError(string.Empty, " Ha ocurrido un error, ya existe un registro con mismo Pais, mismo Almacen y mismo Cliente.");
                            break;
                        default:
                            ModelState.AddModelError(string.Empty, " Ha ocurrido un error, verifique los datos e intente de nuevo.");
                            break;
                    }
                }
                else
                {
                    ViewBag.disabled = false;
                    ModelState.AddModelError(string.Empty, "Faltan campos por seleccionar.");
                }
                _Modelo = new PrefeAlmClienPaisViewModel()
                {
                    ListaAlmacenes = new AlmacenRepository(_configVariables).ConsultarAlmacenes(0).Select(x => new ListaGeneral() { Codigo = x.IdAlmacen, Descripcion = x.IdAlmacen + " - " + x.Descripcion }).ToList(),
                    //ListaClientes = new ClienteRepository().ObtenerClientes((string)Session["idvendedor"], string.Empty, 2).Select(x => new ListaGeneral() { Codigo = x.CustomerId, Descripcion = x.CustomerId + " - " + x.Name }).ToList(),
                    ListaClientes = new ClienteRepository(_configVariables).ObtenerClientesPotencia(null).Select(x => new ListaGeneral() { Codigo = x.CustomerId.Trim(), Descripcion = x.CustomerId.Trim() + " - " + x.Name.Trim() }).ToList(),
                    ListaPaises = new ConfiguracionRepository(_configVariables).ConsultarPedidosPais().Select(x => new ListaGeneral() { Codigo = x.CodigoPais, Descripcion = x.CodigoPais + " - " + x.Pais }).ToList(),
                    Codpais = preferenciaAlmacenClientePais.Codpais,
                    IdAlmacen = preferenciaAlmacenClientePais.IdAlmacen,
                    IdCliente = preferenciaAlmacenClientePais.IdCliente,
                    IdPreferencia = preferenciaAlmacenClientePais.IdPreferencia
                };
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpPost-DetallePreferenciaAlmacenClientePais()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(_Modelo);
        }

        [HttpPost()]
        public JsonResult ObtenerListCodigoUsuario(string dtsy)
        {
            List<ListaGeneral> CodigosUsuarios = new ConfiguracionRepository(_configVariables).ObtenerF0004(dtsy, null).Select(x => new ListaGeneral() { Codigo = x.dtrt, Descripcion = x.dtrt }).ToList();
            CodigosUsuarios.Insert(0, new ListaGeneral { Codigo = "", Descripcion = "SELECCIONE" });
            return Json(new { CodigosUsuarios });
        }

        [HttpPost()]
        public JsonResult ObtenerUDC(string dtsy, string dtrt)
        {
            string descripcion = new ConfiguracionRepository(_configVariables).ObtenerF0004(dtsy, dtrt).Select(x => x.dtdl01).FirstOrDefault();
            return Json(new { descripcion });
        }

        [Authorize()]
        [HttpGet()]
        public ActionResult ConsultarAgentesAduanales()
        {
            List<ForwardingAgent> Modelo = new List<ForwardingAgent>();

            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Configuracion/ConsultarAgentesAduanales");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                //LLENADO DE  LISTAS
                Modelo = new ConfiguracionRepository(_configVariables).ConsultarAgentesAduanales();
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpGet-ConsultarAgentesAduanales()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }

        [Authorize()]
        [HttpGet()]
        public ActionResult DetalleAgenteAduanal(int AgentId)
        {

            ForwardingAgent _Modelo = new ForwardingAgent();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Configuracion/DetalleAgenteAduanal");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                if (AgentId > 0)
                {
                    _Modelo = new ConfiguracionRepository(_configVariables).ConsultarUnAgenteAduanal(AgentId);
                }
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpPost-DetalleAgenteAduanal()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(_Modelo);
        }

        [Authorize()]
        [HttpPost()]
        public ActionResult DetalleAgenteAduanal(ForwardingAgent forwardingAgent)
        {
            ForwardingAgent _Modelo = new ForwardingAgent();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Configuracion/DetalleAgenteAduanal");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);
                if (ModelState.IsValid)
                {
                    var _resultQuery = new ConfiguracionRepository(_configVariables).ActualizarAgenteAduanal(forwardingAgent);
                    if (bool.Parse(_resultQuery["@RESULT"].ToString()))
                    {
                        ViewBag.Mensaje = "¡Información guardada Con Exito!";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "¡Ha ocurrido un error al intentar guardar la información, intente de nuevo!");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "¡Ha ocurrido un error al intentar guardar la información, intente de nuevo!");
                }
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpPost-DetalleF0004()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(_Modelo);
        }

        [HttpGet()]
        public JsonResult EliminarAgenteAduanal(int AgentId)
        {
            int _result = 0;
            try
            {
                _result = new ConfiguracionRepository(_configVariables).EliminarAgenteAduanal(AgentId);

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ConfiguracionController", "HttpPost-EliminarAgenteAduanal()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _result });
        }

        private string ConvertViewToString(string viewName, object model)
        {
            viewName = viewName ?? ControllerContext.ActionDescriptor.ActionName;
            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                IView view = _viewEngine.FindView(ControllerContext, viewName, true).View;
                ViewContext viewContext = new ViewContext(ControllerContext, view, ViewData, TempData, sw, new HtmlHelperOptions());

                view.RenderAsync(viewContext).Wait();

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
