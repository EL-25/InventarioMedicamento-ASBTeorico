using Microsoft.AspNetCore.Mvc;

namespace Proyecto_Catedra_Medicamento.Controllers
{
    public class OperadorController : Controller
    {
        public IActionResult Index()
        {
            return View("OperadorIndex"); //Carga Views/Operador/Index.cshtml
        }

        public IActionResult AccesoRestringido()
        {
            return View("AccesoRestringido"); // Carga Views/Operador/AccesoRestringido.cshtml
        }
    }
}
