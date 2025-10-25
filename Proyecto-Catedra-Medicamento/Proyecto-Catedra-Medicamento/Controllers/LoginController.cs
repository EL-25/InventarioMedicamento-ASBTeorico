using Microsoft.AspNetCore.Mvc;
using Proyecto_Catedra_Medicamento.Models;
using Proyecto_Catedra_Medicamento.Data;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;

namespace Proyecto_Catedra_Medicamento.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        // Vista GET para mostrar el formulario de login
        [HttpGet]
        public IActionResult Login()
        {
            return View(); // Muestra Login/Login.cshtml
        }

        // Vista POST para procesar el login
        [HttpPost]
        public async Task<IActionResult> Login(string usuario, string contrasena)
        {
            // Validación básica de campos vacíos
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena))
            {
                ViewBag.Error = "Debe ingresar usuario y contraseña.";
                return View();
            }

            // Normalizamos entrada para comparación segura
            var usuarioInput = usuario.Trim().ToLower();
            var contrasenaInput = contrasena.Trim();

            // Consulta en base de datos (sin StringComparison por compatibilidad con MySQL)
            var usuarioEncontrado = _context.Usuario
                .FirstOrDefault(u =>
                    u.usuario.ToLower() == usuarioInput &&
                    u.contrasena == contrasenaInput);

            if (usuarioEncontrado != null)
            {
                // Creamos los claims para sesión real
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuarioEncontrado.nombre),
                    new Claim(ClaimTypes.Role, usuarioEncontrado.rol)
                };

                // Creamos la identidad y el principal
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Activamos la sesión con cookies
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Redirección según rol
                if (usuarioEncontrado.rol == "Administrador")
                    return RedirectToAction("Index", "Admin");
                else
                    return RedirectToAction("Index", "Operador");
            }

            // Si no se encuentra el usuario
            ViewBag.Error = "Credenciales incorrectas.";
            return View();
        }

        // Cierre de sesión
        public async Task<IActionResult> Logout()
        {
            // Cerramos sesión y redirigimos al login
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login");
        }
    }
}
