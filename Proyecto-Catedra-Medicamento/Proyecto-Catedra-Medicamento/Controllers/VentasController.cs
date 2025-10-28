using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Catedra_Medicamento.Data;
using Proyecto_Catedra_Medicamento.Models;
using Proyecto_Catedra_Medicamento.Models.ViewModels;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Proyecto_Catedra_Medicamento.Controllers
{
    public class VentasController : Controller
    {
        private readonly AppDbContext _context;

        public VentasController(AppDbContext context)
        {
            _context = context;
        }

        // ============================
        // VISTA: Registrar nueva venta
        // ============================
        public IActionResult RegistrarVenta()
        {
            var lotes = _context.Lotes
                .Include(l => l.Medicamento)
                .Include(l => l.Entradas)
                .Include(l => l.Salidas)
                .ToList();

            var lotesConStock = lotes
                .Select(l => new LoteDisponibleViewModel
                {
                    id_lote = l.id_lote,
                    id_medicamento = l.id_medicamento,
                    nombre_medicamento = l.Medicamento.nombre,
                    presentacion = l.Medicamento.presentacion,
                    stock_disponible = (l.Entradas?.Sum(e => e.cantidad) ?? 0) - (l.Salidas?.Sum(s => s.cantidad) ?? 0)
                })
                .Where(l => l.stock_disponible > 0)
                .ToList();

            ViewBag.LotesConStock = lotesConStock;
            return View();
        }

        // ============================================
        // POST: Procesar venta y redirigir a factura
        // ============================================
        [HttpPost]
        public IActionResult RegistrarVenta([FromForm] string ventasJson)
        {
            var idUsuario = int.Parse(User.FindFirst("IdUsuario").Value);

            var options = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };

            var ventas = JsonSerializer.Deserialize<List<VentaViewModel>>(ventasJson, options);

            var nuevaVenta = new Venta
            {
                fecha = DateTime.Now,
                id_usuario = idUsuario
            };

            _context.Ventas.Add(nuevaVenta);
            _context.SaveChanges(); // ← necesario para obtener id_venta

            foreach (var item in ventas)
            {
                var lote = _context.Lotes
                    .Include(l => l.Entradas)
                    .Include(l => l.Salidas)
                    .FirstOrDefault(l => l.id_lote == item.id_lote);

                if (lote == null)
                {
                    TempData["Error"] = $"El lote con ID {item.id_lote} no existe.";
                    return RedirectToAction("RegistrarVenta");
                }

                var disponible = lote.Entradas.Sum(e => e.cantidad) - lote.Salidas.Sum(s => s.cantidad);

                if (item.cantidad > disponible)
                {
                    TempData["Error"] = $"Stock insuficiente para {item.nombre_medicamento}. Disponible: {disponible}";
                    return RedirectToAction("RegistrarVenta");
                }

                _context.Salidas.Add(new Salida
                {
                    id_lote = item.id_lote,
                    cantidad = item.cantidad,
                    fecha = DateTime.Now,
                    id_usuario = idUsuario
                });

                _context.DetalleVentas.Add(new DetalleVenta
                {
                    id_venta = nuevaVenta.id_venta,
                    id_lote = item.id_lote,
                    cantidad = item.cantidad,
                    precio_unitario = item.precio_unitario
                });
            }

            _context.SaveChanges();
            TempData["Success"] = "Venta registrada correctamente.";
            return RedirectToAction("GenerarFactura", new { id = nuevaVenta.id_venta });
        }

        // ============================
        // VISTA: Listado de ventas
        // ============================
        public IActionResult ListadoVentas()
        {
            var lista = _context.DetalleVentas
                .Include(d => d.Lote).ThenInclude(l => l.Medicamento)
                .Include(d => d.Venta).ThenInclude(v => v.Usuario)
                .OrderByDescending(d => d.Venta.fecha)
                .Select(d => new
                {
                    Medicamento = d.Lote.Medicamento.nombre,
                    Presentacion = d.Lote.Medicamento.presentacion,
                    Cantidad = d.cantidad,
                    Fecha = d.Venta.fecha,
                    Usuario = d.Venta.Usuario.nombre
                })
                .ToList();

            return View("ListadoVentas", lista);
        }

        // ============================
        // VISTA: Inventario actual
        // ============================
        public IActionResult RevisarInventario()
        {
            var lotesConStock = _context.Lotes
                .Include(l => l.Medicamento)
                .Include(l => l.Entradas)
                .Include(l => l.Salidas)
                .ToList()
                .Select(l => new LoteDisponibleViewModel
                {
                    id_lote = l.id_lote,
                    id_medicamento = l.id_medicamento,
                    nombre_medicamento = l.Medicamento.nombre,
                    presentacion = l.Medicamento.presentacion,
                    stock_disponible = l.Entradas.Sum(e => e.cantidad) - l.Salidas.Sum(s => s.cantidad)
                })
                .ToList();

            return View(lotesConStock);
        }

        // ============================
        // VISTA: Generar factura
        // ============================
        public IActionResult GenerarFactura(int id)
        {
            var venta = _context.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Lote)
                        .ThenInclude(l => l.Medicamento)
                .FirstOrDefault(v => v.id_venta == id);

            if (venta == null)
                return NotFound();

            return View("GenerarFactura", venta);
        }

        // ============================
        // PDF: Descargar factura
        // ============================
        public IActionResult DescargarPDF(int id)
        {
            var venta = _context.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Lote)
                        .ThenInclude(l => l.Medicamento)
                .FirstOrDefault(v => v.id_venta == id);

            if (venta == null)
                return NotFound();

            using var ms = new MemoryStream();
            var doc = new Document(PageSize.A4, 40, 40, 40, 40);
            var writer = PdfWriter.GetInstance(doc, ms);
            doc.Open();

            var titleFont = FontFactory.GetFont("Helvetica", 16, Font.BOLD, new BaseColor(0, 0, 0, 255));
            var normalFont = FontFactory.GetFont("Helvetica", 12, Font.NORMAL, new BaseColor(105, 105, 105, 255));
            var headerFont = FontFactory.GetFont("Helvetica", 12, Font.BOLD, new BaseColor(255, 255, 255, 255));
            var cellBg = new BaseColor(30, 144, 255);

            doc.Add(new Paragraph("Factura de Venta", titleFont));
            doc.Add(new Paragraph($"Fecha: {venta.fecha:dd/MM/yyyy HH:mm}", normalFont));
            doc.Add(new Paragraph($"Usuario: {venta.Usuario.nombre}", normalFont));
            doc.Add(new Paragraph($"ID Venta: {venta.id_venta}", normalFont));
            doc.Add(new Paragraph(" "));

            var table = new PdfPTable(5) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 2, 2, 1, 1, 1 });

            string[] headers = { "Medicamento", "Presentación", "Cantidad", "Precio", "Subtotal" };
            foreach (var h in headers)
            {
                var cell = new PdfPCell(new Phrase(h, headerFont))
                {
                    BackgroundColor = cellBg,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5
                };
                table.AddCell(cell);
            }

            foreach (var d in venta.Detalles)
            {
                table.AddCell(new PdfPCell(new Phrase(d.Lote.Medicamento.nombre, normalFont)));
                table.AddCell(new PdfPCell(new Phrase(d.Lote.Medicamento.presentacion, normalFont)));
                table.AddCell(new PdfPCell(new Phrase(d.cantidad.ToString(), normalFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase($"${d.precio_unitario:F2}", normalFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                table.AddCell(new PdfPCell(new Phrase($"${(d.cantidad * d.precio_unitario):F2}", normalFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
            }

            doc.Add(table);

            var total = venta.Detalles.Sum(d => d.cantidad * d.precio_unitario);
            doc.Add(new Paragraph($"Total: ${total:F2}", titleFont));

            doc.Close();
            writer.Close();

            return File(ms.ToArray(), "application/pdf", $"venta_{id}.pdf");
        }
    }
}
