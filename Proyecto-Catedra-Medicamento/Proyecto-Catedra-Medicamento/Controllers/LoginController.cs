using Microsoft.AspNetCore.Mvc;
using Proyecto_Catedra_Medicamento.Models;
using Proyecto_Catedra_Medicamento.Data;
using System.Linq;

namespace Proyecto_Catedra_Medicamento.Controllers
{
    // Este controlador se encarga SOLO del login
    public class LoginController : Controller
    {
        // Creamos una variable privada para acceder a la base de datos
        private readonly AppDbContext _context;

        // Constructor que recibe el contexto de base de datos y lo guarda en la variable privada
        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        // Acción que muestra el formulario de login (cuando el usuario entra a la página)
        [HttpGet]
        public IActionResult Login()
        {
            // Muestra la vista Login/Login.cshtml
            return View();
        }

        // Acción que procesa el login (cuando el usuario hace clic en "Ingresar")
        [HttpPost]
        public IActionResult Login(string usuario, string contrasena)
        {
            // Validamos que el usuario haya escrito algo en ambos campos
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena))
            {
                // Si falta algún campo, mostramos un mensaje de error
                ViewBag.Error = "Debe ingresar usuario y contraseña.";
                return View(); // Volvemos a mostrar el formulario
            }

            // Buscamos en la base de datos un usuario que coincida con el nombre de usuario y la contraseña
            var usuarioEncontrado = _context.Usuario
                .FirstOrDefault(u => u.usuario.Trim() == usuario.Trim() && u.contrasena.Trim() == contrasena.Trim());

            // Si encontramos un usuario válido...
            if (usuarioEncontrado != null)
            {
                // Guardamos temporalmente el nombre y rol del usuario (puede usarse en otras vistas)
                TempData["NombreUsuario"] = usuarioEncontrado.nombre;
                TempData["Rol"] = usuarioEncontrado.rol;

                // Redirigimos según el rol del usuario
                if (usuarioEncontrado.rol == "Administrador")
                    return RedirectToAction("Index", "Admin"); // Va al controlador Admin
                else
                    return RedirectToAction("Index", "Operador"); // Va al controlador Operador
            }

            // Si no encontramos el usuario, mostramos error
            ViewBag.Error = "Credenciales incorrectas.";
            return View(); // Volvemos a mostrar el formulario
        }
    }
}