using Microsoft.AspNetCore.Mvc;
using Proyecto_Catedra_Medicamento.Data;
using Proyecto_Catedra_Medicamento.Models;
using Proyecto_Catedra_Medicamento.Models.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;

namespace Proyecto_Catedra_Medicamento.Controllers
{
    public class MedicamentosController : Controller
    {
        private readonly AppDbContext db;

        public MedicamentosController(AppDbContext context)
        {
            db = context;
        }

        public IActionResult RegistrarIngreso()
        {
            ViewBag.Proveedores = db.Proveedores.ToList();
            ViewBag.Medicamentos = db.Medicamentos.ToList();
            return View();
        }

        public IActionResult RegistrarMedicamento()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegistrarMedicamento(string nombre, string presentacion, string marca, int cantidad, DateTime fecha_vencimiento, string proveedor)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(nombre))
                ModelState.AddModelError("nombre", "El nombre del medicamento es obligatorio.");

            if (string.IsNullOrWhiteSpace(presentacion))
                ModelState.AddModelError("presentacion", "La presentación es obligatoria.");

            if (cantidad <= 0)
                ModelState.AddModelError("cantidad", "La cantidad debe ser mayor a cero.");

            if (string.IsNullOrWhiteSpace(proveedor))
                ModelState.AddModelError("proveedor", "El proveedor es obligatorio.");

            if (fecha_vencimiento < DateTime.Today)
                ModelState.AddModelError("fecha_vencimiento", "La fecha de vencimiento no puede ser anterior a hoy.");

            if (!ModelState.IsValid)
                return View();

            // Buscar proveedor existente
            var proveedorExistente = db.Proveedores.FirstOrDefault(p => p.nombre.ToLower() == proveedor.ToLower());

            if (proveedorExistente == null)
            {
                proveedorExistente = new Proveedor { nombre = proveedor };
                db.Proveedores.Add(proveedorExistente);
                db.SaveChanges();
            }

            // Registrar medicamento
            var medicamento = new Medicamento
            {
                nombre = nombre,
                presentacion = presentacion,
                marca = marca
            };
            db.Medicamentos.Add(medicamento);
            db.SaveChanges();

            // Registrar lote
            var lote = new Lote
            {
                id_medicamento = medicamento.id_medicamento,
                fecha_vencimiento = fecha_vencimiento,
                id_proveedor = proveedorExistente.id_proveedor
            };
            db.Lotes.Add(lote);
            db.SaveChanges();

            // Registrar entrada
            var claimUsuario = User.FindFirstValue("IdUsuario");

            if (string.IsNullOrEmpty(claimUsuario))
            {
                TempData["Error"] = "No se pudo identificar al usuario actual.";
                return RedirectToAction("RegistrarMedicamento");
            }

            var id_usuario = int.Parse(claimUsuario);

            var entrada = new Entrada
            {
                fecha = DateTime.Now,
                cantidad = cantidad,
                id_lote = lote.id_lote,
                id_usuario = id_usuario
            };
            db.Entradas.Add(entrada);
            db.SaveChanges();

            TempData["Success"] = "Medicamento y proveedor registrados correctamente.";
            return RedirectToAction("RegistrarMedicamento");
        }

        public IActionResult ListadoMedicamentos()
        {
            var lista = (from m in db.Medicamentos
                         join l in db.Lotes on m.id_medicamento equals l.id_medicamento
                         join e in db.Entradas on l.id_lote equals e.id_lote
                         join p in db.Proveedores on l.id_proveedor equals p.id_proveedor
                         group new { m, l, e, p } by new
                         {
                             m.id_medicamento,
                             m.nombre,
                             m.presentacion,
                             m.marca,
                             l.fecha_vencimiento,
                             nombre_proveedor = p.nombre
                         } into grupo
                         select new MedicamentoViewModel
                         {
                             id_medicamento = grupo.Key.id_medicamento,
                             nombre = grupo.Key.nombre,
                             presentacion = grupo.Key.presentacion,
                             marca = grupo.Key.marca,
                             fecha_vencimiento = grupo.Key.fecha_vencimiento,
                             proveedor = grupo.Key.nombre_proveedor,
                             cantidad_total = grupo.Sum(x => x.e.cantidad)
                         }).ToList();

            return View("ListadoMedicamentos", lista);
        }

        public IActionResult ListadoProveedores()
        {
            var proveedores = db.Proveedores
                .OrderBy(p => p.nombre)
                .ToList();

            return View("ListadoProveedores", proveedores);
        }

        
        public IActionResult ListadoEntradas()
        {
            var lista = (from e in db.Entradas
                         join l in db.Lotes on e.id_lote equals l.id_lote
                         join m in db.Medicamentos on l.id_medicamento equals m.id_medicamento
                         join p in db.Proveedores on l.id_proveedor equals p.id_proveedor
                         join u in db.Usuario on e.id_usuario equals u.id_usuario
                         select new EntradaViewModel
                         {
                             nombre_medicamento = m.nombre,
                             nombre_proveedor = p.nombre,
                             fecha_vencimiento = l.fecha_vencimiento,
                             cantidad = e.cantidad,
                             fecha = e.fecha,
                             nombre_usuario = u.nombre
                         }).ToList();

            return View("ListadoEntradas", lista);
        }


    }
}
