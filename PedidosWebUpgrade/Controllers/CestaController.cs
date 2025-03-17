using Microsoft.AspNetCore.Mvc;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.Repository;
using PedidosWebUpgrade.Infrastructure.Utilities;
using Shyjus.BrowserDetection;

namespace PedidosWebUpgrade.Web.Controllers
{
    public class CestaController : Controller
    {
        private readonly ConfigVariables _configVariables;
        private readonly IBrowserDetector _browserDetector;

        public CestaController(ConfigVariables configVariables, IBrowserDetector browserDetector)
        {
            _configVariables = configVariables;
            _browserDetector = browserDetector;
        }
        
        [HttpGet()]
        public IActionResult Detalle(int IdOrden, string IdCli, string IdSuc, string IdCat, int back = 0, int TipoPedido = 1)
        {
            CestaViewModel Modelo = new CestaViewModel();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "Cesta/Detalle");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                TempData["CodCli"] = IdCli;
                TempData["CodSuc"] = IdSuc;
                TempData["CodCat"] = IdCat;
                TempData["UrlBack"] = back;

                Modelo.Condicion = new CestaRepository(_configVariables).ObtenerCondiciones();
                Modelo.Descuento = new CestaRepository(_configVariables).ObtenerPorcentajes();
                Modelo.Encabezado = new PedidoRepository(_configVariables).ObtenerEncabezadoOrden(IdOrden, IdCli);
                Modelo.Encabezado.TotalDescuento = Modelo.Encabezado.TotalDescuento;
                List<Customer> _Clientes = new ClienteRepository(_configVariables).ObtenerClientes((string)HttpContext.Session.GetString("idvendedor"), IdSuc, 2).Where(x => x.CustomerId == IdCli).ToList();
                Modelo.NombreCliente = _Clientes.FirstOrDefault().Name;
                ViewBag.DiasRequerido = _Clientes.FirstOrDefault().DiasRequerido;

                Modelo.Carriers = new CestaRepository(_configVariables).ObtenerF0005("55", "CR");
                Modelo.PuertosDescargas = new CestaRepository(_configVariables).ObtenerF0005("55", "PS");
                Modelo.IncosTerms = new CestaRepository(_configVariables).ObtenerF0005("42", "FR");
                Modelo.Referencias01 = new CestaRepository(_configVariables).ObtenerF0005("00", "00");
                Modelo.ModalidadTransporte = new CestaRepository(_configVariables).ObtenerF0005("00", "TM");
                Modelo.ForwardingAgents = new PedidoRepository(_configVariables).ConsultarForwardingAgent();
                Modelo.PaisesFacturacion = new PedidoRepository(_configVariables).ConsultarPaisesFacturacion(IdCli);
                Modelo.CodigoCliente = IdCli;

                //datos del pedido
                Modelo.Pedido = new PedidoRepository(_configVariables).ConsultarPedidos("", "", "", IdOrden, "", Convert.ToInt32(HttpContext.Session.GetString("idusuario"))).FirstOrDefault();
                Modelo.Productos = new CestaRepository(_configVariables).ObtenerProductosCesta(IdOrden, IdCli, Modelo.Pedido.ListaPrecio ?? string.Empty);
                Modelo.CodigoModTransportte = Modelo.Pedido.CodigoModTransporte;
                Modelo.CodigoForwardingAgent = Modelo.Pedido.IdAgente.ToString();
                Modelo.Observaciones = Modelo.Pedido.Comments;
                Modelo.OrderPrint = Modelo.Pedido.OrdenPrint;
                Modelo.FechaPedido = Modelo.Pedido.OrderDate;
                Modelo.LeadTime = Modelo.Pedido.LeadTime;
                Modelo.CabecListaPrecios = new ListaPreciosRepository(_configVariables).ConsultarCabeceraListoPrecio().Select(x => new ListaGeneral() { Codigo = x.Codigo.Trim(), Descripcion = x.Descripcion.Trim() }).ToList();
                Modelo.CondicionesDePago = new PedidoRepository(_configVariables).ConsultarCondicionesDePago().Select(x => new ListaGeneral() { Codigo = x.Codigo.Trim(), Descripcion = x.Codigo.Trim() + " - " + x.Descripcion.Trim() }).ToList();
                Modelo.ShipTo = new ClienteRepository(_configVariables).ObtenerShipTo(null, null, 2, null).Select(x => new ListaGeneral() { Codigo = x.CustomerId, Descripcion = x.CustomerId + " - " + x.Name }).ToList();
                Modelo.Almacenes = new AlmacenRepository(_configVariables).ConsultarAlmacenes(0).Select(x => new ListaGeneral { Codigo = x.IdAlmacen, Descripcion = x.IdAlmacen + " - " + x.Descripcion }).ToList();
                Modelo.Codigoalmacent = Modelo.Pedido.Warehouse;
                Modelo.CodigoPaisFacturacion = Modelo.Pedido.CodigoPaisFacturacion;
                Modelo.CodigoAnnocorrelativo = Modelo.Pedido.CodigoAnnoCorrelativo;
                Modelo.TipoPedido = TipoPedido;

                //El PEDIDO NO HA SIDO PROCESADO
                if (Modelo.Pedido.Estatus == "TRA")
                {
                    //FECHAS ESTIMADAS
                    List<FechasEstimadas> _ListFechasEstimadas = new CestaRepository(_configVariables).ObtenerFechasEstimadas(IdOrden);
                    Modelo.FechaRequerida = (Modelo.Pedido.RequiredDate == "" && _ListFechasEstimadas.Count > 0 ? _ListFechasEstimadas.FirstOrDefault().FechaRequerida.ToString("yyyyMMdd") : Modelo.Pedido.RequiredDate);
                    Modelo.FechaCorte = (_ListFechasEstimadas.FirstOrDefault().FechaCorte != null) ? _ListFechasEstimadas.FirstOrDefault().FechaCorte.ToString("yyyyMMdd") : string.Empty;
                    Modelo.TiempoLlegada = _ListFechasEstimadas.FirstOrDefault().LeadTime.ToString();
                    Modelo.LeadTime = _ListFechasEstimadas.FirstOrDefault().LeadTime.ToString();
                    Modelo.FechaEstimadaDespachoETD = (_ListFechasEstimadas.FirstOrDefault().FechaEstimadaDespechoETD != null) ? _ListFechasEstimadas.FirstOrDefault().FechaEstimadaDespechoETD.ToString("yyyyMMdd") : string.Empty;
                    Modelo.FechaEstimadaLlegadaETA = (_ListFechasEstimadas.FirstOrDefault().FechaEstimadaLlegadaETA != null) ? _ListFechasEstimadas.FirstOrDefault().FechaEstimadaLlegadaETA.ToString("yyyyMMdd") : string.Empty;
                    Modelo.CodigoCondicionPago = _Clientes.FirstOrDefault().CodCondicionPago.Trim();

                    if (Modelo.PaisesFacturacion.Count > 0)
                    {
                        Modelo.AnnosCorrelativos = new PedidoRepository(_configVariables).ConsultarContadoresAnnoPais(Modelo.PaisesFacturacion.FirstOrDefault().Codigo).Select(x => new ListaGeneral() { Codigo = x.IdTipo.ToString(), Descripcion = x.Descripcion.Trim() }).ToList();
                    }
                    else
                    {
                        //NUNCA DEBERIA ENTRAR AQUI
                        Modelo.AnnosCorrelativos = new PedidoRepository(_configVariables).ConsultarContadoresAnnoPais(null).Select(x => new ListaGeneral() { Codigo = x.IdTipo.ToString(), Descripcion = x.Descripcion.Trim() }).ToList();
                    }
                }
                else
                {
                    Modelo.FechaRequerida = DateTime.Parse(Modelo.Pedido.RequieredDate).ToString("yyyyMMdd");
                    Modelo.FechaCorte = Modelo.Pedido.CutDate;
                    Modelo.TiempoLlegada = Modelo.Pedido.TiempoLlegada;
                    Modelo.LeadTime = Modelo.Pedido.LeadTime;
                    Modelo.FechaEstimadaDespachoETD = DateTime.Parse(Modelo.Pedido.EtdDate).ToString("yyyyMMdd");
                    Modelo.FechaEstimadaLlegadaETA = DateTime.Parse(Modelo.Pedido.EtaDate).ToString("yyyyMMdd");
                    Modelo.CodigoCondicionPago = Modelo.Pedido.IdCondicionPago;
                    Modelo.CodigoAnnocorrelativo = Modelo.Pedido.CodigoAnnoCorrelativo;
                    Modelo.AnnosCorrelativos = new PedidoRepository(_configVariables).ConsultarContadoresAnnoPais(Modelo.Pedido.CodigoPaisFacturacion).Select(x => new ListaGeneral() { Codigo = x.IdTipo.ToString(), Descripcion = x.Descripcion.Trim() }).ToList();
                }


            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"CestaController", "HttpPost-ProcesarOrden()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }

        [HttpPost()]
        public JsonResult VaciarCesta(int IdOrden)
        {
            bool _result = false;
            try
            {
                _result = new CestaRepository(_configVariables).VaciarCesta(IdOrden);

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"CestaController", "HttpGet-VaciarCesta()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _result });
        }

        [HttpPost]
        public JsonResult EliminarProductoCesta(int IdOrden, string CodProd, string CodCli)
        {
            bool _result = false;
            EncabezadoPedido _Encabezado = new EncabezadoPedido();
            try
            {
                _result = new CestaRepository(_configVariables).EliminarProducto(IdOrden, CodProd);
                _Encabezado = new PedidoRepository(_configVariables).ObtenerEncabezadoOrden(IdOrden, CodCli);

            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"CestaController", "HttpPost-EliminarProductoCesta()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return Json(new { result = _result, encabezado = _Encabezado });
        }
    }
}
