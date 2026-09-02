using Metas.AplicacionWeb.Models.ViewModels;
using Metas.BLL.DTO;
using Metas.BLL.Implementacion;
using Metas.BLL.Interfaces;
using Metas.DAL.DBContext;
using Metas.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace Metas.AplicacionWeb.Controllers
{
    [Authorize]
    public class ProgramacionController : Controller
    {
        private readonly IDepartamentoService _departamentoService;
        private readonly IProgramacionService _programacionService;
        private readonly IFechasService _fechasService;
        private readonly IUsuarioService _usuarioService;
        private readonly MetasContext _context;

        public ProgramacionController(IDepartamentoService departamentoService, IProgramacionService programacionService, IFechasService fechasService, IUsuarioService usuarioService, MetasContext context)
        {
            _departamentoService = departamentoService;
            _programacionService = programacionService;
            _fechasService = fechasService;
            _usuarioService = usuarioService;
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ValidarProgramacion([FromBody] ValidacionRequest request)
        {
            try
            {
                // 1. Eliminar todos los comentarios de esta programación
                var comentarios = await _context.Comentarios
                    .FirstOrDefaultAsync(c => c.IdProgramacion == request.IdProgramacion);

                if (comentarios != null)
                {
                    _context.Comentarios.Remove(comentarios);
                    await _context.SaveChangesAsync();
                }

                // 2. Cambiar estatus a 3 (Validado/Aprobado)
                var resultado = await _programacionService.ActualizarEstatusProgramacion(request.IdProgramacion, 3);

                if (resultado)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Programación validada correctamente. Comentarios eliminados."
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Error al validar la programación"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Error: {ex.Message}"
                });
            }
        }

        public class ValidacionRequest
        {
            public int IdProgramacion { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> Programacion()
        {
            var departamentos = await _departamentoService.ObtenerDepartamentos();
            bool esAdmin = User.IsInRole("Administrador");
            var areaClaim = User.Claims.FirstOrDefault(c => c.Type == "Departamento")?.Value;

            IEnumerable<SelectListItem> listaDepartamentos;

            if (esAdmin)
            {
                listaDepartamentos = departamentos
                    .Select(d => new SelectListItem
                    {
                        Value = d.IdDepartamento.ToString(),
                        Text = d.Departamento1,
                        Group = new SelectListGroup { Name = d.Area }
                    })
                    .OrderBy(item => item.Text)
                    .ToList();
            }
            else
            {
                if (User.IsInRole("Departamento"))
                {
                    // Es un usuario de departamento específico
                    listaDepartamentos = departamentos
                        .Where(d => d.Departamento1 == areaClaim)
                        .Select(d => new SelectListItem
                        {
                            Value = d.IdDepartamento.ToString(),
                            Text = d.Departamento1,
                            Group = new SelectListGroup { Name = d.Area }
                        }).ToList();
                }
                else
                {
                    // Es un usuario de dirección/unidad — filtrar por área
                    listaDepartamentos = departamentos
                        .Where(d => d.Area == areaClaim)
                        .Select(d => new SelectListItem
                        {
                            Value = d.IdDepartamento.ToString(),
                            Text = d.Departamento1,
                            Group = new SelectListGroup { Name = d.Area }
                        })
                        .OrderBy(item => item.Text)
                        .ToList();
                }
            }

            var listaAreas = departamentos
                .Where(d => !string.IsNullOrWhiteSpace(d.Area))
                .Select(d => d.Area)
                .Distinct()
                .OrderBy(a => a)
                .Select(a => new SelectListItem { Value = a, Text = a })
                .ToList();

            var modelo = new VMDepartamentos
            {
                ListaDepartamentos = listaDepartamentos.ToList(),
                ListaAreas = listaAreas
            };

            return View(modelo);
        }

        [HttpGet]
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
        public async Task<IActionResult> ObtenerDatos(int anoFiscal, int? departamento, string area = null)
        {
            try
            {
                bool esAdmin = User.IsInRole("Administrador");
                var datos = await _programacionService.ObtenerDatosProgramacion(anoFiscal, departamento, area);

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
                    IdEstatus = x.Programacions.FirstOrDefault()?.IdEstatus,
                    NombreEstatus = x.Programacions.FirstOrDefault()?.IdEstatusNavigation.Valor
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

        public async Task<IActionResult> RevisarProgramacion(int id)
        {
            var componentes = await _departamentoService.ObtenerComponentes();
            var medidas = await _departamentoService.ObtenerMedidas();
            var municipios = await _departamentoService.ObtenerMunicipios();

            var datosCompletos = await _programacionService.ObtenerDatosCompletos(id);

            if (datosCompletos == null)
            {
                TempData["Error"] = "No se encontraron datos para la programación solicitada.";
                return RedirectToAction("Programacion");
            }

            // ✅ Asignar lista de programas
            datosCompletos.ListaProgramas = componentes
                .GroupBy(x => x.Pp)
                .Select(g => new SelectListItem
                {
                    Value = g.Key.ToString(),
                    Text = g.First().PpCompuesto1
                })
                .OrderBy(item => item.Text)
                .ToList();

            // ✅ CAMBIO: Cargar TODOS los componentes (como en RegistrarProgramacion)
            datosCompletos.ListaComponentes = componentes
                .Select(c => new SelectListItem
                {
                    Value = c.Componente.ToString(),
                    Text = c.ComponenteCompuesto,
                    Group = new SelectListGroup { Name = c.PpCompuesto1 } // 🎯 Texto del programa
                })
                .ToList();

            datosCompletos.ListaMedidas = medidas
                .Select(m => new SelectListItem
                {
                    Value = m.IdUnidad.ToString(),
                    Text = m.Valor
                })
                .ToList();

            datosCompletos.ListaMunicipios = municipios
                .Select(m => new SelectListItem
                {
                    Value = m.IdMunicipio.ToString(),
                    Text = m.NombreMunicipios
                })
                .ToList();

            var idProgramacion = datosCompletos.Id;
            var comentariosExistentes = await _programacionService.ObtenerComentariosPorProgramacion(idProgramacion);

            ViewBag.ComentariosExistentes = comentariosExistentes;
            ViewBag.IdProgramacion = idProgramacion;
            ViewData["IdProgramacion"] = idProgramacion;
            ViewBag.EsAdministrador = User.IsInRole("Administrador");
            ViewBag.EstatusActual = datosCompletos.Estatus;

            return View(datosCompletos);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarComentarios([FromBody] List<ComentarioDTO> comentarios)
        {
            try
            {
                var resultado = await _programacionService.GuardarComentarios(comentarios);

                // CAMBIO: Retornar objeto JSON en lugar de bool directo
                return Ok(new
                {
                    success = resultado,
                    message = resultado ? "Comentarios guardados correctamente" : "Error al guardar comentarios"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerComentariosPorProgramacion(int idProgramacion)
        {
            try
            {
                var comentarios = await _programacionService.ObtenerComentariosPorProgramacion(idProgramacion);

                if (comentarios == null)
                {
                    comentarios = new Comentario { IdProgramacion = idProgramacion };
                }

                // Forzar serialización con camelCase
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                return Json(comentarios, options);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ActualizarProgramacion([FromBody] ProgramacionDTO modelo)
        {
            try
            {
                // Validaciones
                if (modelo == null || modelo.Id <= 0)
                {
                    return BadRequest(new { success = false, message = "Datos inválidos" });
                }

                if (string.IsNullOrWhiteSpace(modelo.Pp))
                    return BadRequest(new { success = false, message = "El programa presupuestario es requerido" });

                if (string.IsNullOrWhiteSpace(modelo.NComponente))
                    return BadRequest(new { success = false, message = "El número de componente es requerido" });

                // Llamar al servicio
                bool resultado = await _programacionService.ActualizarProgramacion(modelo);

                if (resultado)
                {
                    return Ok(new { success = true, message = "Programación actualizada correctamente" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Error al actualizar la programación" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActualizarProgramacion: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GuardarCambiosUsuario([FromBody] ProgramacionDTO modelo)
        {
            try
            {
                if (modelo == null || modelo.Id <= 0)
                {
                    return BadRequest(new { success = false, message = "Datos inválidos" });
                }

                // Actualizar la programación
                bool resultado = await _programacionService.ActualizarProgramacion(modelo);

                if (resultado)
                {
                    // ✅ NUEVO: Actualizar comentarios (eliminar los que el usuario borró al editar)
                    if (modelo.Comentarios != null && modelo.Comentarios.Any())
                    {
                        await _programacionService.GuardarComentarios(modelo.Comentarios);
                    }

                    // Cambiar estatus a 1 (En Revisión) después de que el usuario guarda
                    await _programacionService.ActualizarEstatusProgramacion(modelo.Id, 1);

                    return Ok(new { success = true, message = "Cambios guardados correctamente" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Error al guardar los cambios" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GuardarCambiosAdmin([FromBody] ProgramacionDTO modelo)
        {
            try
            {
                if (modelo == null || modelo.Id <= 0)
                {
                    return BadRequest(new { success = false, message = "Datos inválidos" });
                }

                // 1. Actualizar la programación
                bool resultado = await _programacionService.ActualizarProgramacion(modelo);

                if (!resultado)
                {
                    return BadRequest(new { success = false, message = "Error al actualizar la programación" });
                }

                // 2. Guardar comentarios
                if (modelo.Comentarios != null && modelo.Comentarios.Any())
                {
                    await _programacionService.GuardarComentarios(modelo.Comentarios);
                }

                return Ok(new { success = true, message = "Cambios guardados correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public IActionResult TableroCompletoProgramacion(int? anoFiscal)
        {
            ViewBag.AnoFiscalSeleccionado = anoFiscal;
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ObtenerDatosTableroProgramacion(int anoFiscal)
        {
            try
            {
                var departamentos = await _departamentoService.ObtenerDepartamentos();
                var todosLosDatos = new List<dynamic>();

                foreach (var depto in departamentos)
                {
                    var datosProgramacion = await _programacionService
                        .ObtenerDatosProgramacion(anoFiscal, depto.IdDepartamento);

                    if (datosProgramacion == null || !datosProgramacion.Any())
                        continue;

                    foreach (var proceso in datosProgramacion)
                    {
                        var datosCompletos = await _programacionService
                            .ObtenerDatosCompletos(proceso.IdProceso);

                        if (datosCompletos == null)
                            continue;

                        var registro = new
                        {
                            idProceso = proceso.IdProceso,

                            area = datosCompletos.Area ?? "",
                            departamento = datosCompletos.Departamento ?? "",
                            correoContacto = datosCompletos.CorreoContacto ?? "",

                            pp = datosCompletos.Pp ?? "",
                            componente = datosCompletos.NComponente ?? "",
                            actividad = datosCompletos.NActividad,
                            justificacion = datosCompletos.Justificacion ?? "",
                            descripcionDocumento = datosCompletos.DescripcionDocumento ?? "",
                            recursoFederal = datosCompletos.RecursoFederal ?? "",
                            recursoEstatal = datosCompletos.RecursoEstatal ?? "",

                            programaSocial = datosCompletos.ProgramaSocial ?? "",
                            descripcionActividad = datosCompletos.DescripcionActividad ?? "",
                            nombreIndicador = datosCompletos.NombreIndicador ?? "",
                            definicionIndicador = datosCompletos.DefinicionIndicador ?? "",

                            primerServicio = datosCompletos.PrimerServicio,
                            segundoServicio = datosCompletos.SegundoServicio,
                            tercerServicio = datosCompletos.TercerServicio,
                            cuartoServicio = datosCompletos.CuartoServicio,

                            primerPersona = datosCompletos.PrimerPersona,
                            segundoPersona = datosCompletos.SegundoPersona,
                            tercerPersona = datosCompletos.TercerPersona,
                            cuartoPersona = datosCompletos.CuartoPersona,

                            unidadMedida = datosCompletos.UnidadMedida ?? "",
                            mediosVerificacion = datosCompletos.MediosVerificacion ?? "",
                            serieInformacionDesde = datosCompletos.SerieInformacionDesde ?? "",
                            serieInformacionHasta = datosCompletos.SerieInformacionHasta ?? "",
                            fuenteInformacion = datosCompletos.FuenteInformacion ?? "",
                            intervienenDelegaciones = datosCompletos.IntervienenDelegaciones ?? "",
                            intervienenDelegacionesManera = datosCompletos.IntervienenDelegacionesManera ?? "",

                            anoBase = datosCompletos.AnoBase,
                            porcentajeBase = datosCompletos.PorcentajeBase,
                            servicioBase = datosCompletos.ServicioBase,
                            personasBase = datosCompletos.PersonasBase,

                            anoMeta = datosCompletos.AnoMeta,
                            porcentajeMeta = datosCompletos.PorcentajeMeta,
                            servicioMeta = datosCompletos.ServicioMeta,
                            personasMeta = datosCompletos.PersonasMeta,

                            beneficiarios = datosCompletos.Beneficiarios ?? "",
                            acumulable = datosCompletos.SelectAcumulable ?? "",

                            mesesServicios = datosCompletos.MesesServicios,
                            mesesPersonas = datosCompletos.MesesPersonas,

                            totalServicios = datosCompletos.TotalAnos,
                            totalPersonas = datosCompletos.TotalAnos2,

                            estatus = datosCompletos.Estatus
                        };

                        todosLosDatos.Add(registro);
                    }
                }

                return Json(new { success = true, datos = todosLosDatos.OrderBy(x => (string)x.pp)
        .ThenBy(x => (string)x.componente)
        .ThenBy(x => (int)x.actividad)
        .ToList()
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = $"Error: {ex.Message}" });
            }
        }

    }
}
