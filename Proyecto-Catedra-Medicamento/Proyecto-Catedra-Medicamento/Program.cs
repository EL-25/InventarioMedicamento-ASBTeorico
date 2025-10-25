using Microsoft.EntityFrameworkCore;
using Proyecto_Catedra_Medicamento.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurar la cadena de conexión desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");

// Registrar el DbContext con MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
