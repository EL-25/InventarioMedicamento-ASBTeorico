using Microsoft.EntityFrameworkCore;
using Proyecto_Catedra_Medicamento.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Configurar la cadena de conexi�n desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");

// Registrar el DbContext con MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Agregar servicios de autenticaci�n con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Login";       // Ruta al formulario de login
        options.LogoutPath = "/Login/Logout";     // Ruta para cerrar sesi�n
        options.AccessDeniedPath = "/Home/Error"; // Ruta si el acceso es denegado
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configuraci�n del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Activar autenticaci�n y autorizaci�n
app.UseAuthentication();
app.UseAuthorization();

// Configurar rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
