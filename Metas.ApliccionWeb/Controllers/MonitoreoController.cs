using Metas.AplicacionWeb.Models.ViewModels;
using Metas.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Metas.AplicacionWeb.Controllers
{
    public class MonitoreoController : Controller
    {
        private readonly IDepartamentoService _departamentoService;
        public MonitoreoController(IDepartamentoService departamentoService)
        {
            _departamentoService = departamentoService;
        }
        public async Task<IActionResult> Monitoreo()
        {
            var departamentos = await _departamentoService.ObtenerDepartamentos();

            var modelo = new VMDepartamentos
            {
                ListaDepartamentos = departamentos.Select(d => new SelectListItem
                {
                    Value = d.IdDepartamento.ToString(),
                    Text = d.Departamento1
                }).ToList()
            };
            return View(modelo);
        }

        public async Task<IActionResult> ActualizacionMeses()
        {
            return View();
        }
    }
}
