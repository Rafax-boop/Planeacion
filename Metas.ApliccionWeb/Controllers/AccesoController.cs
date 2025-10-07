using Metas.AplicacionWeb.Models.ViewModels;
using Metas.BLL.Interfaces;
using Metas.Entity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Metas.AplicacionWeb.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public AccesoController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            ClaimsPrincipal User = HttpContext.User;
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(VMUsuarioLogin modelo)
        {
            Usuario usuarioEncontrado = await _usuarioService.ObtenerPorCredenciales(modelo.Usuario.Trim(), modelo.Password);

            if (usuarioEncontrado == null)
            {
                ViewData["Mensaje"] = "Usuario o contraseña incorrectos";
                return View(modelo);
            }

            ViewData["Mensaje"] = null;

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuarioEncontrado.Usuario1),
                new Claim(ClaimTypes.NameIdentifier, usuarioEncontrado.IdUsuario.ToString()),
                new Claim("Departamento", usuarioEncontrado.Area),
                new Claim(ClaimTypes.Role, usuarioEncontrado.Definicion)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

            //TempData["LoginExitoso"] = "true";
            //TempData["NombreUsuario"] = usuarioEncontrado.Usuario1;

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Acceso");
        }
    }
}