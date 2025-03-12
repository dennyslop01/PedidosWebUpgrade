using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PedidosWebUpgrade.Application.Common.Interfaces;
using PedidosWebUpgrade.Infrastructure.Repository;
using PedidosWebUpgrade.Infrastructure.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<ConfigVariables>(builder.Configuration.GetSection("Principal"));
builder.Services.AddSingleton(es => es.GetRequiredService<IOptions<ConfigVariables>>().Value);

builder.Services.AddScoped<IAlmacenRepository, AlmacenRepository>();
builder.Services.AddScoped<ICestaRepository, CestaRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ICobranzaRepository, CobranzaRepository>();
builder.Services.AddScoped<IConfiguracionRepository, ConfiguracionRepository>();
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
