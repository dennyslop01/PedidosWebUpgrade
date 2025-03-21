using PedidosWebUpgrade.Domain.Entities;

namespace PedidosWebUpgrade.Application.Common.Interfaces
{
    public interface IAlmacenRepository
    {
        Task<Dictionary<string, object>> ActualizarAlmacen(Almacen _Almacen);

        Task<List<Almacen>> ConsultarAlmacenes(int Id);

        Task<int> EliminarAlmacen(int Id);
    }
}
