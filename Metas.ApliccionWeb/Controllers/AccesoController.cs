using Microsoft.AspNetCore.Mvc;

namespace Metas.AplicacionWeb.Controllers
{
    public class AccesoController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(int idUsuario)
        {
            return View();
        }
    }
}
