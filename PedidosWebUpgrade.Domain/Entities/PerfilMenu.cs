namespace PedidosWebUpgrade.Domain.Entities
{
    public class PerfilMenu
    {
        public int Idperfilmenu { get; set; }
        public int Idperfil { get; set; }
        public int Idmenu { get; set; }
        public string? Descripcion { get; set; }
        public string? Url { get; set; }
    }
}
