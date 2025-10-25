using Microsoft.AspNetCore.Mvc;
using Proyecto_Catedra_Medicamento.Data;
using Proyecto_Catedra_Medicamento.Models;
using Microsoft.EntityFrameworkCore;

namespace Proyecto_Catedra_Medicamento.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        // Vista principal: lista de usuarios
        public IActionResult Index()
        {
            var usuarios = _context.Usuario
                .OrderBy(u => u.nombre) // Orden institucional por nombre
                .ToList();

            return View("UsuarioIndex", usuarios);
        }

        // Vista para registrar nuevo usuario
        public IActionResult Create()
        {
            return View();
        }

        // POST para guardar nuevo usuario
        [HttpPost]
        public IActionResult Create(Usuario nuevoUsuario)
        {
            ModelState.Remove("rol");

            bool esPrimerUsuario = !_context.Usuario.Any();
            nuevoUsuario.rol = esPrimerUsuario ? "Administrador" : "Operador";

            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Por favor complete todos los campos correctamente.";
                return View(nuevoUsuario);
            }

            _context.Usuario.Add(nuevoUsuario);
            _context.SaveChanges();

            TempData["RegistroExitoso"] = "Usuario registrado correctamente.";
            return RedirectToAction("Index");
        }

        // Vista para editar usuario existente
        public IActionResult Edit(int id)
        {
            var usuario = _context.Usuario.FirstOrDefault(u => u.id_usuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST para guardar cambios del usuario (protegiendo rol y contraseña)
        [HttpPost]
        public IActionResult Edit(Usuario usuarioActualizado)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Por favor revise los campos antes de guardar.";
                return View(usuarioActualizado);
            }

            var usuarioExistente = _context.Usuario.FirstOrDefault(u => u.id_usuario == usuarioActualizado.id_usuario);
            if (usuarioExistente == null)
            {
                return NotFound();
            }

            // Actualizamos campos manualmente
            usuarioExistente.nombre = usuarioActualizado.nombre;
            usuarioExistente.usuario = usuarioActualizado.usuario;

            // Solo actualizamos la contraseña si se escribió una nueva
            if (!string.IsNullOrWhiteSpace(usuarioActualizado.contrasena))
            {
                usuarioExistente.contrasena = usuarioActualizado.contrasena;
            }

            usuarioExistente.correo = usuarioActualizado.correo;
            usuarioExistente.telefono = usuarioActualizado.telefono;
            usuarioExistente.departamento = usuarioActualizado.departamento;
            usuarioExistente.municipio = usuarioActualizado.municipio;

            // No se modifica usuarioExistente.rol

            _context.SaveChanges();

            TempData["EdicionExitosa"] = "Datos del usuario actualizados correctamente.";
            return RedirectToAction("Index");
        }

        // Vista para confirmar eliminación
        public IActionResult Delete(int id)
        {
            var usuario = _context.Usuario.FirstOrDefault(u => u.id_usuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST para ejecutar la eliminación
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var usuario = _context.Usuario.FirstOrDefault(u => u.id_usuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuario.Remove(usuario);
            _context.SaveChanges();

            TempData["EliminacionExitosa"] = "Usuario eliminado correctamente.";
            return RedirectToAction("Index");
        }
    }
}
