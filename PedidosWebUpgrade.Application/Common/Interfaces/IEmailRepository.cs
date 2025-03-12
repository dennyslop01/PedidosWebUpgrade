using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IEmailRepository
    {
        Email ObtenerEmail(int Id);

        Email ObtenerEmailDescuento(int Id);

        bool SendMail(Email EmailModel);

        bool SendMailSalesmen(int Idpedido, string IdSalesman, string fechapedido, string montototal, int IdUsuario);

        bool SendMailDescuento(int Idpedido, string IdSalesman, dynamic datos, int IdUsuario);

        public bool SendMailAdmSistema(string estado, string destinatarios, string usuario);

        public bool SendMailEstadoCuenta(string IdSalesman, string customerid, List<EstadoCuenta> Movimientos);

        bool SendMailLogistica(int Idpedido, string IdSalesman);

        public bool SendMailAprobarPedido(int IdOrden);
    }
}
