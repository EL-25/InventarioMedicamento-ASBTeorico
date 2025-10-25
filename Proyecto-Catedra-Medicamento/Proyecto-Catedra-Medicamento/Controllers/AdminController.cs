using Microsoft.AspNetCore.Mvc;

namespace Proyecto_Catedra_Medicamento.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}