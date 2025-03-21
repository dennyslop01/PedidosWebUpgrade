using iTextSharp.text;
using iTextSharp.text.pdf;
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
using System.Globalization;
using System.Text.Json;

namespace PedidosWebUpgrade.Web.Controllers
{
    [Authorize()]
    public class PedidoController : Controller
    {
        private readonly ConfigVariables _configVariables;
        private readonly IBrowserDetector _browserDetector;
        protected ICompositeViewEngine _viewEngine;
        private readonly IWebHostEnvironment _env;


        public PedidoController(ConfigVariables configVariables, IBrowserDetector browserDetector, ICompositeViewEngine viewEngine, IWebHostEnvironment env)
        {
            _configVariables = configVariables;
            _browserDetector = browserDetector;
            _viewEngine = viewEngine;
            _env = env;
        }

        /// <summary>
        /// GET: MOSTRAR PEDIDO
        /// </summary>
        /// <returns>PedidoViewModel</returns>
        [HttpGet()]
        public async Task<IActionResult> Crear(int back = 0, int? TipoPedido = 1)
        {
            PedidoViewModel Modelo = new PedidoViewModel();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Pedido/Crear");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);


                if (back == 1)
                {
                    Modelo.CodigoCliente = TempData["CodCli"].ToString();
                    Modelo.CodigoSucursal = TempData["CodSuc"].ToString();
                    Modelo.CodigoCategoria = TempData["CodCat"].ToString();
                    TempData["UrlBack"] = back;
                }



                Modelo.Sucursales = new EmpresaRepository(_configVariables).ObtenerSucursales(HttpContext.Session.GetString("idvendedor")).Select(x => new ListaGeneral() { Codigo = x.Codigo, Descripcion = x.Codigo + " - " + x.Descripcion }).ToList();
                Modelo.Clientes = new ClienteRepository(_configVariables).ObtenerClientes(HttpContext.Session.GetString("idvendedor"), (Modelo.CodigoSucursal == "" ? Modelo.Sucursales.First().Codigo : Modelo.CodigoSucursal), 2).Select(x => new ListaGeneral() { Codigo = x.CustomerId, Descripcion = x.CustomerId + " - " + x.Name }).ToList();
                Modelo.Categorias = new ProductoRepository(_configVariables).ObtenerCategorias().Select(x => new ListaGeneral() { Codigo = x.Codigo, Descripcion = x.Codigo + " - " + x.Descripcion }).ToList();



                Modelo.Encabezado = await new PedidoRepository(_configVariables).ObtenerEncabezadoOrden(0, (Modelo.CodigoCliente == "" ? Modelo.Clientes.FirstOrDefault().Codigo : Modelo.CodigoCliente), HttpContext.Session.GetString("idvendedor"));

                Modelo.CabecListaPrecios = new ListaPreciosRepository(_configVariables).ConsultarCabeceraListoPrecio().Select(x => new ListaGeneral() { Codigo = x.Codigo.Trim(), Descripcion = x.Descripcion.Trim() }).ToList();
                Modelo.ShipTo = new ClienteRepository(_configVariables).ObtenerShipTo(null, null, 2, null).Select(x => new ListaGeneral() { Codigo = x.CustomerId, Descripcion = x.CustomerId + " - " + x.Name }).ToList();

                Modelo.Productos = new ProductoRepository(_configVariables).ObtenerProductos("", (Modelo.CodigoSucursal == "" ? Modelo.Sucursales.FirstOrDefault().Codigo : Modelo.CodigoSucursal),
                                                                             (Modelo.CodigoCategoria == "" ? Modelo.Categorias.FirstOrDefault().Codigo : Modelo.CodigoCategoria),
                                                                             (Modelo.CodigoCliente == "" ? Modelo.Clientes.FirstOrDefault().Codigo : Modelo.CodigoCliente),
                                                                             (Modelo.CodigoCabecListaPrecios == "" ? Modelo.CabecListaPrecios.FirstOrDefault().Codigo : Modelo.CodigoCabecListaPrecios)
                                                                             ).ToList();
                Modelo.TipoPedido = TipoPedido;

                if (Modelo.Encabezado == null)
                {
                    Modelo.Encabezado = new EncabezadoPedido();
                }
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpGet-Crear", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }

        [Authorize()]
        [HttpGet()]
        public JsonResult ObtenerCustomerCPGP(string IdCliente)
        {
            string CPGP = string.Empty;
            try
            {
                Customer Cliente = new ClienteRepository(_configVariables).ObtenerClientes(null, null, 2).Find(x => x.CustomerId == IdCliente);
                if (Cliente != null)
                {
                    CPGP = !string.IsNullOrWhiteSpace(Cliente.IdListaPrecio) ? Cliente.IdListaPrecio.Trim() : string.Empty;
                }
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpGet-ObtenerCustomerCPGP()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = CPGP });
        }

        /// <summary>
        /// POST: BÚSQUEDA DE PRODUCTOS
        /// </summary>
        /// <param name="CodSuc">string</param>
        /// <param name="CodCat">string</param>
        /// <returns>PartialView</returns>
        [HttpPost()]
        public JsonResult BuscarProductos(string CodSuc, string CodCat, string CodCli, string CodListPrecio)
        {
            List<Producto> _Productos = new List<Producto>();
            EncabezadoPedido _Encabezado = new EncabezadoPedido();
            string viewContent = string.Empty;
            try
            {
                _Productos = new ProductoRepository(_configVariables).ObtenerProductos("", CodSuc, CodCat, CodCli, CodListPrecio).ToList();
                _Encabezado = new PedidoRepository(_configVariables).ObtenerEncabezadoOrden(0, CodCli, HttpContext.Session.GetString("idvendedor")).Result;
                if (_Encabezado == null)
                {
                    _Encabezado = new EncabezadoPedido();
                }
                viewContent = ConvertViewToString("_Productos", _Productos);

                TempData["CodCli"] = CodCli;
                TempData["CodSuc"] = CodSuc;
                TempData["CodCat"] = CodCat;

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpPost-BuscarProductos()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { PartialView = viewContent, encabezado = _Encabezado });
        }

        [HttpPost()]
        public JsonResult ConsultarProductosCesta(int IdOrden, string CodCli, string IdListaPrecio)
        {
            List<Producto> _Productos = new List<Producto>();
            EncabezadoPedido _Encabezado = new EncabezadoPedido();
            string viewContent = string.Empty;
            try
            {
                _Productos = new CestaRepository(_configVariables).ObtenerProductosCesta(IdOrden, CodCli, IdListaPrecio).Result;
                _Encabezado = new PedidoRepository(_configVariables).ObtenerEncabezadoOrden(IdOrden, CodCli, HttpContext.Session.GetString(("idvendedor"))).Result;
                if (_Encabezado == null)
                {
                    _Encabezado = new EncabezadoPedido();
                }
                viewContent = ConvertViewToString("_Productos", _Productos);

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpPost-ConsultarProductosCesta()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { PartialView = viewContent, encabezado = _Encabezado });
        }

        [HttpPost()]
        public JsonResult ConsultarProductosSeleccionados(int IdOrden)
        {
            List<Producto> _Productos = new List<Producto>();
            string viewContent = string.Empty;
            try
            {
                _Productos = new CestaRepository(_configVariables).ObtenerProductosCestaSeleccionados(IdOrden).Result;
                viewContent = ConvertViewToString("_Productos", _Productos);

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpPost-ConsultarProductosSeleccionados()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { PartialView = viewContent });
        }

        /// <summary>
        /// POST: AGREGAR PRODUCTO A ORDEN
        /// </summary>
        /// <param name="Producto">string</param>
        /// <param name="CodCli">string</param>
        /// <param name="Idorden">int</param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> AgregarProducto(string Producto, string CodCli, int Idorden)
        {
            int _resultado = 0;
            int _idOrder = 0;
            EncabezadoPedido _Encabezado = new EncabezadoPedido();

            try
            {

                Producto _Producto = JsonSerializer.Deserialize<Producto>(Producto);
                var _resultQuery = await new PedidoRepository(_configVariables).AgregarProducto(_Producto, CodCli, Idorden);
                foreach (var item in _resultQuery)
                {
                    switch (item.Key)
                    {
                        case "@RESULTADO":
                            _resultado = Convert.ToInt32(item.Value);
                            break;
                        case "@ORDERIDOUT":
                            _idOrder = Convert.ToInt32(item.Value);
                            break;
                        default:
                            break;
                    }
                }
                _Encabezado = new PedidoRepository(_configVariables).ObtenerEncabezadoOrden(_idOrder, CodCli).Result;
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpPost-AgregarProducto()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _resultado, encabezado = _Encabezado });
        }

        /// <summary>
        /// POST: PROCESAR ORDEN 
        /// </summary>
        /// <param name="IdOrden">int</param>
        /// <returns>Json</returns>
        [HttpPost()]
        public async Task<IActionResult> ProcesarOrden(int IdOrden, string ParamCesta)
        {
            bool _result = false;
            bool _resultemail = false;
            bool _resultemaildscto = false;
            byte[] bytes;
            try
            {
                dynamic DatosCesta = JsonSerializer.Deserialize<Object>(ParamCesta);


                _result = new PedidoRepository(_configVariables).ProcesarOrden(IdOrden, DatosCesta["condicion"], int.Parse(DatosCesta["descuento"]), DatosCesta["fechapedido"],
                                                                DatosCesta["fecharequerida"], DatosCesta["Observaciones"], DatosCesta["montototal"], DatosCesta["montodscto"],
                                                                DatosCesta["prepagado"], DatosCesta["tasanegociacion"], DatosCesta["carrier"], DatosCesta["puertodescarga"],
                                                                DatosCesta["iconterms"], "", DatosCesta["tiempollegada"], DatosCesta["ordenprint"], Convert.ToInt32(HttpContext.Session.GetString("idusuario")),
                                                                DatosCesta["fechacorte"], DatosCesta["leadtime"], DatosCesta["modalidadtransporte"], DatosCesta["FechaDespachoETD"],
                                                                DatosCesta["FechaLlegadaETA"], int.Parse(DatosCesta["ForwardingAgent"]), DatosCesta["CodListaPrecios"], DatosCesta["CodShiptTo"],
                                                                DatosCesta["CodAlmacen"], DatosCesta["CodCondPago"], DatosCesta["CodPaisFacturacion"], int.Parse(DatosCesta["CodAnnoCorrelativo"].ToString())
                                                                , int.Parse(DatosCesta["TipoPedido"].ToString()));
                /*
                if (_result)
                {
                    //enviar el correo a salesman
                    _resultemail = new EmailRepository().SendMailSalesmen(IdOrden, Session["login"].ToString(), DatosCesta["fechapedido"], DatosCesta["montototal"], Convert.ToInt32(HttpContext.Session.GetString("idusuario")));
                    //verificar si trae descuento  para enviar el correo   
                    if (int.Parse(DatosCesta["descuento"]) > 0)
                    {
                        _resultemaildscto = new EmailRepository().SendMailDescuento(IdOrden, Session["idvendedor"].ToString(), DatosCesta, Convert.ToInt32(HttpContext.Session.GetString("idusuario")));
                    }

                }
                */

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpPost-ProcesarOrden()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _result });
        }

        [HttpPost()]
        public async Task<IActionResult> AprobarOrden(int IdOrden, string NroOrden)
        {
            bool _result = false;
            //bool _resultemail = false;
            byte[] bytes;

            try
            {

                _result = new PedidoRepository(_configVariables).AprobarOrden(IdOrden).Result;
                if (_result)
                {
                    bytes = ConstruirPDF(IdOrden, "PRODUCTION  ORDER", 1);

                    var _resultEmailAprobacion = new EmailRepository(_configVariables).SendMailAprobarPedido(IdOrden);
                    //_resultemail = new EmailRepository().SendMailLogistica(IdOrden, Session["login"].ToString());
                }

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpPost-ProcesarOrden()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _result });
        }

        /// <summary>
        /// CONVERTIR UNA PARTIAL VIEW EN STRING
        /// </summary>
        /// <param name="viewName">string</param>
        /// <param name="model">object</param>
        /// <returns>StringWriter</returns>
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

        /// <summary>
        /// POST: LISTADO DE CLIENTES SEGÚN SUCURSAL
        /// </summary>
        /// <returns>Json</returns>
        [Authorize()]
        [HttpPost()]
        public JsonResult ObtenerDescuestoComercial(int IdOrden, float Porcentaje)
        {
            List<Descuento> Descuentos = new List<Descuento>();
            try
            {
                Descuentos = new PedidoRepository(_configVariables).ObtenerCalculoDescuentoComercial(IdOrden, Porcentaje).Select(x => new Descuento() { BaseImponibleCesta = x.BaseImponibleCesta, ImpuestoCesta = x.ImpuestoCesta, DescuentoCesta = x.DescuentoCesta, TotalPagarCesta = x.TotalPagarCesta }).ToList();
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpPost-ObtenerContadorAnnoPaisAjax()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { Descuentos });
        }

        [Authorize()]
        [HttpPost()]
        public JsonResult ObtenerClientes(string CodSuc)
        {
            List<ListaGeneral> Clientes = new List<ListaGeneral>();
            try
            {
                Clientes = new ClienteRepository(_configVariables).ObtenerClientes(HttpContext.Session.GetString("idvendedor"), CodSuc, 2).Select(x => new ListaGeneral() { Codigo = x.CustomerId, Descripcion = x.CustomerId + " - " + x.Name }).ToList();

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpPost-ObtenerClientes()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { Clientes });
        }

        [Authorize()]
        [HttpPost()]
        public JsonResult ObtenerContadorAnnoPaisAjax(string CodPais)
        {
            List<ListaGeneral> Contadores = new List<ListaGeneral>();
            try
            {
                Contadores = new PedidoRepository(_configVariables).ConsultarContadoresAnnoPais(CodPais).Select(x => new ListaGeneral() { Codigo = x.IdTipo.ToString(), Descripcion = x.Descripcion.Trim() }).ToList();
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpPost-ObtenerContadorAnnoPaisAjax()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { Contadores });
        }

        [HttpGet()]
        public async Task<IActionResult> Consultar(string CodCli = null, string NroPedido = "0", string CodEstatus = null, string FechaDesde = null, string FechaHasta = null)
        {
            PedidosGeneradosViewModel _Modelo = new PedidosGeneradosViewModel();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Pedido/Consultar");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);


                TempData["CodCli"] = CodCli;
                TempData["NroPedido"] = NroPedido;
                TempData["CodEstatus"] = CodEstatus;
                TempData["FechaDesde"] = FechaDesde;
                TempData["FechaHasta"] = FechaHasta;


                _Modelo = new PedidosGeneradosViewModel { CodigoCliente = CodCli, CodigoEstatus = CodEstatus, FechaDesde = FechaDesde, FechaHasta = FechaHasta, NumeroPedido = NroPedido };
                _Modelo.Clientes = new ClienteRepository(_configVariables).ObtenerClientes(HttpContext.Session.GetString("idvendedor"), null, 2).Select(x => new ListaGeneral() { Codigo = x.CustomerId, Descripcion = x.CustomerId + " - " + x.Name }).ToList();
                _Modelo.Clientes.Insert(0, new ListaGeneral { Codigo = "", Descripcion = "TODOS" });
                _Modelo.Estatus = await new PedidoRepository(_configVariables).ObtenerEstatus();
                _Modelo.Estatus.Insert(0, new ListaGeneral { Codigo = "", Descripcion = "TODOS" });
                _Modelo.Ordenes = new PedidoRepository(_configVariables).ConsultarPedidos(CodCli, FechaDesde, FechaHasta, Convert.ToInt32(NroPedido), CodEstatus, Convert.ToInt32(HttpContext.Session.GetString("idusuario"))).GroupBy(a => a.IdOrder).Select(g => g.First()).ToList();
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpGet-Consultar()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(_Modelo);
        }

        [HttpGet()]
        public async Task<IActionResult> ConsultarModificacion(string CodCli = null, string NroPedido = "0", string CodEstatus = null, string FechaDesde = null, string FechaHasta = null)
        {
            PedidosGeneradosViewModel _Modelo = new PedidosGeneradosViewModel();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Pedido/ConsultarModificacion");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);


                TempData["CodCli"] = CodCli;
                TempData["NroPedido"] = NroPedido;
                TempData["CodEstatus"] = CodEstatus;
                TempData["FechaDesde"] = FechaDesde;
                TempData["FechaHasta"] = FechaHasta;

                _Modelo = new PedidosGeneradosViewModel { CodigoCliente = CodCli, CodigoEstatus = CodEstatus, FechaDesde = FechaDesde, FechaHasta = FechaHasta, NumeroPedido = NroPedido };
                _Modelo.Clientes = new ClienteRepository(_configVariables).ObtenerClientes(HttpContext.Session.GetString("idvendedor"), null, 2).Select(x => new ListaGeneral() { Codigo = x.CustomerId, Descripcion = x.CustomerId + " - " + x.Name }).ToList();
                _Modelo.Clientes.Insert(0, new ListaGeneral { Codigo = "", Descripcion = "TODOS" });
                _Modelo.Estatus = await new PedidoRepository(_configVariables).ObtenerEstatus();
                _Modelo.Estatus.Insert(0, new ListaGeneral { Codigo = "", Descripcion = "TODOS" });
                _Modelo.Ordenes = new PedidoRepository(_configVariables).ConsultarPedidos(CodCli, FechaDesde, FechaHasta, Convert.ToInt32(NroPedido), CodEstatus, Convert.ToInt32(HttpContext.Session.GetString("idusuario"))).GroupBy(a => a.IdOrder).Select(g => g.First()).ToList();
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpGet-Consultar()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(_Modelo);
        }

        [HttpPost()]
        public JsonResult ActualizarNumeroCorrelativo(string Nrocorrelativo, int IdOrden)
        {
            bool _result = false;
            try
            {
                _result = new CestaRepository(_configVariables).ActualizarCorrelativo(Nrocorrelativo, IdOrden).Result;
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "CestaController", "HttpGet-ActualizarNumeroCorrelativo()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _result });
        }

        [HttpPost()]
        public async Task<IActionResult> BuscarPedidos(string CodCli, int NroPedido, string CodEstatus, string FechaDesde, string FechaHasta)
        {
            List<Orders> _pedidos = new List<Orders>();
            string viewContent = string.Empty;

            try
            {
                _pedidos = new PedidoRepository(_configVariables).ConsultarPedidos(CodCli, FechaDesde, FechaHasta, NroPedido, CodEstatus, Convert.ToInt32(HttpContext.Session.GetString("idusuario"))).GroupBy(a => a.IdOrder).Select(g => g.First()).ToList();
                viewContent = ConvertViewToString("_Pedidos", _pedidos);

                //GUARDAR LOS VALORES DE BUSQUEDA
                TempData["CodCli"] = CodCli;
                TempData["NroPedido"] = NroPedido;
                TempData["CodEstatus"] = CodEstatus;
                TempData["FechaDesde"] = FechaDesde;
                TempData["FechaHasta"] = FechaHasta;

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpPost()-BuscarPedidos()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }

            return Json(new { result = viewContent });

        }

        [HttpPost()]
        public async Task<IActionResult> BuscarPedidosParaModificacion(string CodCli, int NroPedido, string CodEstatus, string FechaDesde, string FechaHasta)
        {
            List<Orders> _pedidos = new List<Orders>();
            string viewContent = string.Empty;

            try
            {
                _pedidos = new PedidoRepository(_configVariables).ConsultarPedidos(CodCli, FechaDesde, FechaHasta, NroPedido, CodEstatus, Convert.ToInt32(HttpContext.Session.GetString("idusuario"))).GroupBy(a => a.IdOrder).Select(g => g.First()).ToList();
                viewContent = ConvertViewToString("_PedidosConsulta", _pedidos);

                //GUARDAR LOS VALORES DE BUSQUEDA
                TempData["CodCli"] = CodCli;
                TempData["NroPedido"] = NroPedido;
                TempData["CodEstatus"] = CodEstatus;
                TempData["FechaDesde"] = FechaDesde;
                TempData["FechaHasta"] = FechaHasta;

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpPost()-BuscarPedidosParaModificacion()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }

            return Json(new { result = viewContent });

        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int IdOrden)
        {
            PedidosGeneradosViewModel _Modelo = new PedidosGeneradosViewModel();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Pedido/VerPedido");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                _Modelo.Ordenes = await new PedidoRepository(_configVariables).ConsultarPedidos("", "", "", IdOrden, "", Convert.ToInt32(HttpContext.Session.GetString("idusuario")));

                List<ListaGeneral> General = new List<ListaGeneral>();
                //DATOS COMPLEMENTARIOS
                if (!string.IsNullOrEmpty(_Modelo.Ordenes.FirstOrDefault().IdCarrier))
                {
                    General = new CestaRepository(_configVariables).ObtenerF0005("55", "CR").Where(x => x.Codigo == _Modelo.Ordenes.FirstOrDefault().IdCarrier).ToList();
                    _Modelo.Ordenes.FirstOrDefault().Carrier = (General.Count() == 0) ? string.Empty : General.FirstOrDefault().Descripcion;
                }
                if (!string.IsNullOrEmpty(_Modelo.Ordenes.FirstOrDefault().IdPuertoDescarga))
                {
                    General = new CestaRepository(_configVariables).ObtenerF0005("55", "PS").Where(x => x.Codigo == _Modelo.Ordenes.FirstOrDefault().IdPuertoDescarga).ToList();
                    _Modelo.Ordenes.FirstOrDefault().PuertoDescarga = (General.Count() == 0) ? string.Empty : General.FirstOrDefault().Descripcion;
                }
                if (!string.IsNullOrEmpty(_Modelo.Ordenes.FirstOrDefault().IdIncoTerms))
                {
                    General = new CestaRepository(_configVariables).ObtenerF0005("42", "FR").Where(x => x.Codigo == _Modelo.Ordenes.FirstOrDefault().IdIncoTerms).ToList();
                    _Modelo.Ordenes.FirstOrDefault().IncoTerms = (General.Count() == 0) ? string.Empty : General.FirstOrDefault().Descripcion;
                }
                if (!string.IsNullOrEmpty(_Modelo.Ordenes.FirstOrDefault().IdRef001))
                {
                    General = new CestaRepository(_configVariables).ObtenerF0005("00", "00").Where(x => x.Codigo == _Modelo.Ordenes.FirstOrDefault().IdRef001).ToList();
                    _Modelo.Ordenes.FirstOrDefault().Referencia001 = (General.Count() == 0) ? string.Empty : General.FirstOrDefault().Descripcion;
                }
                if (!string.IsNullOrEmpty(_Modelo.Ordenes.FirstOrDefault().CodigoModTransporte))
                {
                    General = new CestaRepository(_configVariables).ObtenerF0005("00", "TM").Where(x => x.Codigo == _Modelo.Ordenes.FirstOrDefault().CodigoModTransporte).ToList();
                    _Modelo.Ordenes.FirstOrDefault().ModTransporte = (General.Count() == 0) ? string.Empty : General.FirstOrDefault().Descripcion;
                }

                //CONSULTAR SEGUIMIENTO DE LA ORDEN
                _Modelo.SeguimientoPedido.SeguimientoOrden = await new PedidoRepository(_configVariables).ObtenerSeguimiento(IdOrden);
                if (_Modelo.SeguimientoPedido.SeguimientoOrden.Count > 0)
                {
                    //CONSULTAR LOS ESTATUS
                    _Modelo.SeguimientoPedido.EstatusOrden = await new PedidoRepository(_configVariables).ObtenerEstatusOrden();
                }

                //CONSULTAR EL HISTORICO
                _Modelo.PedidosHistoricos = await new PedidoRepository(_configVariables).ConsultarHistoricoPedidos(_Modelo.Ordenes.FirstOrDefault().IdOrderOriginal);

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpGet()-VerPedido()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(_Modelo);
        }

        [HttpGet]
        public async Task<IActionResult> DetalleModificacion(int IdOrden)
        {
            PedidosGeneradosViewModel _Modelo = new PedidosGeneradosViewModel();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Pedido/VerPedido");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                _Modelo.Ordenes = await new PedidoRepository(_configVariables).ConsultarPedidos("", "", "", IdOrden, "", Convert.ToInt32(HttpContext.Session.GetString("idusuario")));

                List<ListaGeneral> General = new List<ListaGeneral>();
                //DATOS COMPLEMENTARIOS
                if (!string.IsNullOrEmpty(_Modelo.Ordenes.FirstOrDefault().IdCarrier))
                {
                    General = new CestaRepository(_configVariables).ObtenerF0005("55", "CR").Where(x => x.Codigo == _Modelo.Ordenes.FirstOrDefault().IdCarrier).ToList();
                    _Modelo.Ordenes.FirstOrDefault().Carrier = (General.Count() == 0) ? string.Empty : General.FirstOrDefault().Descripcion;
                }
                if (!string.IsNullOrEmpty(_Modelo.Ordenes.FirstOrDefault().IdPuertoDescarga))
                {
                    General = new CestaRepository(_configVariables).ObtenerF0005("55", "PS").Where(x => x.Codigo == _Modelo.Ordenes.FirstOrDefault().IdPuertoDescarga).ToList();
                    _Modelo.Ordenes.FirstOrDefault().PuertoDescarga = (General.Count() == 0) ? string.Empty : General.FirstOrDefault().Descripcion;
                }
                if (!string.IsNullOrEmpty(_Modelo.Ordenes.FirstOrDefault().IdIncoTerms))
                {
                    General = new CestaRepository(_configVariables).ObtenerF0005("42", "FR").Where(x => x.Codigo == _Modelo.Ordenes.FirstOrDefault().IdIncoTerms).ToList();
                    _Modelo.Ordenes.FirstOrDefault().IncoTerms = (General.Count() == 0) ? string.Empty : General.FirstOrDefault().Descripcion;
                }
                if (!string.IsNullOrEmpty(_Modelo.Ordenes.FirstOrDefault().IdRef001))
                {
                    General = new CestaRepository(_configVariables).ObtenerF0005("00", "00").Where(x => x.Codigo == _Modelo.Ordenes.FirstOrDefault().IdRef001).ToList();
                    _Modelo.Ordenes.FirstOrDefault().Referencia001 = (General.Count() == 0) ? string.Empty : General.FirstOrDefault().Descripcion;
                }
                if (!string.IsNullOrEmpty(_Modelo.Ordenes.FirstOrDefault().CodigoModTransporte))
                {
                    General = new CestaRepository(_configVariables).ObtenerF0005("00", "TM").Where(x => x.Codigo == _Modelo.Ordenes.FirstOrDefault().CodigoModTransporte).ToList();
                    _Modelo.Ordenes.FirstOrDefault().ModTransporte = (General.Count() == 0) ? string.Empty : General.FirstOrDefault().Descripcion;
                }

                //CONSULTAR SEGUIMIENTO DE LA ORDEN
                _Modelo.SeguimientoPedido.SeguimientoOrden = await new PedidoRepository(_configVariables).ObtenerSeguimiento(IdOrden);
                if (_Modelo.SeguimientoPedido.SeguimientoOrden.Count > 0)
                {
                    //CONSULTAR LOS ESTATUS
                    _Modelo.SeguimientoPedido.EstatusOrden = await new PedidoRepository(_configVariables).ObtenerEstatusOrden();
                }

                //CONSULTAR EL HISTORICO
                _Modelo.PedidosHistoricos = await new PedidoRepository(_configVariables).ConsultarHistoricoPedidos(_Modelo.Ordenes.FirstOrDefault().IdOrderOriginal);

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpGet()-VerPedido()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(_Modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Seguimiento()
        {

            //PERMISOS DE USUARIO
            List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Pedido/Seguimiento");
            TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
            TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
            TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
            TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

            return View(new SeguimientoViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Seguimiento(SeguimientoViewModel Modelo)
        {
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = await new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Pedido/Seguimiento");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                //CONSULTAR SEGUIMIENTO DE LA ORDEN
                Modelo.SeguimientoOrden = await new PedidoRepository(_configVariables).ObtenerSeguimiento(Convert.ToInt32(Modelo.IdOrden));
                if (Modelo.SeguimientoOrden.Count > 0)
                {
                    //CONSULTAR LOS ESTATUS
                    Modelo.EstatusOrden = await new PedidoRepository(_configVariables).ObtenerEstatusOrden();
                }



            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpPost()-Seguimiento()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }

        [HttpGet]
        public async Task<IActionResult> GenerarPDF(int nroorden, string ordernumber, short type)//1:Production Order, 2: Proforma
        {
            try
            {
                string TypeToPrint = "";

                if (type == 1)
                {
                    TypeToPrint = "PRODUCTION  ORDER";
                }
                else if (type == 2)
                {
                    TypeToPrint = "PROFORMA";
                }
                else if (type == 3)
                {
                    TypeToPrint = "PROFORMA POS";
                }

                byte[] bytes;
                bytes = ConstruirPDF(nroorden, TypeToPrint, type);
                Response.Clear();
                Response.ContentType = "application/pdf";
                //Response.AddHeader("Content-Disposition", "attachment; filename=" + TypeToPrint + "_" + ordernumber + ".pdf");
                Response.ContentType = "application/pdf";
                //Response.Buffer = true;
                //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //Response.BinaryWrite(bytes);
                //Response.End();
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpPost()-GenerarPDF()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return RedirectToAction("Consultar", "Pedido");
        }

        public byte[] ConstruirPDF(int nroorden, string titulo, int tipo)
        {
            List<PedidoPDFViewModel> _Detalle = new List<PedidoPDFViewModel>();
            PedidoPDFViewModel _Pedido = new PedidoPDFViewModel();

            //LLENADO DEL MODELO
            int parTipo = tipo;
            if (tipo == 3)
                parTipo = 2;
            _Detalle = new PedidoRepository(_configVariables).ConsultarOrdenPdf(nroorden, parTipo).Result;
            _Pedido = _Detalle.FirstOrDefault();
            byte[] bytes;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document doc = new Document(PageSize.LETTER, 40f, 40f, 40f, 40f);
                PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                doc.Open();

                //fuente del contenido
                Font _smallFontC = new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD, BaseColor.BLACK);
                Font _standardFontC = new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL, BaseColor.BLACK);
                Font _standardFontP = new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD, BaseColor.BLACK);
                Font _standardFontN = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, BaseColor.BLACK);
                Font _standardFontG = new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD, BaseColor.BLACK);
                Font _standardFontS = new Font(Font.FontFamily.HELVETICA, 10, Font.UNDERLINE, BaseColor.BLACK);
                Paragraph spacerParagraph = new Paragraph();

                //ENCABEZADO
                PdfPTable tblEncabezado = new PdfPTable(2);
                tblEncabezado.WidthPercentage = 100;

                Image img = Image.GetInstance(_env.WebRootPath + "~/Content/img/sitio/logo.png");
                // Para impresion de orden de produccion
                if (tipo == 1)
                {
                    img = Image.GetInstance(_env.WebRootPath + String.Concat("~/Content/img/sitio/", _Pedido.NombreLogoProduccion));
                }
                else
                {
                    img = Image.GetInstance(_env.WebRootPath + String.Concat("~/Content/img/sitio/", _Pedido.NombreLogoProforma));
                }

                PdfPCell celda1 = new PdfPCell(img);
                celda1.Border = 0;

                PdfPCell celda2 = new PdfPCell(new Paragraph(titulo, _standardFontG));
                celda2.Border = 0;
                celda2.HorizontalAlignment = 2;//0=Left, 1=Center, 2=Right

                tblEncabezado.AddCell(celda1);
                tblEncabezado.AddCell(celda2);
                doc.Add(tblEncabezado);
                doc.Add(new Chunk("\n"));
                doc.Add(Chunk.NEWLINE);

                //FECHA
                PdfPTable tblFecha = new PdfPTable(3);
                float[] anchotblfecha = new float[3];
                anchotblfecha[0] = 160;
                anchotblfecha[1] = 60;
                anchotblfecha[2] = 60;
                tblFecha.SetWidths(anchotblfecha);
                tblFecha.WidthPercentage = 100;
                tblFecha.AddCell(new PdfPCell(new Paragraph(""))).Border = 0;
                tblFecha.AddCell(new PdfPCell(new Paragraph("DATE", _standardFontN))).HorizontalAlignment = 1;
                tblFecha.AddCell(new PdfPCell(new Paragraph("P.O.N°", _standardFontN))).HorizontalAlignment = 1;

                //DETALLE FECHA
                tblFecha.AddCell(new PdfPCell(new Paragraph(""))).Border = 0;
                tblFecha.AddCell(new PdfPCell(new Paragraph(_Pedido.DateOrder, _standardFontC))).HorizontalAlignment = 1;
                tblFecha.AddCell(new PdfPCell(new Paragraph(_Pedido.PoNumber, _standardFontC))).HorizontalAlignment = 1;

                doc.Add(tblFecha);

                spacerParagraph.SpacingBefore = 10f;
                spacerParagraph.SpacingAfter = 0f;
                doc.Add(spacerParagraph);


                //VENDEDOR Y CONSIGNEE
                PdfPTable tblVendorConsignee = new PdfPTable(3);
                float[] anchotblVendorConsignee = new float[3];
                anchotblVendorConsignee[0] = 300;
                anchotblVendorConsignee[1] = 100;
                anchotblVendorConsignee[2] = 300;
                tblVendorConsignee.SetWidths(anchotblVendorConsignee);
                tblVendorConsignee.WidthPercentage = 100;

                tblVendorConsignee.AddCell(new PdfPCell(new Paragraph("VENDOR", _standardFontN))).HorizontalAlignment = 1;
                tblVendorConsignee.AddCell(new PdfPCell(new Paragraph(""))).Border = 0;
                tblVendorConsignee.AddCell(new PdfPCell(new Paragraph("CONSIGNEE", _standardFontN))).HorizontalAlignment = 1;

                tblVendorConsignee.AddCell(new PdfPCell(new Paragraph(_Pedido.VendorName, _standardFontP)) { Border = PdfPCell.RIGHT_BORDER | PdfPCell.LEFT_BORDER });
                tblVendorConsignee.AddCell(new PdfPCell(new Paragraph(""))).Border = 0;
                tblVendorConsignee.AddCell(new PdfPCell(new Paragraph(_Pedido.ConsigneeName, _standardFontP)) { Border = PdfPCell.RIGHT_BORDER | PdfPCell.LEFT_BORDER });

                tblVendorConsignee.AddCell(new PdfPCell(new Paragraph(_Pedido.VendorDir1, _standardFontP)) { Border = PdfPCell.RIGHT_BORDER | PdfPCell.LEFT_BORDER });
                tblVendorConsignee.AddCell(new PdfPCell(new Paragraph(""))).Border = 0;
                tblVendorConsignee.AddCell(new PdfPCell(new Paragraph(_Pedido.ConsigneeDir1, _standardFontC)) { Border = PdfPCell.RIGHT_BORDER | PdfPCell.LEFT_BORDER });

                tblVendorConsignee.AddCell(new PdfPCell(new Paragraph(_Pedido.VendorDir2, _standardFontP)) { Border = PdfPCell.RIGHT_BORDER | PdfPCell.LEFT_BORDER });
                tblVendorConsignee.AddCell(new PdfPCell(new Paragraph(""))).Border = 0;
                tblVendorConsignee.AddCell(new PdfPCell(new Paragraph(_Pedido.ConsigneeDir2, _standardFontC)) { Border = PdfPCell.RIGHT_BORDER | PdfPCell.LEFT_BORDER });


                tblVendorConsignee.AddCell(new PdfPCell(new Paragraph(_Pedido.VendorDir3, _standardFontP)) { Border = PdfPCell.RIGHT_BORDER | PdfPCell.LEFT_BORDER });
                tblVendorConsignee.AddCell(new PdfPCell(new Paragraph(""))).Border = 0;
                tblVendorConsignee.AddCell(new PdfPCell(new Paragraph(_Pedido.ConsigneeDir3, _standardFontC)) { Border = PdfPCell.RIGHT_BORDER | PdfPCell.LEFT_BORDER });

                if (tipo == 1)//PRODUCTION ORDER
                {
                    tblVendorConsignee.AddCell(new PdfPCell(new Paragraph(_Pedido.VendorRuc, _standardFontP)) { Border = PdfPCell.RIGHT_BORDER | PdfPCell.LEFT_BORDER });
                    tblVendorConsignee.AddCell(new PdfPCell(new Paragraph(""))).Border = 0;
                    tblVendorConsignee.AddCell(new PdfPCell(new Paragraph("", _standardFontN)) { Border = PdfPCell.RIGHT_BORDER | PdfPCell.LEFT_BORDER });
                    tblVendorConsignee.AddCell(new PdfPCell(new Paragraph("\n", _standardFontP)) { Border = PdfPCell.RIGHT_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.BOTTOM_BORDER });
                    tblVendorConsignee.AddCell(new PdfPCell(new Paragraph("\n"))).Border = 0;
                    tblVendorConsignee.AddCell(new PdfPCell(new Paragraph("\n", _standardFontC)) { Border = PdfPCell.RIGHT_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.BOTTOM_BORDER });
                }
                if ((tipo == 2) || (tipo == 3))//PROFORMA
                {
                    tblVendorConsignee.AddCell(new PdfPCell(new Paragraph(_Pedido.VendorRuc, _standardFontP)) { Border = PdfPCell.RIGHT_BORDER | PdfPCell.LEFT_BORDER });
                    tblVendorConsignee.AddCell(new PdfPCell(new Paragraph(""))).Border = 0;
                    tblVendorConsignee.AddCell(new PdfPCell(new Paragraph("SHIP TO", _standardFontN)) { Border = PdfPCell.RIGHT_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER }).HorizontalAlignment = 1;
                    tblVendorConsignee.AddCell(new PdfPCell(new Paragraph("", _standardFontP)) { Border = PdfPCell.RIGHT_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.BOTTOM_BORDER });
                    tblVendorConsignee.AddCell(new PdfPCell(new Paragraph(""))).Border = 0;
                    tblVendorConsignee.AddCell(new PdfPCell(new Paragraph(_Pedido.Shipto_Linea1 + " \n" + _Pedido.Shipto_Linea2 + " \n" + _Pedido.Shipto_Linea3 + " \n", _standardFontC)) { Border = PdfPCell.RIGHT_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.BOTTOM_BORDER }).HorizontalAlignment = 1; ;
                }

                doc.Add(tblVendorConsignee);

                spacerParagraph.Font = new Font(new Font(Font.FontFamily.HELVETICA, 5, Font.NORMAL, BaseColor.RED));
                spacerParagraph.SpacingBefore = 10f;
                spacerParagraph.SpacingAfter = 0f;
                doc.Add(spacerParagraph);

                //DISCHARGE Y CARRIER
                PdfPTable tbldischanger = new PdfPTable(4);
                float[] anchotbldischanger = new float[4];
                anchotbldischanger[0] = 150;
                anchotbldischanger[1] = 150;
                anchotbldischanger[2] = 100;
                anchotbldischanger[3] = 300;
                tbldischanger.SetWidths(anchotbldischanger);
                tbldischanger.WidthPercentage = 100;
                PdfPCell cellport = new PdfPCell(new Phrase("PORT OF DISCHARGE", _standardFontN));
                cellport.Colspan = 2;
                cellport.HorizontalAlignment = 1;
                tbldischanger.AddCell(cellport);
                tbldischanger.AddCell(new PdfPCell(new Paragraph(""))).Border = 0;

                if (tipo == 1)//PRODUCTION ORDER 
                {
                    tbldischanger.AddCell(new PdfPCell(new Paragraph("CARRIER", _standardFontN))).HorizontalAlignment = 1;
                }
                if ((tipo == 2) || (tipo == 3))//PROFORMA 
                {
                    tbldischanger.AddCell(new PdfPCell(new Paragraph("Forwarding Agent", _standardFontN))).HorizontalAlignment = 1;
                }

                PdfPCell cellport1 = new PdfPCell(new Phrase(new Paragraph(_Pedido.PortDischarge, _standardFontC)));
                //cellport1.FixedHeight = 75f;
                tbldischanger.AddCell(cellport1);

                PdfPCell cellport2 = new PdfPCell(new Phrase(new Paragraph(_Pedido.CountryPortDischarge, _standardFontC)));
                //cellport2.FixedHeight = 75f;
                cellport2.HorizontalAlignment = 1;
                tbldischanger.AddCell(cellport2);


                PdfPCell cellport3 = new PdfPCell(new Phrase(""));
                //cellport3.FixedHeight = 75f;
                cellport3.Border = 0;
                tbldischanger.AddCell(cellport3);


                //string _tira = _Pedido.CarrierName + " \n" + _Pedido.CarrierDir1 + " \n" + _Pedido.CarrierDir2 + " \n" + _Pedido.CarrierDir3;
                string _tira = String.Empty;
                if (tipo == 1)//PRODUCTION ORDER 
                {
                    _tira = _Pedido.CarrierName;
                }
                if ((tipo == 2) || (tipo == 3))//PROFORMA 
                {
                    _tira = _Pedido.Forward_Linea1 + " \n" + _Pedido.Forward_Linea2 + " \n" + _Pedido.Forward_Linea3 + " \n" + _Pedido.Forward_Linea4 + " \n" + _Pedido.Forward_Linea5 + " \n"
                        + _Pedido.Forward_Linea6 + " \n" + _Pedido.Forward_Linea7 + " \n" + _Pedido.Forward_Linea8;
                }

                PdfPCell cellport4 = new PdfPCell(new Phrase(new Paragraph(_tira, _standardFontC)));
                //cellport4.FixedHeight = 75f;
                tbldischanger.AddCell(cellport4).HorizontalAlignment = 1;
                doc.Add(tbldischanger);

                spacerParagraph.SpacingBefore = 10f;
                spacerParagraph.SpacingAfter = 0f;
                doc.Add(spacerParagraph);
                //doc.Add(Chunk.NEWLINE);

                //ITEMS
                PdfPTable tblitems = new PdfPTable(7);
                float[] anchotblitems = new float[7];
                anchotblitems[0] = 150;
                anchotblitems[1] = 300;
                anchotblitems[2] = 80;
                anchotblitems[3] = 80;
                anchotblitems[4] = 80;
                anchotblitems[5] = 80;
                anchotblitems[6] = 80;
                tblitems.SetWidths(anchotblitems);
                tblitems.WidthPercentage = 100;

                tblitems.AddCell(new PdfPCell(new Paragraph("ITEM", _smallFontC))).HorizontalAlignment = 1;
                tblitems.AddCell(new PdfPCell(new Paragraph("DESCRIPTION", _smallFontC))).HorizontalAlignment = 1;
                tblitems.AddCell(new PdfPCell(new Paragraph("QTY", _smallFontC))).HorizontalAlignment = 1;
                tblitems.AddCell(new PdfPCell(new Paragraph("PRICE EACH", _smallFontC))).HorizontalAlignment = 1;
                tblitems.AddCell(new PdfPCell(new Paragraph("DISCOUNT.", _smallFontC))).HorizontalAlignment = 1;
                tblitems.AddCell(new PdfPCell(new Paragraph("TAX.", _smallFontC))).HorizontalAlignment = 1;
                tblitems.AddCell(new PdfPCell(new Paragraph("AMOUNT", _smallFontC))).HorizontalAlignment = 1;

                Double acumulador = 0;
                Double acumuladorTax = 0;

                foreach (var item in _Detalle)
                {

                    tblitems.AddCell(new PdfPCell(new Paragraph(item.ItemCode, _smallFontC))).HorizontalAlignment = 1;
                    tblitems.AddCell(new PdfPCell(new Paragraph(item.ItemDescription, _smallFontC)));
                    tblitems.AddCell(new PdfPCell(new Paragraph(item.ItemQty, _smallFontC))).HorizontalAlignment = 2;
                    tblitems.AddCell(new PdfPCell(new Paragraph(double.Parse(item.ItemRate, CultureInfo.InvariantCulture).ToString("#,##0.0000"), _smallFontC))).HorizontalAlignment = 2;
                    tblitems.AddCell(new PdfPCell(new Paragraph(double.Parse(item.Discount, CultureInfo.InvariantCulture).ToString("#,##0.0000"), _smallFontC))).HorizontalAlignment = 2;
                    tblitems.AddCell(new PdfPCell(new Paragraph(double.Parse(item.Tax, CultureInfo.InvariantCulture).ToString("#,##0.0000"), _smallFontC))).HorizontalAlignment = 2;
                    tblitems.AddCell(new PdfPCell(new Paragraph(double.Parse(item.ItemAmount, CultureInfo.InvariantCulture).ToString("#,##0.00"), _smallFontC))).HorizontalAlignment = 2;
                    acumulador = acumulador + Double.Parse(item.ItemAmount, CultureInfo.InvariantCulture);
                    acumuladorTax = acumuladorTax + Double.Parse(item.Tax, CultureInfo.InvariantCulture);
                }

                doc.Add(tblitems);


                //TOTALES
                PdfPTable tblTotales = new PdfPTable(7);
                float[] anchotblTotales = new float[7];
                anchotblTotales[0] = 150;
                anchotblTotales[1] = 300;
                anchotblTotales[2] = 80;
                anchotblTotales[3] = 80;
                anchotblTotales[4] = 80;
                anchotblTotales[5] = 80;
                anchotblTotales[6] = 80;
                tblTotales.SetWidths(anchotblTotales);
                tblTotales.WidthPercentage = 100;
                tblTotales.AddCell(new PdfPCell(new Paragraph("", _smallFontC)) { Border = 0 });
                tblTotales.AddCell(new PdfPCell(new Paragraph("TOTAL", _smallFontC)) { Border = 0, HorizontalAlignment = 2 });
                tblTotales.AddCell(new PdfPCell(new Paragraph(_Pedido.TotalQty, _smallFontC)) { Border = 0, HorizontalAlignment = 2 });
                tblTotales.AddCell(new PdfPCell(new Paragraph("", _smallFontC)) { Border = 0, HorizontalAlignment = 2 });
                tblTotales.AddCell(new PdfPCell(new Paragraph("VAT " + _Pedido.VAT + "%", _smallFontC)) { Border = 0, HorizontalAlignment = 2 });
                tblTotales.AddCell(new PdfPCell(new Paragraph(acumuladorTax.ToString("#,##0.0000"), _smallFontC)) { Border = 0, HorizontalAlignment = 2 });
                tblTotales.AddCell(new PdfPCell(new Paragraph(_Pedido.Moneda + "  " + acumulador.ToString("#,##0.00"), _smallFontC)) { Border = 0, HorizontalAlignment = 2 });
                //tblTotales.AddCell(new PdfPCell(new Paragraph("", _smallFontC)) { Border = 0, HorizontalAlignment = 2 });
                doc.Add(tblTotales);
                doc.Add(Chunk.NEWLINE);

                //COMENTARIOS
                PdfPTable tblcoments = new PdfPTable(2);
                float[] anchotblcoments = new float[2];
                anchotblcoments[0] = 300;
                anchotblcoments[1] = 300;
                tblcoments.SetWidths(anchotblcoments);
                tblcoments.WidthPercentage = 100;

                if (tipo == 1)//Production Order
                {
                    tblcoments.AddCell(new PdfPCell(new Paragraph("COMMENTS: " + _Pedido.Comments, _standardFontN)) { Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER });
                    tblcoments.AddCell(new PdfPCell(new Paragraph("")) { Border = PdfPCell.TOP_BORDER | PdfPCell.RIGHT_BORDER });
                    tblcoments.AddCell(new PdfPCell(new Paragraph("PAYMENT: " + _Pedido.Payment, _standardFontN)) { Border = PdfPCell.LEFT_BORDER });
                    tblcoments.AddCell(new PdfPCell(new Paragraph("Lead time: " + _Pedido.LeadTime, _standardFontN)) { Border = PdfPCell.RIGHT_BORDER, HorizontalAlignment = 1 });

                    tblcoments.AddCell(new PdfPCell(new Paragraph("ETA: " + _Pedido.Eta, _standardFontN)) { Border = PdfPCell.LEFT_BORDER });
                    tblcoments.AddCell(new PdfPCell(new Paragraph("Fecha corte: " + _Pedido.FechaCorte, _standardFontN)) { Border = PdfPCell.RIGHT_BORDER, HorizontalAlignment = 1 });
                    tblcoments.AddCell(new PdfPCell(new Paragraph("Order: " + _Pedido.Order, _standardFontN)) { Border = PdfPCell.LEFT_BORDER });
                    tblcoments.AddCell(new PdfPCell(new Paragraph("Motivo de activación: " + _Pedido.MotivoReactivacion, _standardFontN)) { Border = PdfPCell.RIGHT_BORDER, HorizontalAlignment = 1 });
                    tblcoments.AddCell(new PdfPCell(new Paragraph("PRICE LIST:" + _Pedido.ListaPrecio, _standardFontN)) { Border = PdfPCell.LEFT_BORDER });
                    tblcoments.AddCell(new PdfPCell(new Paragraph("# Orden Original: " + _Pedido.IdOrderOriginal, _standardFontN)) { Border = PdfPCell.RIGHT_BORDER, HorizontalAlignment = 1 });
                    tblcoments.AddCell(new PdfPCell(new Paragraph("MTO:", _standardFontN)) { Border = PdfPCell.LEFT_BORDER });
                    tblcoments.AddCell(new PdfPCell(new Paragraph("# Revisión: " + _Pedido.NumeroRevision, _standardFontN)) { Border = PdfPCell.RIGHT_BORDER, HorizontalAlignment = 1 });

                    tblcoments.AddCell(new PdfPCell(new Paragraph("Incoterm: " + _Pedido.Incoterm, _standardFontN)) { Border = PdfPCell.LEFT_BORDER });
                    tblcoments.AddCell(new PdfPCell(new Paragraph("")) { Border = PdfPCell.RIGHT_BORDER });

                    tblcoments.AddCell(new PdfPCell(new Paragraph("")) { Border = PdfPCell.LEFT_BORDER | PdfPCell.BOTTOM_BORDER });
                    tblcoments.AddCell(new PdfPCell(new Paragraph("")) { Border = PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER });

                }
                if ((tipo == 2) || (tipo == 3))//Proforma y Proforma POS
                {
                    tblcoments.AddCell(new PdfPCell(new Paragraph("COMMENTS: " + _Pedido.Comments, _standardFontN)) { Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER });
                    tblcoments.AddCell(new PdfPCell(new Paragraph("")) { Border = PdfPCell.TOP_BORDER | PdfPCell.RIGHT_BORDER });
                    tblcoments.AddCell(new PdfPCell(new Paragraph("PAYMENT: " + _Pedido.Payment, _standardFontN)) { Border = PdfPCell.LEFT_BORDER });
                    tblcoments.AddCell(new PdfPCell(new Paragraph("")) { Border = PdfPCell.RIGHT_BORDER });
                    tblcoments.AddCell(new PdfPCell(new Paragraph("ETA: " + _Pedido.Eta, _standardFontN)) { Border = PdfPCell.LEFT_BORDER });
                    tblcoments.AddCell(new PdfPCell(new Paragraph("")) { Border = PdfPCell.RIGHT_BORDER });
                    if (tipo == 2) // Solo Proforma 
                    {
                        tblcoments.AddCell(new PdfPCell(new Paragraph("Order: " + _Pedido.Order, _standardFontN)) { Border = PdfPCell.LEFT_BORDER });
                        tblcoments.AddCell(new PdfPCell(new Paragraph("")) { Border = PdfPCell.RIGHT_BORDER });
                    }
                    tblcoments.AddCell(new PdfPCell(new Paragraph("INCOTERMS: " + _Pedido.Incoterm, _standardFontN)) { Border = PdfPCell.LEFT_BORDER | PdfPCell.BOTTOM_BORDER });
                    tblcoments.AddCell(new PdfPCell(new Paragraph("")) { Border = PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER });
                }

                doc.Add(tblcoments);
                doc.Add(Chunk.NEWLINE);

                //TELEFONO/FAX
                //PdfPTable tblphonefax = new PdfPTable(3);
                //float[] anchotblphonefax = new float[3];
                //anchotblphonefax[0] = 100;
                //anchotblphonefax[1] = 100;
                //anchotblphonefax[2] = 100;
                //tblphonefax.SetWidths(anchotblphonefax);
                //tblphonefax.WidthPercentage = 60;
                //tblphonefax.HorizontalAlignment = 0;
                //tblphonefax.AddCell(new PdfPCell(new Paragraph("Phone #", _standardFontC)) { HorizontalAlignment = 1 });
                //tblphonefax.AddCell(new PdfPCell(new Paragraph("Fax #", _standardFontC)) { HorizontalAlignment = 1 });
                //tblphonefax.AddCell(new PdfPCell(new Paragraph("", _standardFontC)) { HorizontalAlignment = 1 });
                //tblphonefax.AddCell(new PdfPCell(new Paragraph(_Pedido.Phone)) { FixedHeight = 20f });
                //tblphonefax.AddCell(new PdfPCell(new Paragraph(_Pedido.Fax)) { FixedHeight = 20f });
                //tblphonefax.AddCell(new PdfPCell(new Paragraph("")) { FixedHeight = 20f });
                //doc.Add(tblphonefax);


                //PIE DE PAGINA
                PdfPTable tblfooter = new PdfPTable(2);
                float[] anchotblfooter = new float[2];
                anchotblfooter[0] = 300;
                anchotblfooter[1] = 300;
                tblfooter.SetWidths(anchotblfooter);
                tblfooter.WidthPercentage = 100;
                tblfooter.AddCell(new PdfPCell(new Paragraph(_Pedido.EmpresaNombre, _standardFontP)) { Border = 0 });
                tblfooter.AddCell(new PdfPCell(new Paragraph("")) { Border = 0 });
                tblfooter.AddCell(new PdfPCell(new Paragraph(_Pedido.EmpresaDir1, _standardFontP)) { Border = 0 });
                tblfooter.AddCell(new PdfPCell(new Paragraph("")) { Border = 0 });
                tblfooter.AddCell(new PdfPCell(new Paragraph(_Pedido.EmpresaDir2, _standardFontP)) { Border = 0 });
                tblfooter.AddCell(new PdfPCell(new Paragraph("")) { Border = 0 });
                tblfooter.AddCell(new PdfPCell(new Paragraph(_Pedido.EmpresaDir3, _standardFontP)) { Border = 0 });
                tblfooter.AddCell(new PdfPCell(new Paragraph("")) { Border = 0 });
                tblfooter.AddCell(new PdfPCell(new Paragraph(_Pedido.EmpresaDir4, _standardFontP)) { Border = 0 });
                tblfooter.AddCell(new PdfPCell(new Paragraph("")) { Border = 0 });
                tblfooter.AddCell(new PdfPCell(new Paragraph(_Pedido.EmpresaRuc, _standardFontP)) { Border = 0 });
                tblfooter.AddCell(new PdfPCell(new Paragraph("")) { Border = 0 });
                doc.Add(tblfooter);

                doc.Close();
                bytes = memoryStream.ToArray();
                memoryStream.Close();

            }
            string path = _env.WebRootPath + _configVariables.PedidosPDF; //Server.MapPath(ConfigurationManager.AppSettings["PedidosPDF"].ToString());
            if (tipo == 1)
            {
                System.IO.File.WriteAllBytes(@path + "\\" + _Pedido.PoNumber + ".pdf", bytes);
            }
            else
            if ((tipo == 2) || (tipo == 3))
            {
                System.IO.File.WriteAllBytes(@path + "\\" + _Pedido.PoNumber + "-PROFORMA.pdf", bytes);
            }
            return bytes;
        }

        [HttpGet()]
        public async Task<IActionResult> CargarOpcionesF0005()
        {
            MotivosF0005 Model = new MotivosF0005();
            try
            {
                Model.Motivos = new CestaRepository(_configVariables).ObtenerF0005("55", "RC").Result;
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpGet()-CargarOpcionesF0005()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return PartialView("_CargarOpcionesF0005", Model);
        }

        [HttpPost]
        public JsonResult Eliminar(int IdOrden, string IdMotivo)
        {
            bool _result = false;
            try
            {
                _result = new PedidoRepository(_configVariables).EliminarPedido(IdOrden, IdMotivo, int.Parse(HttpContext.Session.GetString("idusuario"))).Result;

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpPost-Eliminar()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _result });
        }

        [HttpGet()]
        public async Task<IActionResult> CargarOpcionesReactivacion()
        {
            MotivosF0005 Model = new MotivosF0005();
            try
            {
                Model.Motivos = new CestaRepository(_configVariables).ObtenerF0005("55", "AC").Result;
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpGet()-CargarOpcionesReactivacion()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return PartialView("_CargarOpcionesReactivacion", Model);
        }

        [HttpPost]
        public JsonResult ActivarPedido(int IdOrden, string IdMotivo)
        {
            int _result = 0;
            int _IdOrdenReactivada = 0;
            bool _resultemail = false;
            try
            {
                var _resultQuery = new PedidoRepository(_configVariables).ReactivarPedido(IdOrden, IdMotivo, int.Parse(HttpContext.Session.GetString("idusuario"))).Result;
                foreach (var item in _resultQuery)
                {
                    switch (item.Key)
                    {
                        case "@IDORDENOUT":
                            _IdOrdenReactivada = Convert.ToInt32(item.Value);
                            break;
                        case "@RESULTADO":
                            _result = Convert.ToInt32(item.Value);
                            break;
                        default:
                            break;
                    }
                }

                /*

                if (_result == 1)
                {

                    List<PedidoPDFViewModel> _Detalle = new List<PedidoPDFViewModel>();
                    PedidoPDFViewModel _Pedido = new PedidoPDFViewModel();

                    //LLENADO DEL MODELO
                    _Detalle = new PedidoRepository().ConsultarOrdenPdf(IdOrden);
                    _Pedido = _Detalle.FirstOrDefault();

                    _resultemail = new EmailRepository().SendMailSalesmen(IdOrden, Session["login"].ToString(), _Pedido.DateOrder, _Pedido.TotalAmount, Convert.ToInt32(HttpContext.Session.GetString("idusuario")));
                }

    */



            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpPost-Eliminar()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _result, idOrd = _IdOrdenReactivada });
        }

        [HttpPost]
        public JsonResult EnviarProforma(int IdOrden)
        {
            int _result = 0;
            try
            {
                var _resultQuery = new PedidoRepository(_configVariables).AsignarPoNumber(IdOrden);
                byte[] bytes;
                bytes = ConstruirPDF(IdOrden, "PROFORMA", 2);
                _resultQuery = new PedidoRepository(_configVariables).EnviarProforma(IdOrden);
                _result = 1;

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpPost-Eliminar()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _result });
        }

        [HttpPost()]
        public JsonResult RefrescarPreciosPedido(int IdOrden)
        {
            bool _result = false;
            try
            {
                _result = new PedidoRepository(_configVariables).RecalcularPrecioOrden(IdOrden, int.Parse(HttpContext.Session.GetString("idusuario"))).Result;
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory, "PedidoController", "HttpPost-Eliminar()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _result });
        }
    }
}
