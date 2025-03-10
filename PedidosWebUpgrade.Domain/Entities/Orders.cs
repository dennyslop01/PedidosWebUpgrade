namespace PedidosWebUpgrade.Domain.Entities
{
    public class Orders
    {
        public int IdOrder { get; set; }
        public string? OrderDate { get; set; }
        public string? OrderTime { get; set; }
        public string? CustomerId { get; set; }
        public string? Estatus { get; set; }
        public double Amount { get; set; } = 0;
        public string? Comments { get; set; }
        public string? Cliente { get; set; }
        public string? CodigoProducto { get; set; }
        public string? Unit { get; set; }
        public double Qty { get; set; } = 0;
        public double SubTotal { get; set; } = 0;
        public string? Warehouse { get; set; }
        public string? Descripcion { get; set; }
        public double Precio { get; set; } = 0;
        public double AmountTotal { get; set; } = 0;
        public double DiscountTotal { get; set; } = 0;
        public double DiscountPercentage { get; set; } = 0;
        public string? OrderNumber { get; set; }
        public string? IdCarrier { get; set; }
        public string? Carrier { get; set; }
        public string? IdPuertoDescarga { get; set; }
        public string? PuertoDescarga { get; set; }
        public string? IdIncoTerms { get; set; }
        public string? IncoTerms { get; set; }
        public string? IdRef001 { get; set; }
        public string? Referencia001 { get; set; }
        public string? TiempoLlegada { get; set; }
        public string? OrdenPrint { get; set; }
        public string? RequiredDate { get; set; }
        public string? RequiredDateDatetimeFormat { get; set; }
        public string? Almacen { get; set; }
        public string? Motivo { get; set; }
        public string? MotivoReactivacion { get; set; } = string.Empty;
        public int NumeroRevision { get; set; } = 0;
        public string? UsuarioReactivacion { get; set; } = string.Empty;
        public int IdOrderOriginal { get; set; } = 0;
        public string? ReactivarPedido { get; set; }
        public string? infoJDE { get; set; }
        public string? CutDate { get; set; }
        public string? LeadTime { get; set; }
        public string? CodigoModTransporte { get; set; }
        public string? ModTransporte { get; set; }
        public int EstadoSiguienteJde { get; set; }
        public int IdAgente { get; set; }
        public string? RequieredDate { get; set; }
        public string? EtaDate { get; set; }
        public string? EtdDate { get; set; }
        public string? ForwardingAgent { get; set; }
        public string? ProformaEnviada { get; set; }
        public string? ListaPrecio { get; set; }
        public string? ShipTo { get; set; }
        public double Descuento { get; set; } = 0;
        public string? NameShipTo { get; set; }
        public string? Moneda { get; set; }
        public double Tax { get; set; }
        public string? IdCondicionPago { get; set; }
        public string? CodigoPaisFacturacion { get; set; }
        public int CodigoAnnoCorrelativo { get; set; }
        public string? DescripcionCondicionPago { get; set; }
        public int? TipoPedido { get; set; }
    }
}
