namespace PedidosWebUpgrade.Infrastructure.DbContext
{
    public class DataResponse<T>
    {
        public int CodigoRetorno { get; set; }
        public string? Mensaje { get; set; }
        public T Valor { get; set; }
    }
}
