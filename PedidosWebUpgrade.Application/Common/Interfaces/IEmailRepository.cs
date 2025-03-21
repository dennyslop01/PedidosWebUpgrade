using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IEmailRepository
    {
        Task<Email> ObtenerEmail(int Id);

        Task<Email> ObtenerEmailDescuento(int Id);

        Task<bool> SendMail(Email EmailModel);

        Task<bool> SendMailSalesmen(int Idpedido, string IdSalesman, string fechapedido, string montototal, int IdUsuario);

        Task<bool> SendMailDescuento(int Idpedido, string IdSalesman, dynamic datos, int IdUsuario);

        Task<bool> SendMailAdmSistema(string estado, string destinatarios, string usuario);

        Task<bool> SendMailEstadoCuenta(string IdSalesman, string customerid, List<EstadoCuenta> Movimientos);

        Task<bool> SendMailLogistica(int Idpedido, string IdSalesman);

        Task<bool> SendMailAprobarPedido(int IdOrden);
    }
}
