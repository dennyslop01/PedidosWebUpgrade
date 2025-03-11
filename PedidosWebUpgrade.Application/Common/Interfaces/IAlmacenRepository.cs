using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IAlmacenRepository
    {
        Dictionary<string, object> ActualizarAlmacen(Almacen _Almacen);

        List<Almacen> ConsultarAlmacenes(int Id);

        int EliminarAlmacen(int Id);
    }
}
