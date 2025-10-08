using Metas.AplicacionWeb.Models.ViewModels;
using Metas.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Metas.AplicacionWeb.Controllers
{
    //[Authorize]
    public class ProgramacionController : Controller
    {
        private readonly IDepartamentoService _departamentoService;
        public ProgramacionController(IDepartamentoService departamentoService)
        {
            _departamentoService = departamentoService;
        }
        public async Task<IActionResult> Programacion()
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

        public async Task<IActionResult> RegistrarProgramacion()
        {
            
            return View();
        }
    }

    
}
