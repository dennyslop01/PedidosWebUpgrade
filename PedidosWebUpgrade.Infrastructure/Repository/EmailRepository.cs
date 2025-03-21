using PedidosWebUpgrade.Application.Common.Interfaces;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Infrastructure.DbContext;
using PedidosWebUpgrade.Infrastructure.Utilities;
using System.Net;
using System.Net.Mail;

namespace PedidosWebUpgrade.Infrastructure.Repository
{
    public class EmailRepository: IEmailRepository
    {
        private readonly ConfigVariables _configVariables;

        public EmailRepository(ConfigVariables configVariables)
        {
            _configVariables = configVariables;
        }

        public async Task<Email> ObtenerEmail(int Id)
        {
            lDato _ldato = new lDato(_configVariables);
            List<Email> _Email = [];
            DataResponse<List<Email>> List_Response = new DataResponse<List<Email>>();
            try
            {

                _ldato.Parametros.Add("@ID", Id);
                _ldato.Esquema.Add("Subject", "TEMA");
                _ldato.Esquema.Add("Body", "CUERPO");
                _ldato.Esquema.Add("footer", "COLETILLA_FINAL");

                List_Response = await _ldato.EjecutarReader(new Email(), "PED_USP_CONSULTARGESTIONCORREO", _ldato.Parametros, _ldato.Esquema);
                _Email = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"EmailRepository", "ObtenerEmail()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARGESTIONCORREO]");
                throw new ApplicationException("ERROR EN: ObtenerEmail()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Email.FirstOrDefault();

        }

        public async Task<Email> ObtenerEmailDescuento(int Id)
        {
            lDato _ldato = new lDato(_configVariables);
            List<Email> _Email = [];
            DataResponse<List<Email>> List_Response = new DataResponse<List<Email>>();
            try
            {
                _ldato.Parametros.Add("@ID", Id);
                _ldato.Esquema.Add("To", "DESTINATARIOS");
                _ldato.Esquema.Add("Subject", "SUBJECT");
                _ldato.Esquema.Add("Body", "BODY");
                _ldato.Esquema.Add("footer", "COLETILLA_FINAL");

                List_Response = await _ldato.EjecutarReader(new Email(), "PED_USP_CONSULTARGESTIONDESCUENTO", _ldato.Parametros, _ldato.Esquema);
                _Email = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"EmailRepository", "ObtenerEmailDescuento()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARGESTIONDESCUENTO]");
                throw new ApplicationException("ERROR EN: ObtenerEmailDescuento()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Email.FirstOrDefault();

        }

        public async Task<bool> SendMail(Email EmailModel)
        {
            MailMessage _EmailMessage = new MailMessage();
            try
            {
                _EmailMessage.To.Add(new MailAddress(EmailModel.To));
                _EmailMessage.From = new MailAddress(EmailModel.From);
                _EmailMessage.Subject = EmailModel.Subject;
                _EmailMessage.Body = EmailModel.Body;

                if (!string.IsNullOrEmpty(EmailModel.CopyTo))
                {
                    _EmailMessage.CC.Add(new MailAddress(EmailModel.CopyTo));
                }

                _EmailMessage.IsBodyHtml = true;

                SmtpClient clienteSmtp = new SmtpClient(EmailModel.Host, EmailModel.Port);
                clienteSmtp.EnableSsl = false;
                if (EmailModel.UseSSL == "true")
                {
                    clienteSmtp.EnableSsl = true;
                }

                clienteSmtp.UseDefaultCredentials = false;
                if (EmailModel.UseSSL == "true")
                {
                    clienteSmtp.UseDefaultCredentials = true;
                    clienteSmtp.Credentials = new NetworkCredential(EmailModel.Login, EmailModel.Password);
                }

                clienteSmtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                await clienteSmtp.SendMailAsync(_EmailMessage);
                return true;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"EmailRepository", "SendMail()", ex.Message.ToString(), null, null, null);
                throw new ApplicationException("ERROR EN: EmailRepository-SendMail()", ex);
            }

        }

        public async Task<bool> SendMailSalesmen(int Idpedido, string IdSalesman, string fechapedido, string montototal, int IdUsuario)
        {
            List<Salesman> _ListSalesman = new List<Salesman>();
            Email EmailModel = new Email();
            bool _result = false;
            try
            {
                //BUSCAR DATOS DEL SALESMEN
                _ListSalesman = await new UsuarioRepository(_configVariables).ObtenerSalesman(IdSalesman);
                Salesman _Salesman = _ListSalesman.FirstOrDefault();

                //BUSCAR PARAMETROS DE CONFIGURACION PARA ENVIO DE EMAIL
                List<Configuracion> _parametros = await new ConfiguracionRepository(_configVariables).ObtenerParametros("portal.general.correo");

                //OBTENER EMAIL
                EmailModel = await ObtenerEmail(1);
                EmailModel.To = _Salesman.Mail;
                EmailModel.CopyTo = _Salesman.MailCoordinador;


                //CONSULTAR DATOS DEL PEDIDO
                List<Orders> _Pedido = await new PedidoRepository(_configVariables).ConsultarPedidos("", "", "", Idpedido, "", IdUsuario);

                EmailModel.Subject = EmailModel.Subject.Replace("[PEDIDO]", _Pedido.FirstOrDefault().OrderNumber).
                                                        Replace("[CODCLI]", _Pedido.FirstOrDefault().CustomerId).
                                                        Replace("[NOMCLI]", _Pedido.FirstOrDefault().Cliente);

                string montoDescuento = _Pedido.FirstOrDefault().DiscountTotal.ToString("#,##0.00");
                string montoTotal = _Pedido.FirstOrDefault().AmountTotal.ToString("#,##0.00");


                EmailModel.Body = EmailModel.Body.Replace("[USUARIO]", IdSalesman).
                                                  Replace("[NOMUSU]", _Salesman.Name).
                                                  Replace("[CODCLI]", _Pedido.FirstOrDefault().CustomerId).
                                                  Replace("[NOMCLI]", _Pedido.FirstOrDefault().Cliente).
                                                  Replace("[PEDIDO]", _Pedido.FirstOrDefault().OrderNumber).
                                                  Replace("[COMENTARIO]", _Pedido.FirstOrDefault().Comments).
                                                  Replace("[FECHA]", fechapedido).
                                                  Replace("[HORA]", _Pedido.FirstOrDefault().OrderTime).
                                                  Replace("[DESCUENTO]", montoDescuento).
                                                  Replace("[MONTO]", montoTotal).
                                                  Replace("[BR]", "<br />").
                                                  Replace("[B]", "<b>").
                                                  Replace("[/B]", "</b>");


                //DETALLE DEL PEDIDO
                string _detpedido = "<table><tr><td colspan = '5' style='padding-bottom:15px;'><b>Detalle del pedido:</b></td></tr>";
                string _cadena = string.Empty;
                foreach (var item in _Pedido)
                {
                    _cadena = "<tr style='padding:5px'><td style='padding-left:10px; padding-right:10px;'>" + item.CodigoProducto + "</td>" +
                              "<td style='padding-left:10px; padding-right:10px;'>" + item.Descripcion + "</td>" +
                              "<td style='padding-left:10px; padding-right:10px;'>" + item.Unit + "</td>" +
                              "<td style='padding-left:10px; padding-right:10px;'>" + item.Qty + "</td>" +
                              "<td style='padding-left:10px; padding-right:10px;'>" + item.Precio.ToString("#,##0") + "</td></tr>";

                    _detpedido = _detpedido + _cadena;

                }
                _detpedido = _detpedido + "</table><br/>";

                EmailModel.Body = EmailModel.Body + _detpedido;

                EmailModel.Body = EmailModel.Body + EmailModel.footer.Replace("[BR]", "<br />");


                EmailModel.Host = _parametros.Find(x => x.Codigo == "HOSTMAIL").Valor;
                EmailModel.Port = int.Parse(_parametros.Find(x => x.Codigo == "PORTHOSTEMAIL").Valor);
                EmailModel.Login = _parametros.Find(x => x.Codigo == "LOGINMAIL").Valor;
                EmailModel.Password = _parametros.Find(x => x.Codigo == "PASSWORDMAIL").Valor;
                EmailModel.From = _parametros.Find(x => x.Codigo == "EMAILEMPRESA").Valor;
                EmailModel.UseSSL = _parametros.Find(x => x.Codigo == "USESSL").Valor;
                EmailModel.UseAuth = _parametros.Find(x => x.Codigo == "USEAUTH").Valor;

                _result = await SendMail(EmailModel);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"EmailRepository", "SendMailSalesmen()", ex.Message.ToString(), null, null, null);
                throw new ApplicationException("ERROR EN: EmailRepository-SendMail()", ex);
            }
            return _result;

        }

        public async Task<bool> SendMailDescuento(int Idpedido, string IdSalesman, dynamic datos, int IdUsuario)
        {
            List<Salesman> _ListSalesman = new List<Salesman>();
            Email EmailModel = new Email();
            bool _result = false;
            try
            {

                List<Orders> _Pedido = await new PedidoRepository(_configVariables).ConsultarPedidos("", "", "", Idpedido, "", IdUsuario);

                //buscar datos del salesmen
                _ListSalesman = await new UsuarioRepository(_configVariables).ObtenerSalesman(IdSalesman);
                Salesman _Salesman = _ListSalesman.FirstOrDefault();

                //buscar parametros de configuracion para envio de email
                List<Configuracion> _parametros = await new ConfiguracionRepository(_configVariables).ObtenerParametros("portal.general.correo");
                //obtener email de descuento
                EmailModel = await ObtenerEmailDescuento(1);
                EmailModel.Subject = EmailModel.Subject.Replace("[PEDIDO]", Idpedido.ToString()).
                                                        Replace("[CODCLI]", _Pedido.FirstOrDefault().CustomerId).
                                                        Replace("[NOMCLI]", _Pedido.FirstOrDefault().Cliente);

                //string montoDescuento = datos["montodscto"].ToString().Replace(",", "").ToString("#,##0.00"); ;
                string montoDescuento = datos["montodscto"];
                string montoTotal = _Pedido.FirstOrDefault().AmountTotal.ToString("#,##0.00");

                EmailModel.Body = EmailModel.Body.Replace("[USUARIO]", IdSalesman).
                                                  Replace("[NOMUSU]", _Salesman.Name).
                                                  Replace("[CODCLI]", _Pedido.FirstOrDefault().CustomerId).
                                                  Replace("[NOMCLI]", _Pedido.FirstOrDefault().Cliente).
                                                  Replace("[PEDIDO]", Idpedido.ToString()).
                                                  Replace("[COMENTARIO]", _Pedido.FirstOrDefault().Comments).
                                                  Replace("[FECHA]", datos["fechapedido"]).
                                                  Replace("[HORA]", _Pedido.FirstOrDefault().OrderTime).
                                                  Replace("[PORCDESCUENTO]", _Pedido.FirstOrDefault().DiscountPercentage.ToString("#,##0")).
                                                  Replace("[MONTO]", montoTotal).
                                                  Replace("[MONTODSCTO]", montoDescuento).
                                                  Replace("[BR]", "<br />").
                                                  Replace("[B]", "<b>").
                                                  Replace("[/B]", "</b>");


                //DETALLE DEL PEDIDO
                string _detpedido = "<table><tr><td colspan = '5' style='padding-bottom:15px;'><b>Detalle del pedido:</b></td></tr>";
                string _cadena = string.Empty;
                foreach (var item in _Pedido)
                {
                    _cadena = "<tr style='padding:5px'><td style='padding-left:10px; padding-right:10px;'>" + item.CodigoProducto + "</td>" +
                              "<td style='padding-left:10px; padding-right:10px;'>" + item.Descripcion + "</td>" +
                              "<td style='padding-left:10px; padding-right:10px;'>" + item.Unit + "</td>" +
                              "<td style='padding-left:10px; padding-right:10px;'>" + item.Qty + "</td>" +
                              "<td style='padding-left:10px; padding-right:10px;'>" + item.Precio.ToString("#,##0") + "</td></tr>";

                    _detpedido = _detpedido + _cadena;

                }
                _detpedido = _detpedido + "</table><br/>";

                EmailModel.Body = EmailModel.Body + _detpedido;

                EmailModel.Body = EmailModel.Body + EmailModel.footer.Replace("[BR]", "<br />");


                EmailModel.CopyTo = _Salesman.Mail;

                EmailModel.Host = _parametros.Find(x => x.Codigo == "HOSTMAIL").Valor;
                EmailModel.Port = int.Parse(_parametros.Find(x => x.Codigo == "PORTHOSTEMAIL").Valor);
                EmailModel.Login = _parametros.Find(x => x.Codigo == "LOGINMAIL").Valor;
                EmailModel.Password = _parametros.Find(x => x.Codigo == "PASSWORDMAIL").Valor;
                EmailModel.From = _parametros.Find(x => x.Codigo == "EMAILEMPRESA").Valor;
                EmailModel.UseSSL = _parametros.Find(x => x.Codigo == "USESSL").Valor;
                EmailModel.UseAuth = _parametros.Find(x => x.Codigo == "USEAUTH").Valor;

                _result = await SendMail(EmailModel);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"EmailRepository", "SendMailSalesmen()", ex.Message.ToString(), null, null, null);
                throw new ApplicationException("ERROR EN: EmailRepository-SendMail()", ex);
            }
            return _result;

        }

        public async Task<bool> SendMailAdmSistema(string estado, string destinatarios, string usuario)
        {

            Email EmailModel = new Email();
            bool _result = false;

            try
            {

                //BUSCAR PARAMETROS DE CONFIGURACION PARA ENVIO DE EMAIL
                List<Configuracion> _parametros = await new ConfiguracionRepository(_configVariables).ObtenerParametros("portal.general.correo");

                //OBTENER EMAIL
                EmailModel = await ObtenerEmail(2);
                EmailModel.To = destinatarios;
                EmailModel.Subject = EmailModel.Subject;
                estado = (estado == "A" ? "ACTIVADO" : "INACTIVADO");

                EmailModel.Body = EmailModel.Body.Replace("[ESTADO]", estado).
                                                  Replace("[USUARIO]", usuario).
                                                  Replace("[FECHA]", DateTime.Now.ToShortDateString()).
                                                  Replace("[BR]", "<br />").
                                                  Replace("[B]", "<b>").
                                                  Replace("[/B]", "</b>");

                EmailModel.Body = EmailModel.Body + EmailModel.footer.Replace("[BR]", "<br />");

                EmailModel.Host = _parametros.Find(x => x.Codigo == "HOSTMAIL").Valor;
                EmailModel.Port = int.Parse(_parametros.Find(x => x.Codigo == "PORTHOSTEMAIL").Valor);
                EmailModel.Login = _parametros.Find(x => x.Codigo == "LOGINMAIL").Valor;
                EmailModel.Password = _parametros.Find(x => x.Codigo == "PASSWORDMAIL").Valor;
                EmailModel.From = _parametros.Find(x => x.Codigo == "EMAILEMPRESA").Valor;
                EmailModel.UseSSL = _parametros.Find(x => x.Codigo == "USESSL").Valor;
                EmailModel.UseAuth = _parametros.Find(x => x.Codigo == "USEAUTH").Valor;

                _result = await SendMail(EmailModel);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"EmailRepository", "SendMailAdmSistema()", ex.Message.ToString(), null, null, null);
                throw new ApplicationException("ERROR EN: EmailRepository-SendMailAdmSistema()", ex);
            }
            return _result;

        }

        public async Task<bool> SendMailEstadoCuenta(string IdSalesman, string customerid, List<EstadoCuenta> Movimientos)
        {
            Email EmailModel = new Email();
            List<Salesman> _ListSalesman = new List<Salesman>();
            List<Customer> _ListCustomer = new List<Customer>();

            bool _result = false;
            try
            {
                //BUSCAR PARAMETROS DE CONFIGURACION PARA ENVIO DE EMAIL
                List<Configuracion> _parametros = await new ConfiguracionRepository(_configVariables).ObtenerParametros("portal.general.correo");

                //buscar datos del salesmen
                _ListSalesman = await new UsuarioRepository(_configVariables).ObtenerSalesman(IdSalesman);
                Salesman _Salesman = _ListSalesman.FirstOrDefault();

                //buscar datos del cliente
                _ListCustomer = await new ClienteRepository(_configVariables).ObtenerClientes(IdSalesman, null, 2, customerid);
                Customer _Customer = _ListCustomer.FirstOrDefault();

                //OBTENER EL CORREO DE LA TABLA CONFIGURACION
                EmailModel = await ObtenerEmail(3);

                EmailModel.To = _Customer.Email;

                EmailModel.Body = EmailModel.Body.Replace("[USUARIO]", IdSalesman).
                                                Replace("[NOMUSU]", _Salesman.Name).
                                                Replace("[BR]", "<br />").
                                                Replace("[B]", "<b>").
                                                Replace("[/B]", "</b>");

                //DETALLE DEL ESTADO DE CUENTA
                string _detEstadoCuenta = "<table><tr><td colspan = '8' style='padding-bottom:15px;'><b>ESTADO DE CUENTA</b></td></tr>" +
                                            "<tr><td style='text-align: center;'>TIPO DOC.</td><td style='text-align: center;'>DESCRIPCIÓN</td>" +
                                            "<td style='text-align: center;'>NRO. DOC.</td><td style='text-align: center;'>FECHA</td>" +
                                            "<td style='text-align: center;'>DIAS VCTO.</td><td style='text-align: center;'>MONTO</td><td style='text-align: center;'>MONTO PENDIENTE</td><td>OBSERVACÓN</td></tr>";
                string _cadena = string.Empty;
                double Total = 0;

                foreach (var item in Movimientos)
                {
                    Total = Total + item.MontoDocumento;
                    _cadena = "<tr style='padding:5px'><td style='padding-left:10px; padding-right:10px;'>" + item.TipoDocumento + "</td>" +
                              "<td style='padding-left:10px; padding-right:10px;'>" + item.Descripcion + "</td>" +
                              "<td style='padding-left:10px; padding-right:10px;'>" + item.NumeroDocumento + "</td>" +
                              "<td style='padding-left:10px; padding-right:10px;'>" + item.FechaDocumento + "</td>" +
                              "<td style='padding-left:10px; padding-right:10px;'>" + item.DiasVencimiento + "</td>" +
                              "<td style='padding-left:10px; padding-right:10px;'>" + item.MontoDocumento.ToString("#,##0.00") + "</td>" +
                              "<td style='padding-left:10px; padding-right:10px;'>" + item.MontoPendiente.ToString("#,##0.00") + "</td>" +
                              "<td style='padding-left:10px; padding-right:10px;'>" + item.Observacion + "</td></tr>";

                    _detEstadoCuenta = _detEstadoCuenta + _cadena;

                }
                _detEstadoCuenta = _detEstadoCuenta + "</table><br/>";

                EmailModel.Body = EmailModel.Body.Replace("[TOTAL]", Total.ToString("#,##0.00")) + _detEstadoCuenta;
                EmailModel.Body = EmailModel.Body + EmailModel.footer.Replace("[BR]", "<br />");


                EmailModel.Host = _parametros.Find(x => x.Codigo == "HOSTMAIL").Valor;
                EmailModel.Port = int.Parse(_parametros.Find(x => x.Codigo == "PORTHOSTEMAIL").Valor);
                EmailModel.Login = _parametros.Find(x => x.Codigo == "LOGINMAIL").Valor;
                EmailModel.Password = _parametros.Find(x => x.Codigo == "PASSWORDMAIL").Valor;
                EmailModel.From = _parametros.Find(x => x.Codigo == "EMAILEMPRESA").Valor;
                EmailModel.UseSSL = _parametros.Find(x => x.Codigo == "USESSL").Valor;
                EmailModel.UseAuth = _parametros.Find(x => x.Codigo == "USEAUTH").Valor;

                _result = await SendMail(EmailModel);

            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"EmailRepository", "SendMailAdmSistema()", ex.Message.ToString(), null, null, null);
                throw new ApplicationException("ERROR EN: EmailRepository-SendMailAdmSistema()", ex);
            }
            return _result;
        }

        public async Task<bool> SendMailLogistica(int Idpedido, string IdSalesman)
        {
            List<Salesman> _ListSalesman = new List<Salesman>();
            Email EmailModel = new Email();
            bool _result = false;
            try
            {
                //BUSCAR DATOS DEL SALESMEN
                _ListSalesman = await new UsuarioRepository(_configVariables).ObtenerSalesman(IdSalesman);
                Salesman _Salesman = _ListSalesman.FirstOrDefault();

                //BUSCAR PARAMETROS DE CONFIGURACION PARA ENVIO DE EMAIL
                List<Configuracion> _parametros = await new ConfiguracionRepository(_configVariables).ObtenerParametros("portal.general.correo");
                List<Configuracion> _maillogistica = await new ConfiguracionRepository(_configVariables).ObtenerParametros("portal.notificacion.logistica");

                //OBTENER EMAIL
                EmailModel = await ObtenerEmail(1);
                EmailModel.To = _maillogistica.Find(x => x.Codigo == "EMAIL").Valor;



                //CONSULTAR DATOS DEL PEDIDO
                List<Orders> _Pedido = await new PedidoRepository(_configVariables).ConsultarPedidos("", "", "", Idpedido, "", 0);

                EmailModel.Subject = EmailModel.Subject.Replace("[PEDIDO]", Idpedido.ToString()).
                                                        Replace("[CODCLI]", _Pedido.FirstOrDefault().CustomerId).
                                                        Replace("[NOMCLI]", _Pedido.FirstOrDefault().Cliente);

                string montoDescuento = _Pedido.FirstOrDefault().DiscountTotal.ToString("#,##0.00");
                string montoTotal = _Pedido.FirstOrDefault().AmountTotal.ToString("#,##0.00");


                EmailModel.Body = EmailModel.Body.Replace("[USUARIO]", IdSalesman).
                                                  Replace("[NOMUSU]", _Salesman.Name).
                                                  Replace("[CODCLI]", _Pedido.FirstOrDefault().CustomerId).
                                                  Replace("[NOMCLI]", _Pedido.FirstOrDefault().Cliente).
                                                  Replace("[PEDIDO]", Idpedido.ToString()).
                                                  Replace("[COMENTARIO]", _Pedido.FirstOrDefault().Comments).
                                                  Replace("[FECHA]", _Pedido.FirstOrDefault().OrderDate).
                                                  Replace("[HORA]", _Pedido.FirstOrDefault().OrderTime).
                                                  Replace("[DESCUENTO]", montoDescuento).
                                                  Replace("[MONTO]", _Pedido.FirstOrDefault().AmountTotal.ToString()).
                                                  Replace("[BR]", "<br />").
                                                  Replace("[B]", "<b>").
                                                  Replace("[/B]", "</b>");


                //DETALLE DEL PEDIDO
                string _detpedido = "<table><tr><td colspan = '5' style='padding-bottom:15px;'><b>Detalle del pedido:</b></td></tr>";
                string _cadena = string.Empty;
                foreach (var item in _Pedido)
                {
                    _cadena = "<tr style='padding:5px'><td style='padding-left:10px; padding-right:10px;'>" + item.CodigoProducto + "</td>" +
                              "<td style='padding-left:10px; padding-right:10px;'>" + item.Descripcion + "</td>" +
                              "<td style='padding-left:10px; padding-right:10px;'>" + item.Unit + "</td>" +
                              "<td style='padding-left:10px; padding-right:10px;'>" + item.Qty + "</td>" +
                              "<td style='padding-left:10px; padding-right:10px;'>" + item.Precio.ToString("#,##0") + "</td></tr>";

                    _detpedido = _detpedido + _cadena;

                }
                _detpedido = _detpedido + "</table><br/>";

                EmailModel.Body = EmailModel.Body + _detpedido;

                EmailModel.Body = EmailModel.Body + EmailModel.footer.Replace("[BR]", "<br />");


                EmailModel.Host = _parametros.Find(x => x.Codigo == "HOSTMAIL").Valor;
                EmailModel.Port = int.Parse(_parametros.Find(x => x.Codigo == "PORTHOSTEMAIL").Valor);
                EmailModel.Login = _parametros.Find(x => x.Codigo == "LOGINMAIL").Valor;
                EmailModel.Password = _parametros.Find(x => x.Codigo == "PASSWORDMAIL").Valor;
                EmailModel.From = _parametros.Find(x => x.Codigo == "EMAILEMPRESA").Valor;
                EmailModel.UseSSL = _parametros.Find(x => x.Codigo == "USESSL").Valor;
                EmailModel.UseAuth = _parametros.Find(x => x.Codigo == "USEAUTH").Valor;

                _result = await SendMail(EmailModel);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"EmailRepository", "SendMailSalesmen()", ex.Message.ToString(), null, null, null);
                throw new ApplicationException("ERROR EN: EmailRepository-SendMail()", ex);
            }
            return _result;

        }

        public async Task<bool> SendMailAprobarPedido(int IdOrden)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {

                _ldato.Parametros.Add("@ORDERID", IdOrden);
                _data = await _ldato.EjecutarScalarReader("PED_USP_ENVIARORDENCORREO", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PedidoRepository", "SendMailAprobarPedido()", ex.Message.ToString(), null, null, "PED_USP_ENVIARORDENCORREO");
            }
            return true;
            //return (bool)_data.Valor;
        }
    }
}
