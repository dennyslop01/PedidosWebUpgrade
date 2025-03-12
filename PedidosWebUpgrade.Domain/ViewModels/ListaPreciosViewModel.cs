using PedidosWebUpgrade.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedidosWebUpgrade.Domain.ViewModels
{
    public class ListaPreciosViewModel
    {

        public ListaPreciosViewModel()
        {
            this.ListCabListPrecios = new List<F45520>();
            this.ListDetListPrecios = new List<F45521>();
            this.CabListPrecios = new F45520();
            this.DetListPrecios = new F45521();
        }
        public List<F45520> ListCabListPrecios { get; set; }
        public List<F45521> ListDetListPrecios { get; set; }
        public F45520 CabListPrecios { get; set; }
        public F45521 DetListPrecios { get; set; }
    }
}
