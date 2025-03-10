namespace PedidosWebUpgrade.Domain.Entities
{
    public class HistoriaOrder
    {
        public int OrderId { get; set; }
        public int IdRevision { get; set; }
        public DateTime Fecha { get; set; }
        public int IdUsuario { get; set; }
        public int OrderIdOriginal { get; set; }
        public bool UltimoEnviado { get; set; }
        public string? Comentario { get; set; }
        public string? NumCorrelativo { get; set; }

        public string? DescripcionUltimoEnviado
        {
            get
            {
                switch (UltimoEnviado)
                {
                    case true: return "SI";
                    default: return "NO";
                }
            }

        }
    }
}
