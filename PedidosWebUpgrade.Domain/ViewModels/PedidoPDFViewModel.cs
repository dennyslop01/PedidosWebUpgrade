using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedidosWebUpgrade.Domain.ViewModels
{
    public class PedidoPDFViewModel
    {
        public string DateOrder { get; set; } = string.Empty;
        public string PoNumber { get; set; } = string.Empty;
        public string VendorName { get; set; } = string.Empty;
        public string VendorDir1 { get; set; } = string.Empty;
        public string VendorDir2 { get; set; } = string.Empty;
        public string VendorDir3 { get; set; } = string.Empty;
        public string VendorRuc { get; set; } = string.Empty;
        public string ConsigneeName { get; set; } = string.Empty;
        public string ConsigneeDir1 { get; set; } = string.Empty;
        public string ConsigneeDir2 { get; set; } = string.Empty;
        public string ConsigneeDir3 { get; set; } = string.Empty;
        public string PortDischarge { get; set; } = string.Empty;
        public string CountryPortDischarge { get; set; } = string.Empty;
        public string CarrierName { get; set; } = string.Empty;
        public string CarrierDir1 { get; set; } = string.Empty;
        public string CarrierDir2 { get; set; } = string.Empty;
        public string CarrierDir3 { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public string ItemDescription { get; set; } = string.Empty;
        public string ItemQty { get; set; } = string.Empty;
        public string ItemRate { get; set; } = string.Empty;
        public string ItemAmount { get; set; } = string.Empty;
        public string TotalAmount { get; set; } = string.Empty;
        public string TotalQty { get; set; } = string.Empty;
        public string Payment { get; set; } = string.Empty;
        public string Eta { get; set; } = string.Empty;
        public string Order { get; set; } = string.Empty;
        public string LeadTime { get; set; } = string.Empty;
        public string FechaCorte { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Fax { get; set; } = string.Empty;
        public string EmpresaNombre { get; set; } = string.Empty;
        public string EmpresaDir1 { get; set; } = string.Empty;
        public string EmpresaDir2 { get; set; } = string.Empty;
        public string EmpresaDir3 { get; set; } = string.Empty;
        public string EmpresaDir4 { get; set; } = string.Empty;
        public string EmpresaRuc { get; set; } = string.Empty;
        public string MotivoReactivacion { get; set; } = string.Empty;
        public int NumeroRevision { get; set; } = 0;
        public string UsuarioReactivacion { get; set; } = string.Empty;
        public int IdOrderOriginal { get; set; } = 0;
        public string Moneda { get; set; } = string.Empty;
        public string Shipto_Linea1 { get; set; } = string.Empty;
        public string Shipto_Linea2 { get; set; } = string.Empty;
        public string Shipto_Linea3 { get; set; } = string.Empty;
        public string Forward_Linea1 { get; set; } = string.Empty;
        public string Forward_Linea2 { get; set; } = string.Empty;
        public string Forward_Linea3 { get; set; } = string.Empty;
        public string Forward_Linea4 { get; set; } = string.Empty;
        public string Forward_Linea5 { get; set; } = string.Empty;
        public string Forward_Linea6 { get; set; } = string.Empty;
        public string Forward_Linea7 { get; set; } = string.Empty;
        public string Forward_Linea8 { get; set; } = string.Empty;
        public string Incoterm { get; set; } = string.Empty;
        public string? Comments { get; set; }
        public string? Discount { get; set; }
        public string? ListaPrecio { get; set; }
        public string? Tax { get; set; }

        public string? NombreLogoProforma { get; set; }
        public string? NombreLogoProduccion { get; set; }
        public string? VAT { get; set; }

    }

}
