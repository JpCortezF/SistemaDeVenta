using Microsoft.AspNetCore.Mvc;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class ReporteVentaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
