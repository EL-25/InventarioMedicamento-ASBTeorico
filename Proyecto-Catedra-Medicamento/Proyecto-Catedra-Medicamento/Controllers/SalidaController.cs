using Proyecto_Catedra_Medicamento.Data;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Catedra_Medicamento.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Proyecto_Catedra_Medicamento.Controllers
{
    public class SalidaController : Controller
    {
        private readonly AppDbContext _contexto;

        public SalidaController(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        // Mostrar formulario de registro de salida
        public IActionResult Index()
        {
            CargarDatosFormulario();
            return View("SalidaIndex", new Salida());
        }

        // Procesar registro de salida
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Salida salida)
        {
            salida.fecha = DateTime.Now;

            if (string.IsNullOrWhiteSpace(salida.observaciones))
            {
                salida.observaciones = "Salida regular sin observaciones";
            }

            Console.WriteLine("ModelState válido: " + ModelState.IsValid);

            if (!ModelState.IsValid)
            {
                CargarDatosFormulario();
                return View("SalidaIndex", salida);
            }

            var lote = _contexto.Lotes.FirstOrDefault(l => l.id_lote == salida.id_lote);

            if (lote == null || lote.cantidad < salida.cantidad)
            {
                ModelState.AddModelError("cantidad", "La cantidad solicitada excede el stock disponible.");
                CargarDatosFormulario();
                return View("SalidaIndex", salida);
            }

            lote.cantidad -= salida.cantidad;

            _contexto.Salidas.Add(salida);
            _contexto.Lotes.Update(lote);
            _contexto.SaveChanges();

            CargarDatosFormulario();
            ViewBag.Mensaje = "Salida registrada correctamente.";
            return View("SalidaIndex", new Salida());
        }

        // Mostrar listado de salidas
        [HttpGet]
        public IActionResult Listado()
        {
            var salidas = _contexto.Salidas
                .Include(s => s.Lote)
                .Include(s => s.Usuario)
                .OrderByDescending(s => s.fecha)
                .ToList();

            foreach (var salida in salidas)
            {
                if (salida.observaciones == null)
                {
                    salida.observaciones = "";
                }
            }

            return View(salidas);
        }

        // Exportar historial a PDF
        public IActionResult ExportarPDF()
        {
            var salidas = _contexto.Salidas
                .Include(s => s.Lote)
                .Include(s => s.Usuario)
                .OrderByDescending(s => s.fecha)
                .ToList();

            using (var ms = new MemoryStream())
            {
                var doc = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                var bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                var italicFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 10);

                doc.Add(new Paragraph("Historial de Salidas de Medicamentos", titleFont));
                doc.Add(new Paragraph($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}", bodyFont));
                doc.Add(new Paragraph(" "));

                if (!salidas.Any())
                {
                    doc.Add(new Paragraph("No hay salidas registradas en el sistema.", italicFont));
                }
                else
                {
                    var table = new PdfPTable(6) { WidthPercentage = 100 };
                    table.SetWidths(new float[] { 1, 2, 2, 1, 2, 3 });

                    string[] headers = { "ID", "Fecha", "Lote", "Cantidad", "Usuario", "Observaciones" };
                    foreach (var h in headers)
                    {
                        var cell = new PdfPCell(new Phrase(h, headerFont))
                        {
                            BackgroundColor = new BaseColor(211, 211, 211),
                            HorizontalAlignment = Element.ALIGN_CENTER
                        };
                        table.AddCell(cell);
                    }

                    int totalCantidad = 0;

                    foreach (var s in salidas)
                    {
                        table.AddCell(new Phrase(s.id_salida.ToString(), bodyFont));
                        table.AddCell(new Phrase(s.fecha.ToString("dd/MM/yyyy"), bodyFont));
                        table.AddCell(new Phrase(s.Lote.descripcion, bodyFont));
                        table.AddCell(new Phrase(s.cantidad.ToString(), bodyFont));
                        table.AddCell(new Phrase(s.Usuario.nombre, bodyFont));
                        table.AddCell(new Phrase(s.observaciones ?? "", bodyFont));

                        totalCantidad += s.cantidad;
                    }

                    doc.Add(table);
                    doc.Add(new Paragraph(" "));
                    doc.Add(new Paragraph($"Total de unidades dispensadas: {totalCantidad}", headerFont));
                    doc.Add(new Paragraph("Documento generado automáticamente para fines de control y auditoría institucional.", italicFont));
                }

                doc.Close();
                return File(ms.ToArray(), "application/pdf", "Historial_Salidas.pdf");
            }
        }

        // ✅ Método centralizado para cargar lotes y usuarios
        private void CargarDatosFormulario()
        {
            ViewBag.Lotes = _contexto.Lotes
                .Where(l => l.cantidad > 0)
                .Select(l => new { l.id_lote, l.descripcion })
                .ToList();

            ViewBag.Usuarios = _contexto.Usuario
                .Select(u => new { u.id_usuario, u.nombre })
                .ToList();
        }
    }
}
