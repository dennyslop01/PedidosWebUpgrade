using PedidosWebUpgrade.Application.Common.Interfaces;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Domain.ViewModels;
using PedidosWebUpgrade.Infrastructure.DbContext;
using PedidosWebUpgrade.Infrastructure.Utilities;
using System.Data;
using System.Data.SqlClient;

namespace PedidosWebUpgrade.Infrastructure.Repository
{
    public class PedidoRepository: IPedidoRepository
    {
        private readonly ConfigVariables _configVariables;

        public PedidoRepository(ConfigVariables configVariables)
        {
            _configVariables = configVariables;
        }

        /// <summary>
        /// AGREGAR PRODUCTO A LA CESTA DEL PEDIDO
        /// </summary>
        /// <param name="Producto">Producto</param>
        /// <param name="CodCliente"></paramstring>
        /// <param name="IdOrden">int</param>
        /// <returns> Dictionary<string, object> </returns>
        public Dictionary<string, object> AgregarProducto(Producto Producto, string CodCliente, int IdOrden)
        {
            lDato _ldato = new lDato(_configVariables);
            try
            {
                List<SqlParameter> _parametros = [ new SqlParameter() {ParameterName = "@CUSTOMER_ID",  Value =  CodCliente, SqlDbType = SqlDbType.VarChar, Size = 8},
                                                                          new SqlParameter() {ParameterName = "@PRODUCT_ID",  Value =  Producto.Codigo, SqlDbType = SqlDbType.VarChar, Size = 50},
                                                                          new SqlParameter() {ParameterName = "@CANTIDAD", Value =  Producto.CantidadIngresada, SqlDbType = SqlDbType.Float},
                                                                          new SqlParameter() {ParameterName = "@PRICE", Value =  Producto.PrecioUno, SqlDbType = SqlDbType.Float},
                                                                          new SqlParameter() {ParameterName = "@WAREHOUSE",  Value =  Producto.Warehouse, SqlDbType = SqlDbType.VarChar, Size = 12},
                                                                          new SqlParameter() {ParameterName = "@ORDER_ID", Value = IdOrden, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@LISTAPRECIO_ID", Value = Producto.IdListaPrecio, SqlDbType = SqlDbType.VarChar, Size = 100},
                                                                          new SqlParameter() {ParameterName = "@SHIPTO_ID", Value = Producto.IdShipTo, SqlDbType = SqlDbType.VarChar, Size = 8},
                                                                          new SqlParameter() {ParameterName = "@ORDERIDOUT", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@RESULTADO",  Direction = ParameterDirection.Output, SqlDbType = SqlDbType.TinyInt }
                                                                         ];
                _ldato.ParametrosSql = _parametros;
                var resultado = _ldato.EjecutarNonQueryOutput("PED_USP_CREARORDENPEDIDO", _ldato.ParametrosSql);
                return resultado.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PedidoRepository", "AgregarProducto()", ex.ToString(), null, null, "PED_USP_CREARORDENPEDIDO");
                throw new ApplicationException("ERROR EN: AgregarProducto", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            ;
        }

        /// <summary>
        /// RETORNA EL ENCABEZADO DEL PEDIDO
        /// </summary>
        /// <param name="IdOrden">int</param>
        /// <param name="IdCliente">string</param>
        /// <returns>EncabezadoPedido</returns>
        public EncabezadoPedido ObtenerEncabezadoOrden(int IdOrden, string IdCliente, string salesmanid = null)
        {
            lDato _ldato = new lDato(_configVariables);
            List<EncabezadoPedido> _Encabezado = [];
            DataResponse<List<EncabezadoPedido>> List_Response = new DataResponse<List<EncabezadoPedido>>();
            try
            {

                _ldato.Parametros.Add("@IDORDEN", IdOrden);
                _ldato.Parametros.Add("@CUSTOMERID", IdCliente);
                _ldato.Parametros.Add("@SALESMANID", salesmanid ?? (object)DBNull.Value);
                _ldato.Esquema.Add("IdOrden", "ORDER_ID");
                _ldato.Esquema.Add("NumeroCajas", "QTY");
                _ldato.Esquema.Add("MontoTotal", "AMOUNT");
                _ldato.Esquema.Add("TotalDescuento", "discount_total");
                _ldato.Esquema.Add("Moneda", "MONEDA");
                _ldato.Esquema.Add("ImpuestoTotal", "tax_total");

                List_Response = _ldato.EjecutarReader(new EncabezadoPedido(), "PED_USP_CONSULTARENCABEZADOORDEN", _ldato.Parametros, _ldato.Esquema);
                _Encabezado = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PedidoRepository", "ObtenerEncabezadoOrden()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARENCABEZADOORDEN");
                throw new ApplicationException("ERROR EN: ObtenerEncabezadoOrden()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Encabezado.FirstOrDefault();
        }

        /// <summary>
        /// PROCESAR EL PEDIDO DE UN CLIENTE
        /// </summary>
        /// <param name="IdOrden">int</param> 
        /// <returns>bool</returns>
        public bool ProcesarOrden(int IdOrden, string condicion, int porcentaje, string fechapedido, string fecharequerida, string observacion, string montototal,
                                   string montodscto, string prepagado, string tasanegociacion, string idcarrier, string idpuertodescarga, string idincoterms,
                                   string referencia01, string tiempollegada, string ordenprint, int usuario, string fechacorte, string leadtime, string codModalidad,
                                   string FechaDespachoETD, string FechaLlegadaETA, int ForwardingAgent, string idlistaprecio, string idshipto, string idalmacen, string CodCondPago, string codPaisFacturacion, int codAnnoCorrelativo, int TipoPedido)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                if (String.IsNullOrEmpty(referencia01))
                    referencia01 = "";

                _ldato.Parametros.Add("@ORDERID", IdOrden);
                _ldato.Parametros.Add("@CONDICION", condicion);
                _ldato.Parametros.Add("@PORCENTAJE", porcentaje);
                _ldato.Parametros.Add("@FECHAPEDIDO", fechapedido);
                _ldato.Parametros.Add("@FECHAREQUERIDA", fecharequerida);
                _ldato.Parametros.Add("@OBSERVACIONES", observacion);
                _ldato.Parametros.Add("@MONTOTOTAL", CustomUtility.FormaterNumero(montototal));
                _ldato.Parametros.Add("@MONTODESCUENTO", CustomUtility.FormaterNumero(montodscto));
                _ldato.Parametros.Add("@PREPAGADO", Convert.ToBoolean(prepagado));
                _ldato.Parametros.Add("@TASANEGOCIACION", CustomUtility.FormaterNumero(tasanegociacion));
                _ldato.Parametros.Add("@ORDER_NUMBER", "0");
                _ldato.Parametros.Add("@IDCARRIER", idcarrier);
                _ldato.Parametros.Add("@IDPUERTODESCARGA", idpuertodescarga);
                _ldato.Parametros.Add("@IDINCOTERMS", idincoterms);
                _ldato.Parametros.Add("@REFERENCIA01", referencia01);
                _ldato.Parametros.Add("@TIEMPOESTIMADOLLEGADA", tiempollegada);
                _ldato.Parametros.Add("@ORDENPRINT", ordenprint);
                _ldato.Parametros.Add("@USERID", usuario);
                _ldato.Parametros.Add("@FECHACORTE", fechacorte);
                _ldato.Parametros.Add("@LEADTIME", leadtime);
                _ldato.Parametros.Add("@IDMODOTRANSPORTE", string.IsNullOrWhiteSpace(codModalidad) ? Convert.DBNull : codModalidad);
                _ldato.Parametros.Add("@FECHADESPACHOETD", FechaDespachoETD);
                _ldato.Parametros.Add("@FECHALLEGADAETA", FechaLlegadaETA);
                _ldato.Parametros.Add("@IDFORWARDINGAGENT", ForwardingAgent);
                _ldato.Parametros.Add("@IDLISTAPRECIOS", idlistaprecio);
                _ldato.Parametros.Add("@IDSHIPTTO", idshipto);
                _ldato.Parametros.Add("@IDALMACEN", idalmacen);
                _ldato.Parametros.Add("@IDCONDPAGO", CodCondPago);
                _ldato.Parametros.Add("@IDPAISFACTURACION", codPaisFacturacion);
                _ldato.Parametros.Add("@ANNOCORRELATIVO", codAnnoCorrelativo);
                _ldato.Parametros.Add("@TIPOPEDIDO", TipoPedido);


                _data = _ldato.EjecutarScalarReader("PED_USP_PROCESARORDEN", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PedidoRepository", "ProcesarOrden()", ex.Message.ToString(), null, null, "PED_USP_PROCESARORDEN");
            }
            return (bool)_data.Valor;
        }


        /// <summary>
        /// PROCESAR EL PEDIDO DE UN CLIENTE
        /// </summary>
        /// <param name="IdOrden">int</param> 
        /// <returns>bool</returns>
        public bool AprobarOrden(int IdOrden)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {

                _ldato.Parametros.Add("@ORDERID", IdOrden);
                _ldato.EjecutarScalarReader("PED_USP_SEGMENTARMARCASPEDIDO", _ldato.Parametros);

                _ldato = new lDato(_configVariables);
                _ldato.Parametros.Add("@ORDERID", IdOrden);
                _data = _ldato.EjecutarScalarReader("PED_USP_APROBARORDEN", _ldato.Parametros);

                // Segmentacion del pedido
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PedidoRepository", "AprobarOrden()", ex.Message.ToString(), null, null, "PED_USP_APROBARORDEN");
            }
            return (bool)_data.Valor;
        }

        public List<Orders> ConsultarPedidos(string CustomerId, string FechaDesde, string FechaHasta, int OrderId, string Estatus, int IdUsuario)
        {
            lDato _ldato = new lDato(_configVariables);
            List<Orders> _Pedidos = [];
            DataResponse<List<Orders>> List_Response = new DataResponse<List<Orders>>();
            try
            {
                _ldato.Parametros.Add("@CUSTOMERID", (CustomerId == "" ? (object)DBNull.Value : CustomerId));
                _ldato.Parametros.Add("@FECHADESDE", (FechaDesde == "" ? (object)DBNull.Value : FechaDesde));
                _ldato.Parametros.Add("@FECHAHASTA", (FechaHasta == "" ? (object)DBNull.Value : FechaHasta));
                _ldato.Parametros.Add("@ORDERID", (OrderId == 0 ? (object)DBNull.Value : OrderId));
                _ldato.Parametros.Add("@ESTATUS", (Estatus == "" ? (object)DBNull.Value : Estatus));
                _ldato.Parametros.Add("@IDUSUARIO", IdUsuario);

                _ldato.Esquema.Add("IdOrder", "ORDER_ID");
                _ldato.Esquema.Add("OrderDate", "ORDER_DATE");
                _ldato.Esquema.Add("OrderTime", "ORDER_TIME");
                _ldato.Esquema.Add("Estatus", "STATUS");
                _ldato.Esquema.Add("Amount", "AMOUNT");
                _ldato.Esquema.Add("Comments", "COMMENTS");
                _ldato.Esquema.Add("Cliente", "NAME");
                _ldato.Esquema.Add("CodigoProducto", "PRODUCT_ID");
                _ldato.Esquema.Add("Qty", "QTY");
                _ldato.Esquema.Add("Unit", "UNIT");
                _ldato.Esquema.Add("SubTotal", "SUBTOTAL");
                _ldato.Esquema.Add("Warehouse", "WAREHOUSE");
                _ldato.Esquema.Add("Descripcion", "DESCRIPTION");
                _ldato.Esquema.Add("Precio", "PRICE1");
                _ldato.Esquema.Add("CustomerId", "CUSTOMER_ID");
                _ldato.Esquema.Add("AmountTotal", "AMOUNT_TOTAL");
                _ldato.Esquema.Add("DiscountTotal", "DISCOUNT_TOTAL");
                _ldato.Esquema.Add("DiscountPercentage", "DISCOUNT_PERCENTAGE");
                _ldato.Esquema.Add("OrderNumber", "ORDER_NUMBER");
                _ldato.Esquema.Add("IdCarrier", "ID_CARRIER");
                _ldato.Esquema.Add("IdPuertoDescarga", "ID_PUERTO_DESCARGA");
                _ldato.Esquema.Add("IdIncoTerms", "ID_INCOTERMS");
                _ldato.Esquema.Add("IdRef001", "REFERENCIA01");
                _ldato.Esquema.Add("TiempoLlegada", "TIEMPO_ESTIMADO_LLEGADA");
                _ldato.Esquema.Add("OrdenPrint", "ORDER_PRINT");
                _ldato.Esquema.Add("RequiredDate", "REQUIRED_DATE");
                _ldato.Esquema.Add("RequiredDateDatetimeFormat", "cut_dateDateFormat");
                _ldato.Esquema.Add("Almacen", "ALMACEN");
                _ldato.Esquema.Add("Motivo", "MOTIVO");
                _ldato.Esquema.Add("MotivoReactivacion", "MOTIVO_REACTIVACION");
                _ldato.Esquema.Add("NumeroRevision", "NUMERO_REVISION");
                _ldato.Esquema.Add("UsuarioReactivacion", "NOMBRE_USUARIO_REACTIVACION");
                _ldato.Esquema.Add("IdOrderOriginal", "ORDER_ID_ORIGINAL");
                _ldato.Esquema.Add("ReactivarPedido", "REACTIVAR_PEDIDO");
                _ldato.Esquema.Add("infoJDE", "INFOJDE");
                _ldato.Esquema.Add("LeadTime", "LEAD_TIME");
                _ldato.Esquema.Add("CutDate", "CUT_DATE");
                _ldato.Esquema.Add("CodigoModTransporte", "ID_MODALIDAD_TRANSPORTE");
                _ldato.Esquema.Add("EstadoSiguienteJde", "ESTADO_SIGUIENTE_JDE");
                _ldato.Esquema.Add("IdAgente", "AgentId");
                _ldato.Esquema.Add("ModTransporte", "ModalidadTransporte");
                _ldato.Esquema.Add("ForwardingAgent", "ForwardingAgent");
                _ldato.Esquema.Add("ProformaEnviada", "ProformaEnviada");
                _ldato.Esquema.Add("RequieredDate", "FechaRequerida");
                _ldato.Esquema.Add("EtaDate", "FechaETA");
                _ldato.Esquema.Add("EtdDate", "FechaETD");
                _ldato.Esquema.Add("ListaPrecio", "id_lista_precio");
                _ldato.Esquema.Add("ShipTo", "ship_to");
                _ldato.Esquema.Add("Descuento", "DISCOUNT");
                _ldato.Esquema.Add("NameShipTo", "NAME_SHIP_TO");
                _ldato.Esquema.Add("Moneda", "moneda");
                _ldato.Esquema.Add("Tax", "TAX");
                _ldato.Esquema.Add("IdCondicionPago", "CodigoCondicionPago");
                _ldato.Esquema.Add("CodigoPaisFacturacion", "CodigoPaisFacturacion");
                _ldato.Esquema.Add("CodigoAnnoCorrelativo", "CodigoAnnoCorrelativo");
                _ldato.Esquema.Add("DescripcionCondicionPago", "DescripcionCondicionPago");


                List_Response = _ldato.EjecutarReader(new Orders(), "PED_USP_CONSULTARPEDIDOSGENERADOS", _ldato.Parametros, _ldato.Esquema);
                _Pedidos = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PedidoRepository", "ConsultarPedidos()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARPEDIDOSGENERADOS");
                throw new ApplicationException("ERROR EN: ConsultarPedidos()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Pedidos;
        }

        public List<ListaGeneral> ObtenerEstatus()
        {
            lDato _ldato = new lDato(_configVariables);
            List<ListaGeneral> _Estatus = [];
            DataResponse<List<ListaGeneral>> List_Response = new DataResponse<List<ListaGeneral>>();
            try
            {
                _ldato.Esquema.Add("Codigo", "CODIGO");
                _ldato.Esquema.Add("Descripcion", "DESCRIPCION");
                List_Response = _ldato.EjecutarReader(new ListaGeneral(), "PED_USP_CONSULTARESTATUSPEDIDOS", _ldato.Parametros, _ldato.Esquema);
                _Estatus = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PedidoRepository", "ObtenerEstatus()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARESTATUSPEDIDOS");
                throw new ApplicationException("ERROR EN: ObtenerEstatus()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Estatus;
        }

        public List<EstatusOrden> ObtenerEstatusOrden()
        {
            lDato _ldato = new lDato(_configVariables);
            List<EstatusOrden> _EstatusOrden = [];
            DataResponse<List<EstatusOrden>> List_Response = new DataResponse<List<EstatusOrden>>();
            try
            {
                _ldato.Esquema.Add("IdEstatus", "IDESTATUS");
                _ldato.Esquema.Add("Nombre", "NOMBRE");
                _ldato.Esquema.Add("Descripcion", "DESCRIPCION");
                _ldato.Esquema.Add("Orden", "ORDEN");
                _ldato.Esquema.Add("Icono", "ICONO");

                List_Response = _ldato.EjecutarReader(new EstatusOrden(), "PED_USP_CONSULTARESTATUSORDEN", _ldato.Parametros, _ldato.Esquema);
                _EstatusOrden = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PedidoRepository", "ObtenerEstatusOrden()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARESTATUSORDEN");
                throw new ApplicationException("ERROR EN: ObtenerEstatusOrden()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _EstatusOrden;

        }

        public List<SeguimientoOrden> ObtenerSeguimiento(int IdOrden)
        {
            lDato _ldato = new lDato(_configVariables);
            List<SeguimientoOrden> _SeguimientoOrden = [];
            DataResponse<List<SeguimientoOrden>> List_Response = new DataResponse<List<SeguimientoOrden>>();
            try
            {
                _ldato.Parametros.Add("@IDORDER", IdOrden);
                _ldato.Esquema.Add("IdSeguimiento", "IDSEGUIMIENTO");
                _ldato.Esquema.Add("IdOrder", "IDORDER");
                _ldato.Esquema.Add("IdEstatus", "IDESTATUS");
                _ldato.Esquema.Add("FechaModificacion", "FECHAMODIFICACION");
                _ldato.Esquema.Add("HoraModificacion", "HORAMODIFICACION");

                List_Response = _ldato.EjecutarReader(new SeguimientoOrden(), "PED_USP_CONSULTARSEGUIMIENTOORDEN", _ldato.Parametros, _ldato.Esquema);
                _SeguimientoOrden = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PedidoRepository", "ObtenerSeguimiento()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARSEGUIMIENTOORDEN");
                throw new ApplicationException("ERROR EN: ObtenerSeguimiento()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _SeguimientoOrden;

        }

        /// <summary>
        /// RETORNA EL ENCABEZADO DEL PEDIDO
        /// </summary>
        /// <param name="IdOrden">int</param>
        /// <param name="IdCliente">string</param>
        /// <returns>EncabezadoPedido</returns>
        public List<Descuento> ObtenerCalculoDescuentoComercial(int IdOrden, float Porcentaje)
        {
            lDato _ldato = new lDato(_configVariables);
            List<Descuento> _Encabezado = [];
            DataResponse<List<Descuento>> List_Response = new DataResponse<List<Descuento>>();
            try
            {

                _ldato.Parametros.Add("@IDORDEN", IdOrden);
                _ldato.Parametros.Add("@PORCDESC", Porcentaje);

                _ldato.Esquema.Add("BaseImponibleCesta", "BASE_IMPONIBLE_CESTA");
                _ldato.Esquema.Add("ImpuestoCesta", "IMPUESTO_CESTA");
                _ldato.Esquema.Add("DescuentoCesta", "DESCUENTO_CESTA");
                _ldato.Esquema.Add("TotalPagarCesta", "TOTAL_PAGAR_CESTA");

                List_Response = _ldato.EjecutarReader(new Descuento(), "PED_USP_CALCULARDESCUENTOCOMERCIAL", _ldato.Parametros, _ldato.Esquema);
                _Encabezado = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PedidoRepository", "ObtenerEncabezadoOrden()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARENCABEZADOORDEN");
                throw new ApplicationException("ERROR EN: ObtenerEncabezadoOrden()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Encabezado;
        }

        public List<PedidoPDFViewModel> ConsultarOrdenPdf(int IdOrden, int Tipo)
        {
            lDato _ldato = new lDato(_configVariables);
            List<PedidoPDFViewModel> _PedidoGenerado = [];
            DataResponse<List<PedidoPDFViewModel>> List_Response = new DataResponse<List<PedidoPDFViewModel>>();
            try
            {
                _ldato.Parametros.Add("@ORDERID", IdOrden);
                _ldato.Parametros.Add("@Modo", Tipo);
                _ldato.Esquema.Add("DateOrder", "DATE_ORDER");
                _ldato.Esquema.Add("PoNumber", "PO_NUMBER");
                _ldato.Esquema.Add("VendorName", "VENDOR_NAME");
                _ldato.Esquema.Add("VendorDir1", "VENDOR_DIR1");
                _ldato.Esquema.Add("VendorDir2", "VENDOR_DIR2");
                _ldato.Esquema.Add("VendorDir3", "VENDOR_DIR3");
                _ldato.Esquema.Add("VendorRuc", "VENDOR_RUC");
                _ldato.Esquema.Add("ConsigneeName", "CONSIGNEE_NAME");
                _ldato.Esquema.Add("ConsigneeDir1", "CONSIGNEE_DIR1");
                _ldato.Esquema.Add("ConsigneeDir2", "CONSIGNEE_DIR2");
                _ldato.Esquema.Add("ConsigneeDir3", "CONSIGNEE_DIR3");
                _ldato.Esquema.Add("PortDischarge", "PORT_OF_DISCHARGE");
                _ldato.Esquema.Add("CountryPortDischarge", "COUNTRY_PORT_OF_DISCHARGE");
                _ldato.Esquema.Add("CarrierName", "CARRIER_NAME");
                _ldato.Esquema.Add("CarrierDir1", "CARRIER_DIR1");
                _ldato.Esquema.Add("CarrierDir2", "CARRIER_DIR2");
                _ldato.Esquema.Add("CarrierDir3", "CARRIER_DIR3");
                _ldato.Esquema.Add("ItemCode", "ITEM_CODE");
                _ldato.Esquema.Add("ItemDescription", "ITEM_DESCRIPTION");
                _ldato.Esquema.Add("ItemQty", "ITEM_QTY");
                _ldato.Esquema.Add("ItemRate", "ITEM_RATE");
                _ldato.Esquema.Add("ItemAmount", "ITEM_AMOUNT");
                _ldato.Esquema.Add("TotalAmount", "TOTAL_NET_AMOUNT");
                _ldato.Esquema.Add("TotalQty", "TOTAL_QTY");
                _ldato.Esquema.Add("Payment", "PAYMENT");
                _ldato.Esquema.Add("Eta", "ETA");
                _ldato.Esquema.Add("Order", "ORDERN");
                _ldato.Esquema.Add("LeadTime", "LEAD_TIME");
                _ldato.Esquema.Add("FechaCorte", "FECHA_CORTE");
                _ldato.Esquema.Add("Phone", "PHONE");
                _ldato.Esquema.Add("Fax", "FAX");
                _ldato.Esquema.Add("EmpresaNombre", "EMPRESA_NOMBRE");
                _ldato.Esquema.Add("EmpresaDir1", "EMPRESA_DIR1");
                _ldato.Esquema.Add("EmpresaDir2", "EMPRESA_DIR2");
                _ldato.Esquema.Add("EmpresaDir3", "EMPRESA_DIR3");
                _ldato.Esquema.Add("EmpresaDir4", "EMPRESA_DIR4");
                _ldato.Esquema.Add("EmpresaRuc", "EMPRESA_RUC");
                _ldato.Esquema.Add("MotivoReactivacion", "MOTIVO_REACTIVACION");
                _ldato.Esquema.Add("NumeroRevision", "NUMERO_REVISION");
                _ldato.Esquema.Add("UsuarioReactivacion", "NOMBRE_USUARIO_REACTIVACION");
                _ldato.Esquema.Add("IdOrderOriginal", "ORDER_ID_ORIGINAL");
                _ldato.Esquema.Add("Moneda", "MONEDA");

                _ldato.Esquema.Add("Shipto_Linea1", "SHIPTO_LINEA1");
                _ldato.Esquema.Add("Shipto_Linea2", "SHIPTO_LINEA2");
                _ldato.Esquema.Add("Shipto_Linea3", "SHIPTO_LINEA3");
                _ldato.Esquema.Add("Forward_Linea1", "FORWARD_LINEA1");
                _ldato.Esquema.Add("Forward_Linea2", "FORWARD_LINEA2");
                _ldato.Esquema.Add("Forward_Linea3", "FORWARD_LINEA3");
                _ldato.Esquema.Add("Forward_Linea4", "FORWARD_LINEA4");
                _ldato.Esquema.Add("Forward_Linea5", "FORWARD_LINEA5");
                _ldato.Esquema.Add("Forward_Linea6", "FORWARD_LINEA6");
                _ldato.Esquema.Add("Forward_Linea7", "FORWARD_LINEA7");
                _ldato.Esquema.Add("Forward_Linea8", "FORWARD_LINEA8");
                _ldato.Esquema.Add("Incoterm", "INCOTERM");
                _ldato.Esquema.Add("Comments", "COMENTARIOS");
                _ldato.Esquema.Add("Discount", "Descuento");
                _ldato.Esquema.Add("ListaPrecio", "id_lista_precio");
                _ldato.Esquema.Add("Tax", "TAX");
                _ldato.Esquema.Add("NombreLogoProforma", "nombre_logo_proforma");
                _ldato.Esquema.Add("NombreLogoProduccion", "nombre_logo_produccion");
                //_ldato.Esquema.Add("VAT", "VAT");

                List_Response = _ldato.EjecutarReader(new PedidoPDFViewModel(), "PED_USP_IMPRIMIRORDEN", _ldato.Parametros, _ldato.Esquema);
                _PedidoGenerado = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PedidoRepository", "ConsultarOrdenPdf()", ex.Message.ToString(), null, null, "PED_USP_IMPRIMIRORDEN");
                throw new ApplicationException("ERROR EN: ConsultarOrdenPdf()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _PedidoGenerado;
        }

        public bool EliminarPedido(int IdOrden, string IdMotivo, int IdUsuario)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@IDORDER", IdOrden);
                _ldato.Parametros.Add("@DRKY", IdMotivo);
                _ldato.Parametros.Add("@USERID", IdUsuario);
                _data = _ldato.EjecutarScalarReader("PED_USP_ELIMINARPEDIDO", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PedidoRepository", "EliminarPedido()", ex.Message.ToString(), null, null, "CON_USP_ELIMINARPEDIDO");
            }
            return Convert.ToBoolean(_data.Valor);
        }

        public Dictionary<string, object> ReactivarPedido(int IdOrden, string IdMotivo, int IdUsuario)
        {
            lDato _ldato = new lDato(_configVariables);
            try
            {
                List<SqlParameter> _parametros = [ new SqlParameter() {ParameterName = "@ORDERID",  Value = IdOrden, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@DRKY",  Value = IdMotivo, SqlDbType = SqlDbType.VarChar, Size = 5},
                                                                          new SqlParameter() {ParameterName = "@USERID",  Value = IdUsuario, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@IDORDENOUT",  Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int},
                                                                          new SqlParameter() {ParameterName = "@RESULTADO", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.TinyInt},
                                                                         ];
                _ldato.ParametrosSql = _parametros;
                var resultado = _ldato.EjecutarNonQueryOutput("PED_USP_REACTIVAR_PEDIDO_SEGMENTADO", _ldato.ParametrosSql);
                return resultado.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PedidoRepository", "ReactivarPedido()", ex.ToString(), null, null, "PED_USP_REACTIVAR_PEDIDO");
                throw new ApplicationException("ERROR EN: ReactivarPedido", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            ;
        }

        public Dictionary<string, object> EnviarProforma(int IdOrden)
        {
            lDato _ldato = new lDato(_configVariables);
            try
            {
                List<SqlParameter> _parametros = [new SqlParameter() { ParameterName = "@ORDERID", Value = IdOrden, SqlDbType = SqlDbType.Int }];
                _ldato.ParametrosSql = _parametros;
                var resultado = _ldato.EjecutarNonQueryOutput("PED_USP_ENVIARPROFORMACORREO", _ldato.ParametrosSql);
                return resultado.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"EnviarProforma", "EnviarProforma()", ex.ToString(), null, null, "PED_USP_REACTIVAR_PEDIDO");
                throw new ApplicationException("ERROR EN: EnviarProforma", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            ;
        }

        public Dictionary<string, object> AsignarPoNumber(int IdOrden)
        {
            lDato _ldato = new lDato(_configVariables);
            try
            {
                List<SqlParameter> _parametros = [new SqlParameter() { ParameterName = "@ORDERID", Value = IdOrden, SqlDbType = SqlDbType.Int }];
                _ldato.ParametrosSql = _parametros;
                var resultado = _ldato.EjecutarNonQueryOutput("PED_USP_ASIGNARPONUMBER", _ldato.ParametrosSql);
                return resultado.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"AsignarPoNumber", "AsignarPoNumber()", ex.ToString(), null, null, "PED_USP_ASIGNARPONUMBER");
                throw new ApplicationException("ERROR EN: EnviarProforma", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            ;
        }

        public List<HistoriaOrder> ConsultarHistoricoPedidos(int IdOrden)
        {
            lDato _ldato = new lDato(_configVariables);
            List<HistoriaOrder> _Pedidos = [];
            DataResponse<List<HistoriaOrder>> List_Response = new DataResponse<List<HistoriaOrder>>();
            try
            {
                _ldato.Parametros.Add("@IDORDEN", IdOrden);


                _ldato.Esquema.Add("OrderId", "ORDER_ID");
                _ldato.Esquema.Add("IdRevision", "ID_REVISION");
                _ldato.Esquema.Add("Fecha", "FECHA");
                _ldato.Esquema.Add("IdUsuario", "ID_USUARIO");
                _ldato.Esquema.Add("OrderIdOriginal", "ORDER_ID_ORIGINAL");
                _ldato.Esquema.Add("UltimoEnviado", "ULTIMO_ENVIADO");
                _ldato.Esquema.Add("Comentario", "COMENTARIO");
                _ldato.Esquema.Add("NumCorrelativo", "order_number");

                List_Response = _ldato.EjecutarReader(new HistoriaOrder(), "PED_USP_CONSULTARHISTORIAREVISIONORDER", _ldato.Parametros, _ldato.Esquema);
                _Pedidos = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PedidoRepository", "ConsultarHistoricoPedidos()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARHISTORIAREVISIONORDER");
                throw new ApplicationException("ERROR EN: ConsultarHistoricoPedidos()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return _Pedidos;
        }

        public List<ListaGeneral> ConsultarCondicionesDePago()
        {
            lDato _ldato = new lDato(_configVariables);
            List<ListaGeneral> Lista = [];
            DataResponse<List<ListaGeneral>> List_Response = new DataResponse<List<ListaGeneral>>();
            try
            {
                _ldato.Esquema.Add("Codigo", "ID");
                _ldato.Esquema.Add("Descripcion", "DESCRIPCION");
                List_Response = _ldato.EjecutarReader(new ListaGeneral(), "PED_USP_CONSULTARCONDICIONPAGO", _ldato.Parametros, _ldato.Esquema);
                Lista = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PedidoRepository", "ConsultarCondicionesDePago()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARCONDICIONPAGO");
                throw new ApplicationException("ERROR EN: ConsultarCondicionesDePago()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return Lista;
        }

        public List<ListaGeneral> ConsultarForwardingAgent()
        {
            lDato _ldato = new lDato(_configVariables);
            List<ListaGeneral> Lista = [];
            DataResponse<List<ListaGeneral>> List_Response = new DataResponse<List<ListaGeneral>>();
            try
            {
                _ldato.Esquema.Add("IdTipo", "codigo");
                _ldato.Esquema.Add("Descripcion", "descripcion");
                List_Response = _ldato.EjecutarReader(new ListaGeneral(), "PED_USP_CONSULTARAGENTES", _ldato.Parametros, _ldato.Esquema);
                Lista = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PedidoRepository", "ConsultarForwardingAgent()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARAGENTES");
                throw new ApplicationException("ERROR EN: ConsultarForwardingAgent()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return Lista;
        }

        public List<ListaGeneral> ConsultarPaisesFacturacion(string IdCli)
        {
            lDato _ldato = new lDato(_configVariables);
            List<ListaGeneral> Lista = [];
            DataResponse<List<ListaGeneral>> List_Response = new DataResponse<List<ListaGeneral>>();
            try
            {
                _ldato.Parametros.Add("@CUSTOMER_ID", IdCli);

                _ldato.Esquema.Add("Codigo", "codigo");
                _ldato.Esquema.Add("Descripcion", "descripcion");
                List_Response = _ldato.EjecutarReader(new ListaGeneral(), "PED_USP_CONSULTARPAISFACTURACION", _ldato.Parametros, _ldato.Esquema);
                Lista = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PedidoRepository", "ConsultarPaisesFacturacion()", ex.Message.ToString(), null, null, "PED_USP_CONSULTARPAISFACTURACION");
                throw new ApplicationException("ERROR EN: ConsultarPaisesFacturacion()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return Lista;
        }

        public List<ListaGeneral> ConsultarContadoresAnnoPais(string codigoPais)
        {
            lDato _ldato = new lDato(_configVariables);
            List<ListaGeneral> Lista = [];
            DataResponse<List<ListaGeneral>> List_Response = new DataResponse<List<ListaGeneral>>();
            try
            {
                _ldato.Parametros.Add("@CODPAIS", string.IsNullOrWhiteSpace(codigoPais) ? Convert.DBNull : codigoPais);

                _ldato.Esquema.Add("IdTipo", "anno");
                _ldato.Esquema.Add("Descripcion", "descripcion");
                List_Response = _ldato.EjecutarReader(new ListaGeneral(), "PED_USP_OBTENERCONTADORESANNOPAIS", _ldato.Parametros, _ldato.Esquema);
                Lista = List_Response.Valor;
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PedidoRepository", "ConsultarAnnosCorrelativos()", ex.Message.ToString(), null, null, "PED_USP_OBTENERCONTADORESANNOPAIS");
                throw new ApplicationException("ERROR EN: ConsultarAnnosCorrelativos()", ex);
            }
            finally
            {
                _ldato.Dispose();;
            }
            return Lista;
        }

        public bool RecalcularPrecioOrden(int IdOrden, int IdUsuario)
        {
            DataResponse<object> _data = new DataResponse<object>();
            lDato _ldato = new lDato(_configVariables);
            try
            {
                _ldato.Parametros.Add("@ORDERID", IdOrden);
                _ldato.Parametros.Add("@USERID", IdUsuario);
                _data = _ldato.EjecutarScalarReader("PED_USP_RECALCULARPRECIOORDEN", _ldato.Parametros);
            }
            catch (Exception ex)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"PedidoRepository", "RecalcularPrecioOrden()", ex.Message.ToString(), null, null, "CON_USP_ELIMINARPEDIDO");
            }
            return Convert.ToBoolean(_data.Valor);
        }

    }

}
