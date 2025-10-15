using Metas.AplicacionWeb.Models.ViewModels;
using Metas.BLL.Implementacion;
using Metas.BLL.Interfaces;
using Metas.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Metas.AplicacionWeb.Controllers
{
    //[Authorize]
    public class ProgramacionController : Controller
    {
        private readonly IDepartamentoService _departamentoService;
        private readonly IProgramacionService _programacionService;
        private readonly IFechasService _fechasService;
        private readonly IUsuarioService _usuarioService;
        public ProgramacionController(IDepartamentoService departamentoService, IProgramacionService programacionService, IFechasService fechasService, IUsuarioService usuarioService)
        {
            _departamentoService = departamentoService;
            _programacionService = programacionService;
            _fechasService = fechasService;
            _usuarioService = usuarioService;
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

        public async Task<IActionResult> RegistrarProgramacion(int anoFiscal, int departamentoId)
        {
            var componentes = await _departamentoService.ObtenerComponentes();
            var departamentos = await _departamentoService.ObtenerDepartamentos();
            var medidas = await _departamentoService.ObtenerMedidas();
            var municipios = await _departamentoService.ObtenerMunicipios();
            var departamento = departamentos
                .FirstOrDefault(d => d.IdDepartamento == departamentoId);

            // 1. Construir el ViewModel
            var modelo = new VMProgramacion
            {
                AnoFiscal = anoFiscal,
                DepartamentoId = departamentoId,
                AreaNombre = departamento?.Area ?? "Área Desconocida",
                DepartamentoNombre = departamento?.Departamento1 ?? "Departamento Desconocido",

                ListaProgramas = componentes
                    .GroupBy(x => x.PpCompuesto1)
                    .Select(g => new SelectListItem
                    {
                        Value = g.First().PpCompuesto1,
                        Text = g.First().PpCompuesto1
                    })
                    .ToList(),

                ListaComponentes = componentes
                    .Select(c => new SelectListItem
                    {
                        Value = c.IdPp.ToString(),
                        Text = c.ComponenteCompuesto,
                        Group = new SelectListGroup { Name = c.PpCompuesto1 }
                    })
                    .ToList(),

                ListaMedidas = medidas
                    .Select(g => new SelectListItem
                    {
                        Value = g.IdUnidad.ToString(),
                        Text = g.Valor
                    })
                    .ToList(),

                ListaMunicipios = municipios
                    .Select(g => new SelectListItem
                    {
                        Value = g.IdMunicipio.ToString(),
                        Text = g.NombreMunicipios
                    })
                    .ToList()
            };
            var correo = await _usuarioService.ObtenerCorreos(modelo.DepartamentoNombre);
            modelo.CorreoContacto = correo?.CorreoElectronico ?? "";

            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDatos(int anoFiscal, int departamento)
        {
            try
            {
                var datos = await _programacionService.ObtenerDatosProgramacion(anoFiscal, departamento);

                // Proyectar los datos a un DTO o modelo anónimo
                var resultado = datos.Select(x => new VMDatosInternos
                {
                    IdProceso = x.IdProceso,
                    pp = x.Pp,
                    Componente = x.Componente,
                    Actividad = x.Actividad,
                    DescripcionActividad = x.DescripcionActividad,
                    Area = x.Area,
                    Departamento = x.Departamento,
                    ProgramaSocial = x.ProgramaSocial,
                    IdEstatus = x.Programacions.FirstOrDefault()?.IdEstatus
                }).ToList();

                var fecha = await _fechasService.ValidarFechaHabilitada(anoFiscal);

                return Json(new { success = true, datos = resultado, fechaHabilitada = fecha });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = $"Error al obtener datos: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerMunicipiosJson()
        {
            try
            {
                var municipios = await _departamentoService.ObtenerMunicipios();

                var municipiosJson = municipios.Select(m => new
                {
                    id = m.IdMunicipio.ToString(),
                    municipio = m.NombreMunicipios,
                    region = m.NombreRegion,
                    numeroRegion = m.NumeroRegion,
                    clave = m.ClaveMuni
                }).ToList();

                return Json(municipiosJson);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }

    
}
