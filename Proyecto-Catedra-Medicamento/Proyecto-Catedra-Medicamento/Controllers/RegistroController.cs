using Microsoft.AspNetCore.Mvc;
using Proyecto_Catedra_Medicamento.Models;
using Proyecto_Catedra_Medicamento.Data;

namespace Proyecto_Catedra_Medicamento.Controllers
{
    public class RegistroController : Controller
    {
        private readonly AppDbContext _context;

        public RegistroController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View(new Usuario());
        }

        [HttpPost]
        public IActionResult Registro(Usuario nuevoUsuario)
        {
            // Asignamos el rol automáticamente ANTES de validar
            bool esPrimerUsuario = !_context.Usuario.Any();
            nuevoUsuario.rol = esPrimerUsuario ? "Administrador" : "Operador";

            // Eliminar validación automática de 'rol' (no viene del formulario)
            ModelState.Remove("rol");

            // 🔍 Imprimir valores clave en consola (sin contraseña)
            Console.WriteLine("=== Registro recibido ===");
            Console.WriteLine($"Nombre: {nuevoUsuario.nombre}");
            Console.WriteLine($"Usuario: {nuevoUsuario.usuario}");
            Console.WriteLine($"Teléfono: {nuevoUsuario.telefono}");
            Console.WriteLine($"Correo: {nuevoUsuario.correo}");
            Console.WriteLine($"Departamento: {nuevoUsuario.departamento}");
            Console.WriteLine($"Municipio: {nuevoUsuario.municipio}");
            Console.WriteLine($"Rol: {nuevoUsuario.rol}");
            Console.WriteLine("=========================");

            // Imprimir estado de validación
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
            foreach (var kvp in ModelState)
            {
                var key = kvp.Key;
                var errors = kvp.Value.Errors;
                foreach (var error in errors)
                {
                    Console.WriteLine($"Error en '{key}': {error.ErrorMessage}");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Por favor complete todos los campos correctamente.";
                return View(nuevoUsuario);
            }

            _context.Usuario.Add(nuevoUsuario);
            _context.SaveChanges();

            TempData["RegistroExitoso"] = "Usuario registrado correctamente.";
            return RedirectToAction("Login", "Login");
        }
    }
}