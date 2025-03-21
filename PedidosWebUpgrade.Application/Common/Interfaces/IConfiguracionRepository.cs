using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IConfiguracionRepository
    {
        Task<List<Configuracion>> ObtenerParametros(String categoria);

        Task<List<Sistema>> ConsultarSistema();

        Task<Dictionary<string, object>> ActualizarSistema(Sistema _Sistema);

        Task<List<ContadorPedidosPais>> ConsultarPedidosPais();

        Task<int> ActualizarContadorPais(ContadorPedidosPais _ContadorPais);

        Task<int> CopiarContadoresAnno(int annoCurso, int AnnoNuevo);

        Task<List<F0004>> ObtenerF0004(string Dtsy, string Dtrt);

        Task<int> ActualizarF0004(F0004 _ModelF0004);

        Task<int> EliminarF0004(string Dtsy, string Dtrt);

        Task<List<F0005>> ObtenerF0005(string drsy, string drrt, string drky);

        Task<int> ActualizarF0005(F0005 _ModelF0005);

        Task<int> EliminarF0005(string Drrt, string Drsy, string Drky);

        Task<List<PreferenciaAlmacenClientePais>> ObtenerPreferenciaAlmacenClientePais(int IdPreferencia);

        Task<Dictionary<string, object>> ActualizarPreferenciaAlmacenClientePais(PreferenciaAlmacenClientePais preferenciaAlmacenClientePais);

        Task<int> EliminarPreferenciaAlmacenClientePais(int IdPreferencia);

        Task<Dictionary<string, object>> ActualizarAgenteAduanal(ForwardingAgent forwardingAgent);

        Task<List<ForwardingAgent>> ConsultarAgentesAduanales();

        Task<ForwardingAgent> ConsultarUnAgenteAduanal(int AgentId);

        Task<int> EliminarAgenteAduanal(int IdAgent);
    }
}
