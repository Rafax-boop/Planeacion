using Metas.AplicacionWeb.Models.ViewModels;
using Metas.BLL.DTO;
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
                    .GroupBy(x => x.Pp)
                    .Select(g => new SelectListItem
                    {
                        Value = g.Key.ToString(),
                        Text = g.First().PpCompuesto1
                    })
                    .OrderBy(item => item.Text)
                    .ToList(),

                ListaComponentes = componentes
                    .Select(c => new SelectListItem
                    {
                        Value = c.Componente.ToString(),
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
                bool esAdmin = User.IsInRole("Administrador");
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

                return Json(new { success = true, datos = resultado, fechaHabilitada = fecha, esAdmin = esAdmin });
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

        [HttpPost]
        public async Task<IActionResult> GuardarProgramacion([FromBody] ProgramacionDTO modelo)
        {
            try
            {
                // Validar que el modelo no sea nulo
                if (modelo == null)
                {
                    return BadRequest(new { success = false, message = "Los datos del formulario no fueron recibidos correctamente." });
                }

                // Validar campos requeridos
                if (string.IsNullOrWhiteSpace(modelo.Pp))
                    return BadRequest(new { success = false, message = "El programa presupuestario es requerido." });

                if (string.IsNullOrWhiteSpace(modelo.NComponente))
                    return BadRequest(new { success = false, message = "El número de componente es requerido." });

                if (modelo.NActividad <= 0)
                    return BadRequest(new { success = false, message = "El número de actividad debe ser mayor a 0." });

                if (string.IsNullOrWhiteSpace(modelo.DescripcionActividad))
                    return BadRequest(new { success = false, message = "La descripción de la actividad es requerida." });

                if (modelo.MesesServicios == null || modelo.MesesServicios.Count != 12)
                    return BadRequest(new { success = false, message = "Debe proporcionar los 12 meses de servicios." });

                if (modelo.MesesPersonas == null || modelo.MesesPersonas.Count != 12)
                    return BadRequest(new { success = false, message = "Debe proporcionar los 12 meses de personas." });

                // Validar que haya acciones (mínimo 3, máximo 6)
                if (modelo.Acciones == null || modelo.Acciones.Count < 3 || modelo.Acciones.Count > 6)
                    return BadRequest(new { success = false, message = "Debe haber entre 3 y 6 acciones." });

                // Llamar al servicio para guardar
                bool resultado = await _programacionService.GuardarProgramacion(modelo);

                if (resultado)
                {
                    return Ok(new { success = true, message = "Programación guardada correctamente." });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Error al guardar la programación. Por favor intente nuevamente." });
                }
            }
            catch (Exception ex)
            {
                // Log del error (aquí podrías usar un logger)
                Console.WriteLine($"Error en GuardarProgramacion: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Error interno del servidor.", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EliminarRegistro(int id)
        {
            try
            {
                // Llamar a la función del servicio
                bool eliminado = await _programacionService.EliminarProgramacion(id);

                if (eliminado)
                {
                    return Json(new { success = true, mensaje = "Registro eliminado correctamente." });
                }
                else
                {
                    return Json(new { success = false, mensaje = "No se pudo encontrar o eliminar el registro." });
                }
            }
            catch (Exception ex)
            {
                // Registrar la excepción (recomendado)
                return Json(new { success = false, mensaje = $"Error al eliminar el registro: {ex.Message}" });
            }
        }
    }    
}
