using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PedidosWebUpgrade.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedidosWebUpgrade.Infrastructure.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Almacen> Almacen { get; set; }
        public DbSet<Compannia> Compannia { get; set; }
        public DbSet<Configuracion> Configuracion { get; set; }
        public DbSet<ContadorPedidosPais> ContadorPedidosPais { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Descuento> Descuento { get; set; }
        public DbSet<Email> Email { get; set; }
        public DbSet<EstadoCuenta> EstadoCuenta { get; set; }
        public DbSet<EstatusOrden> EstatusOrden { get; set; }
        public DbSet<F0004> F0004 { get; set; }
        public DbSet<F0005> F0005 { get; set; }
        public DbSet<F45520> F45520 { get; set; }
        public DbSet<F45521> F45521 { get; set; }
        public DbSet<ForwardingAgent> ForwardingAgent { get; set; }
        public DbSet<HistoriaOrder> HistoriaOrder { get; set; }
        public DbSet<ListaGeneral> ListaGeneral { get; set; }
        public DbSet<ListPreciosPrint> ListPreciosPrint { get; set; }
        public DbSet<Menu> Menu { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Pago> Pago { get; set; }
        public DbSet<Perfil> Perfil { get; set; }
        public DbSet<PerfilMenu> PerfilMenu { get; set; }
        public DbSet<PerfilUsuario> PerfilUsuario { get; set; }
        public DbSet<PreferenciaAlmacenClientePais> PreferenciaAlmacenClientePais { get; set; }
        public DbSet<PreferenciaClientePais> PreferenciaClientePais { get; set; }
        public DbSet<PreferenciaItemDestino> PreferenciaItemDestino { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Producto> Producto { get; set; }
        public DbSet<Salesman> Salesman { get; set; }
        public DbSet<SeguimientoOrden> SeguimientoOrden { get; set; }
        public DbSet<Sistema> Sistema { get; set; }
        public DbSet<TipoCambio> TipoCambio { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Vendedor> Vendedor { get; set; }
    }
}
