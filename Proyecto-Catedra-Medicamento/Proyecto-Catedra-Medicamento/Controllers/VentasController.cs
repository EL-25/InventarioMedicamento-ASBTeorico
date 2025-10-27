using iTextSharp.text;
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
        private readonly AppDbContext db;

        public VentasController(AppDbContext context)
        {
            db = context;
        }

        public IActionResult RegistrarVenta()
        {
            var lotesConStock = db.Lotes
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
                .Where(l => l.stock_disponible > 0)
                .ToList();

            ViewBag.LotesConStock = lotesConStock;
            return View();
        }
        [HttpPost]
        public IActionResult RegistrarVenta([FromForm] string ventasJson)
        {
            var idUsuario = int.Parse(User.FindFirst("IdUsuario").Value);

            var options = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };

            var ventas = JsonSerializer.Deserialize<List<VentaViewModel>>(ventasJson, options);

            var venta = new Venta
            {
                fecha = DateTime.Now,
                id_usuario = idUsuario
            };

            db.Ventas.Add(venta);
            db.SaveChanges(); // Necesario para obtener id_venta

            foreach (var item in ventas)
            {
                var lote = db.Lotes
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

                var salida = new Salida
                {
                    id_lote = item.id_lote,
                    cantidad = item.cantidad,
                    fecha = DateTime.Now,
                    id_usuario = idUsuario
                };
                db.Salidas.Add(salida);

                var detalle = new DetalleVenta
                {
                    id_venta = venta.id_venta,
                    id_lote = item.id_lote,
                    cantidad = item.cantidad,
                    precio_unitario = item.precio_unitario
                };
                db.DetalleVentas.Add(detalle);
            }

            db.SaveChanges();
            TempData["Success"] = "Venta registrada correctamente.";
            return RedirectToAction("GenerarFactura", new { id = venta.id_venta });
        }

        public IActionResult RevisarInventario()
        {
            var lotesConStock = db.Lotes
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
        
        public IActionResult GenerarFactura(int id)
        {
            var venta = db.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Lote)
                        .ThenInclude(l => l.Medicamento)
                .FirstOrDefault(v => v.id_venta == id);

            if (venta == null)
            {
                return NotFound();
            }

            return View("GenerarFactura", venta);
        }

        public IActionResult DescargarPDF(int id)
        {
            var venta = db.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Lote)
                        .ThenInclude(l => l.Medicamento)
                .FirstOrDefault(v => v.id_venta == id);

            if (venta == null)
                return NotFound();

            using (var ms = new MemoryStream())
            {
                var doc = new iTextSharp.text.Document();
                var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var titleFont = iTextSharp.text.FontFactory.GetFont("Segoe UI", 16, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 255, 255));
                var normalFont = FontFactory.GetFont("Segoe UI", 12, Font.NORMAL, new BaseColor(255, 255, 255));

                doc.Add(new iTextSharp.text.Paragraph("Factura de Venta", titleFont));
                doc.Add(new iTextSharp.text.Paragraph($"Fecha: {venta.fecha:dd/MM/yyyy HH:mm}", normalFont));
                doc.Add(new iTextSharp.text.Paragraph($"Usuario: {venta.Usuario.nombre}", normalFont));
                doc.Add(new iTextSharp.text.Paragraph($"ID Venta: {venta.id_venta}", normalFont));
                doc.Add(new iTextSharp.text.Paragraph(" "));

                var table = new iTextSharp.text.pdf.PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 2, 2, 1, 1, 1 });

                var headerFont = FontFactory.GetFont("Segoe UI", 12, Font.BOLD, new BaseColor(0, 0, 0));
                string[] headers = { "Medicamento", "Presentación", "Cantidad", "Precio", "Subtotal" };
                foreach (var h in headers)
                    table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(h, headerFont)));

                foreach (var d in venta.Detalles)
                {
                    table.AddCell(d.Lote.Medicamento.nombre);
                    table.AddCell(d.Lote.Medicamento.presentacion);
                    table.AddCell(d.cantidad.ToString());
                    table.AddCell($"${d.precio_unitario:F2}");
                    table.AddCell($"${(d.cantidad * d.precio_unitario):F2}");
                }

                doc.Add(table);

                var total = venta.Detalles.Sum(d => d.cantidad * d.precio_unitario);
                doc.Add(new iTextSharp.text.Paragraph($"Total: ${total:F2}", titleFont));

                doc.Close();
                writer.Close();

                return File(ms.ToArray(), "application/pdf", $"venta_{id}.pdf");
            }
        }


    }
}
