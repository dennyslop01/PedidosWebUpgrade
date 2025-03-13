using Microsoft.AspNetCore.Mvc;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.Repository;
using PedidosWebUpgrade.Infrastructure.Utilities;
using Shyjus.BrowserDetection;

namespace PedidosWebUpgrade.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly ConfigVariables _configVariables;
        private readonly IBrowserDetector _browserDetector;

        public LoginController(ConfigVariables configVariables, IBrowserDetector browserDetector)
        {
            _configVariables = configVariables;
            _browserDetector = browserDetector;
        }
        
        [HttpGet()]
        public ActionResult IniciarSesion()
        {
            //CONSULTAR DATOS DE LA EMPRESA
            HttpContext.Session.SetString("empresa", new EmpresaRepository(_configVariables).ObtenerEmpresa().FirstOrDefault().NombreCorto);

            return View(new UsuarioLogin());
        }

        [HttpPost()]
        public ActionResult IniciarSesion(UsuarioLogin Model)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    int _resultado = 0;
                    int _idUsuario = 0;
                    string _motivo = string.Empty;
                    string _Idvendedor = string.Empty;


                    var _resultQuery = new UsuarioRepository(_configVariables).ValidarUsuario(Model.Cuenta.Trim(), Model.Clave.Trim());
                    foreach (var item in _resultQuery)
                    {
                        switch (item.Key)
                        {
                            case "@RESULTADO":
                                _resultado = Convert.ToInt32(item.Value);
                                break;
                            case "@IDUSUARIOOUT":
                                _idUsuario = Convert.ToInt32(item.Value);
                                break;
                            case "@MOTIVOOUT":
                                _motivo = Convert.ToString(item.Value);
                                break;
                            case "@IDVENDEDOROUT":
                                _Idvendedor = Convert.ToString(item.Value);
                                break;
                            default:
                                break;
                        }
                    }

                    switch (_resultado)
                    {
                        case 0:
                            ModelState.AddModelError(string.Empty, "Lo Sentimos, ha ocurrido un error!");
                            break;
                        case 1:
                            //CREACION VARIABLES DE SESIÓN
                            HttpContext.Session.SetString("idusuario", _idUsuario.ToString());
                            HttpContext.Session.SetString("idvendedor", _Idvendedor.ToString());

                            //OBTENER DATOS DEL USUARIO-- 
                            Usuario _Usuario = new UsuarioRepository(_configVariables).ObtenerUsuario(_idUsuario).FirstOrDefault();

                            //ASIGNAR DATOS  DEL USUARIO EN SESSION
                            HttpContext.Session.SetString("nombreusuario", _Usuario.PrimerApellido);
                            HttpContext.Session.SetString("login", _Usuario.Login);
                            HttpContext.Session.SetString("tipousuario", _Usuario.TipoUsuario);

                            //CONSULTAR DATOS DE LA EMPRESA
                            HttpContext.Session.SetString("empresa", new EmpresaRepository(_configVariables).ObtenerEmpresa().FirstOrDefault().NombreCorto);

                            //FormsAuthentication.SetAuthCookie(Model.Cuenta, true);
                            return RedirectToAction("Inicio", "Principal");
                        case 2:
                            ModelState.AddModelError(string.Empty, "USUARIO NO EXISTE!");
                            break;
                        case 3:
                            ModelState.AddModelError(string.Empty, "CONTRASEÑA INVÁLIDA!");
                            break;
                        case 4:
                            ModelState.AddModelError(string.Empty, "SISTEMA FUERA DE LÍNEA " + (!string.IsNullOrEmpty(_motivo) ? "- MOTIVO: " + _motivo.ToUpper() : ""));
                            break;
                        default:
                            break;
                    }
                }
                ;
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, "Lo Sentimos, ha ocurrido un error!");
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"LoginController", "HttpPost-IniciarSesion", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            ;
            return View(Model);
        }

        /// <summary>
        /// GET: Cerrar sesión
        /// </summary>
        /// <returns>View("Login")</returns>
        [HttpGet()]
        public ActionResult Cerrar()
        {
            HttpContext.Session.Clear();
            Response.Clear();
            //FormsAuthentication.SignOut();
            return View("IniciarSesion");
        }
    }
}
