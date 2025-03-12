using PedidosWebUpgrade.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IEmpresaRepository
    {
        List<ListaGeneral> ObtenerSucursales(string salesmanid);

        List<Compannia> ObtenerEmpresa();
    }
}
