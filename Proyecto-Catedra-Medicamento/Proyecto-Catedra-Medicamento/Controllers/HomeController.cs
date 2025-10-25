using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Catedra_Medicamento.Models;

namespace Proyecto_Catedra_Medicamento.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Si el usuario ya inici� sesi�n, redirigir seg�n rol
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Administrador"))
                    return RedirectToAction("Index", "Admin");

                // Si tienes otro controlador para operadores
                if (User.IsInRole("Operador"))
                    return RedirectToAction("Index", "Operador");
            }

            // Si no hay sesi�n activa, mostrar la vista institucional p�blica
            return View(); // Muestra Views/Home/Index.cshtml
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
