using Microsoft.AspNetCore.Mvc;

namespace Metas.AplicacionWeb.Controllers
{
    public class MonitoreoController : Controller
    {
        public IActionResult Monitoreo()
        {
            return View();
        }
    }
}
