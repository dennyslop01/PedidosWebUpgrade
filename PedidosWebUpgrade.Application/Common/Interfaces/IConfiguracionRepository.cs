using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IConfiguracionRepository
    {
        List<Configuracion> ObtenerParametros(String categoria);

        List<Sistema> ConsultarSistema();

        Dictionary<string, object> ActualizarSistema(Sistema _Sistema);

        List<ContadorPedidosPais> ConsultarPedidosPais();

        int ActualizarContadorPais(ContadorPedidosPais _ContadorPais);

        int CopiarContadoresAnno(int annoCurso, int AnnoNuevo);

        List<F0004> ObtenerF0004(string Dtsy, string Dtrt);

        int ActualizarF0004(F0004 _ModelF0004);

        int EliminarF0004(string Dtsy, string Dtrt);

        List<F0005> ObtenerF0005(string drsy, string drrt, string drky);

        int ActualizarF0005(F0005 _ModelF0005);

        int EliminarF0005(string Drrt, string Drsy, string Drky);

        List<PreferenciaAlmacenClientePais> ObtenerPreferenciaAlmacenClientePais(int IdPreferencia);

        Dictionary<string, object> ActualizarPreferenciaAlmacenClientePais(PreferenciaAlmacenClientePais preferenciaAlmacenClientePais);

        int EliminarPreferenciaAlmacenClientePais(int IdPreferencia);

        Dictionary<string, object> ActualizarAgenteAduanal(ForwardingAgent forwardingAgent);

        List<ForwardingAgent> ConsultarAgentesAduanales();

        ForwardingAgent ConsultarUnAgenteAduanal(int AgentId);

        int EliminarAgenteAduanal(int IdAgent);
    }
}
