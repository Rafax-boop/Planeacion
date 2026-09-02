using AutoMapper;
using ClosedXML.Excel;
using Metas.AplicacionWeb.Models.ViewModels;
using Metas.BLL.DTO;
using Metas.BLL.Implementacion;
using Metas.BLL.Interfaces;
using Metas.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;

namespace Metas.AplicacionWeb.Controllers
{
    [Authorize]
    public class MonitoreoController : Controller
    {
        private readonly IDepartamentoService _departamentoService;
        private readonly IProgramacionService _programacionService;
        private readonly IFechasService _fechasService;
        private readonly IMonitoreoService _monitoreoService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;
        public MonitoreoController(IDepartamentoService departamentoService,
            IProgramacionService programacionService,
            IFechasService fechasService,
            IMonitoreoService monitoreoService,
            IWebHostEnvironment hostEnvironment,
            IMapper mapper)
        {
            _departamentoService = departamentoService;
            _programacionService = programacionService;
            _fechasService = fechasService;
            _monitoreoService = monitoreoService;
            _hostEnvironment = hostEnvironment;
            _mapper = mapper;
        }
        public async Task<IActionResult> Monitoreo()
        {
            var departamentos = await _departamentoService.ObtenerDepartamentos();
            var areaClaim = User.Claims.FirstOrDefault(c => c.Type == "Departamento")?.Value;
            bool esAdmin = User.IsInRole("Administrador");

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
                    .OrderBy(item => item.Text);
            }
            else
            {
                if (User.IsInRole("Departamento"))
                {
                    // Usuario de departamento específico
                    listaDepartamentos = departamentos
                        .Where(d => d.Departamento1 == areaClaim)
                        .Select(d => new SelectListItem
                        {
                            Value = d.IdDepartamento.ToString(),
                            Text = d.Departamento1,
                            Group = new SelectListGroup { Name = d.Area }
                        });
                }
                else
                {
                    // Usuario de dirección/unidad → trae todos sus departamentos
                    listaDepartamentos = departamentos
                        .Where(d => d.Area == areaClaim)
                        .Select(d => new SelectListItem
                        {
                            Value = d.IdDepartamento.ToString(),
                            Text = d.Departamento1,
                            Group = new SelectListGroup { Name = d.Area }
                        })
                        .OrderBy(item => item.Text);
                }
            }

            IEnumerable<string> areasPermitidas;

            if (esAdmin)
            {
                areasPermitidas = departamentos.Select(d => d.Area);
            }
            else if (User.IsInRole("Departamento"))
            {
                areasPermitidas = departamentos
                    .Where(d => d.Departamento1 == areaClaim)
                    .Select(d => d.Area);
            }
            else
            {
                areasPermitidas = new[] { areaClaim };
            }

            var listaAreas = areasPermitidas
                .Where(a => !string.IsNullOrWhiteSpace(a))
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

        public async Task<IActionResult> ObtenerDatos(int anoFiscal, int? departamento, string area = null)
        {
            try
            {
                bool esAdmin = User.IsInRole("Administrador");

                // Obtener datos de programación
                var datos = await _programacionService.ObtenerDatosProgramacion(anoFiscal, departamento, area);

                // Obtener fechas de captura
                var fechasCaptura = await _fechasService.Lista();

                // Crear diccionario con FechaInicio y FechaFin
                var fechasDelAno = fechasCaptura
                    .ToDictionary(
                        f => f.IdFechaCaptura,
                        f => new RangoDeFechas { FechaInicio = f.FechaInicio, FechaFin = f.FechaFin }
                    );

                // Proyectar los datos
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
                    Enero = x.Enero,
                    Febrero = x.Febrero,
                    Marzo = x.Marzo,
                    Abril = x.Abril,
                    Mayo = x.Mayo,
                    Junio = x.Junio,
                    Julio = x.Julio,
                    Agosto = x.Agosto,
                    Septiembre = x.Septiembre,
                    Octubre = x.Octubre,
                    Noviembre = x.Noviembre,
                    Diciembre = x.Diciembre,
                    IdEstatus = x.Programacions.FirstOrDefault()?.IdEstatus,
                    NombreEstatus = x.Programacions.FirstOrDefault()?.IdEstatusNavigation.Valor,
                    FechaEnero = x.FechaEnero.HasValue ? x.FechaEnero.Value.ToString("yyyy-MM-dd") : null,
                    FechaFebrero = x.FechaFebrero.HasValue ? x.FechaFebrero.Value.ToString("yyyy-MM-dd") : null,
                    FechaMarzo = x.FechaMarzo.HasValue ? x.FechaMarzo.Value.ToString("yyyy-MM-dd") : null,
                    FechaAbril = x.FechaAbril.HasValue ? x.FechaAbril.Value.ToString("yyyy-MM-dd") : null,
                    FechaMayo = x.FechaMayo.HasValue ? x.FechaMayo.Value.ToString("yyyy-MM-dd") : null,
                    FechaJunio = x.FechaJunio.HasValue ? x.FechaJunio.Value.ToString("yyyy-MM-dd") : null,
                    FechaJulio = x.FechaJulio.HasValue ? x.FechaJulio.Value.ToString("yyyy-MM-dd") : null,
                    FechaAgosto = x.FechaAgosto.HasValue ? x.FechaAgosto.Value.ToString("yyyy-MM-dd") : null,
                    FechaSeptiembre = x.FechaSeptiembre.HasValue ? x.FechaSeptiembre.Value.ToString("yyyy-MM-dd") : null,
                    FechaOctubre = x.FechaOctubre.HasValue ? x.FechaOctubre.Value.ToString("yyyy-MM-dd") : null,
                    FechaNoviembre = x.FechaNoviembre.HasValue ? x.FechaNoviembre.Value.ToString("yyyy-MM-dd") : null,
                    FechaDiciembre = x.FechaDiciembre.HasValue ? x.FechaDiciembre.Value.ToString("yyyy-MM-dd") : null,
                    FechasCaptura = fechasDelAno
                }).ToList();

                return Json(new { success = true, datos = resultado, esAdmin = esAdmin });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = $"Error al obtener datos: {ex.Message}" });
            }
        }

        public async Task<IActionResult> ActualizacionMeses(int idProceso, int mes, string modo = null)
        {
            var fechasCaptura = await _fechasService.Lista();

            var fechaRegistro = fechasCaptura
                .FirstOrDefault(f => f.IdFechaCaptura == mes);

            var datosInternos = await _programacionService.ObtenerporId(idProceso);
            var datosExternos = await _monitoreoService.ObtenerLlenadoMensual(idProceso, mes);

            int? totalMes = mes switch
            {
                1 => datosInternos.TotalEnero,
                2 => datosInternos.TotalFebrero,
                3 => datosInternos.TotalMarzo,
                4 => datosInternos.TotalAbril,
                5 => datosInternos.TotalMayo,
                6 => datosInternos.TotalJunio,
                7 => datosInternos.TotalJulio,
                8 => datosInternos.TotalAgosto,
                9 => datosInternos.TotalSeptiembre,
                10 => datosInternos.TotalOctubre,
                11 => datosInternos.TotalNoviembre,
                12 => datosInternos.TotalDiciembre,
                _ => null
            };

            int? totalPersonas = mes switch
            {
                1 => datosInternos.EneroPersona,
                2 => datosInternos.FebreroPersona,
                3 => datosInternos.MarzoPersona,
                4 => datosInternos.AbrilPersona,
                5 => datosInternos.MayoPersona,
                6 => datosInternos.JunioPersona,
                7 => datosInternos.JulioPersona,
                8 => datosInternos.AgostoPersona,
                9 => datosInternos.SeptiembrePersona,
                10 => datosInternos.OctubrePersona,
                11 => datosInternos.NoviembrePersona,
                12 => datosInternos.DiciembrePersona,
                _ => null
            };

            var modelo = new VMGuardarActualizacion
            {
                IdProceso = idProceso,
                Mes = fechaRegistro.Mes,
                MesNum = mes,
                FechaFin = fechaRegistro.FechaFin,
                Total = totalMes,
                TotalPersonas = totalPersonas,
                pp = datosInternos.Pp,
                Componente = datosInternos.Componente,
                Actividad = datosInternos.Actividad,
                UnidadMedida = datosInternos.UnidadMedida,
                ProgramaSocial = datosInternos.ProgramaSocial,
                Area = datosInternos.Area,
                Departamento = datosInternos.Departamento,
                DescripcionActividad = datosInternos.DescripcionActividad,
                Realizado = datosExternos?.Realizado ?? 0,
                MujeresAtendidas = datosExternos?.MujeresAtendidas ?? 0,
                HombresAtendidos = datosExternos?.HombresAtendidos ?? 0,
                Rango0a3 = datosExternos?._03anos ?? 0,
                Rango4a8 = datosExternos?._48anos ?? 0,
                Rango9a12 = datosExternos?._912anos ?? 0,
                Rango13a17 = datosExternos?._1317anos ?? 0,
                Rango18a29 = datosExternos?._1829anos ?? 0,
                Rango30a59 = datosExternos?._3059anos ?? 0,
                Rango60adelante = datosExternos?._60amasanos ?? 0,
                RangoNoEspecifica = datosExternos?.NoDefinida ?? 0,
                Indigena = datosExternos?.Indigena ?? 0,
                RutaEvidencia = datosExternos?.Evidencia,
                RutaJustificacion = datosExternos?.Justificacion,
                NombreRealizo = datosInternos.NombreRealizo ?? "",
                PuestoRealizo = datosInternos.CargoRealizo ?? "",
                NombreAutorizo = datosInternos.NombreValido ?? "",
                PuestoAutorizo = datosInternos.CargoValido ?? ""
            };
            ViewBag.EsModoVisualizar = (modo == "visualizar");
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarActualizacion(GuardarActualizacionDTO modelo)
        {
            // Variables de ruta - inicializamos con las rutas existentes del modelo
            string rutaEvidencia = modelo.RutaEvidenciaExistente; // Nueva propiedad que necesitarás agregar
            string rutaJustificacion = modelo.RutaJustificacionExistente; // Nueva propiedad que necesitarás agregar

            // Variables de ruta para construcción
            string idProceso = modelo.IdProceso.ToString();
            string mes = modelo.Mes.ToString();

            // Definición de las carpetas base en wwwroot
            const string BASE_FOLDER_EVIDENCIA = "Evidencia";
            const string BASE_FOLDER_JUSTIFICACION = "Justificacion";

            // 1. Definir rutas físicas completas para las carpetas destino
            string folderPathEvidencia = Path.Combine(_hostEnvironment.WebRootPath, BASE_FOLDER_EVIDENCIA, idProceso, mes);
            string folderPathJustificacion = Path.Combine(_hostEnvironment.WebRootPath, BASE_FOLDER_JUSTIFICACION, idProceso, mes);

            try
            {
                // 2. PROCESAMIENTO DE ARCHIVOS Y CREACIÓN DE CARPETAS

                // A. Evidencia - SOLO si se subió un archivo nuevo
                if (modelo.InputEvidencia != null && modelo.InputEvidencia.Length > 0)
                {
                    // Crear la estructura de carpetas para Evidencia si no existe
                    Directory.CreateDirectory(folderPathEvidencia);

                    // OPCIONAL: Eliminar archivo anterior si existía
                    if (!string.IsNullOrEmpty(rutaEvidencia))
                    {
                        string rutaFisicaAnterior = Path.Combine(_hostEnvironment.WebRootPath, rutaEvidencia.TrimStart('/'));
                        if (System.IO.File.Exists(rutaFisicaAnterior))
                        {
                            System.IO.File.Delete(rutaFisicaAnterior);
                        }
                    }

                    // Usar el nombre original del archivo
                    string originalFileName = Path.GetFileName(modelo.InputEvidencia.FileName);

                    // Ruta física donde se guardará el archivo
                    string rutaFisicaEvidencia = Path.Combine(folderPathEvidencia, originalFileName);

                    // Guardar el archivo físicamente
                    using (var fileStream = new FileStream(rutaFisicaEvidencia, FileMode.Create))
                    {
                        await modelo.InputEvidencia.CopyToAsync(fileStream);
                    }

                    // Actualizamos la ruta para guardar en BD
                    rutaEvidencia = $"/{BASE_FOLDER_EVIDENCIA}/{idProceso}/{mes}/{originalFileName}";
                }
                // Si NO se subió archivo nuevo, rutaEvidencia conserva el valor existente

                // B. Justificación - SOLO si se subió un archivo nuevo
                if (modelo.InputJustificacion != null && modelo.InputJustificacion.Length > 0)
                {
                    // Crear la estructura de carpetas para Justificación si no existe
                    Directory.CreateDirectory(folderPathJustificacion);

                    // OPCIONAL: Eliminar archivo anterior si existía
                    if (!string.IsNullOrEmpty(rutaJustificacion))
                    {
                        string rutaFisicaAnterior = Path.Combine(_hostEnvironment.WebRootPath, rutaJustificacion.TrimStart('/'));
                        if (System.IO.File.Exists(rutaFisicaAnterior))
                        {
                            System.IO.File.Delete(rutaFisicaAnterior);
                        }
                    }

                    // Usar el nombre original del archivo
                    string originalFileName = Path.GetFileName(modelo.InputJustificacion.FileName);

                    // Ruta física donde se guardará el archivo
                    string rutaFisicaJustificacion = Path.Combine(folderPathJustificacion, originalFileName);

                    // Guardar el archivo físicamente
                    using (var fileStream = new FileStream(rutaFisicaJustificacion, FileMode.Create))
                    {
                        await modelo.InputJustificacion.CopyToAsync(fileStream);
                    }

                    // Actualizamos la ruta para guardar en BD
                    rutaJustificacion = $"/{BASE_FOLDER_JUSTIFICACION}/{idProceso}/{mes}/{originalFileName}";
                }
                // Si NO se subió archivo nuevo, rutaJustificacion conserva el valor existente

                // 3. LLAMADA AL SERVICIO Y PERSISTENCIA DE DATOS
                bool resultado = await _monitoreoService.GuardarActualizacion(
                    modelo,
                    rutaEvidencia,
                    rutaJustificacion
                );

                // 4. MANEJO DE RESULTADO Y RESPUESTA JSON
                if (resultado)
                {
                    string mensaje = modelo.EsBorrador ? "El borrador se guardó exitosamente." : "La actualización se envió con éxito.";

                    return Json(new
                    {
                        success = true,
                        message = mensaje,
                        redirectTo = Url.Action("Monitoreo", "Monitoreo")
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Hubo un error al intentar guardar la actualización. Intente de nuevo."
                    });
                }
            }
            catch (Exception ex)
            {
                // Se recomienda loggear el error (ex)
                return Json(new
                {
                    success = false,
                    message = $"Ocurrió un error inesperado en el servidor: {ex.Message}"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> HabilitarCaptura(int idProceso, int mes)
        {
            // Verificar si el usuario es administrador por seguridad
            if (!User.IsInRole("Administrador"))
            {
                return Json(new { success = false, message = "Acceso denegado." });
            }

            try
            {
                // 1. LLAMAR al método del servicio para actualizar la fecha
                bool resultado = await _monitoreoService.HabilitarCaptura(idProceso, mes);

                if (resultado)
                {
                    return Json(new { success = true, message = "Campo de fecha actualizado." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo encontrar o actualizar el registro." });
                }
            }
            catch (Exception ex)
            {
                // Loggear el error (recomendado)
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        public async Task<IActionResult> TableroControl(int anoFiscal, int departamento)
        {
            ViewBag.AnoFiscal = anoFiscal;
            ViewBag.Departamento = departamento;
            return View();
        }

        public async Task<IActionResult> ObtenerDatosTableroControl(int anoFiscal, int departamento)
        {
            try
            {
                var datos = await _programacionService.ObtenerDatosProgramacion(anoFiscal, departamento);

                var datosParaJSON = datos.Select(x => new
                {
                    x.IdProceso,
                    x.Componente,
                    x.Actividad,
                    x.DescripcionActividad,
                    x.UnidadMedida,
                    x.ProgramaSocial,
                    NombreReviso = x.NombreRealizo ?? "Sin asignar",
                    NombreValido = x.NombreValido ?? "Sin asignar",
                    CargoReviso = x.CargoRealizo ?? "Sin asignar",
                    CargoValido = x.CargoValido ?? "Sin asignar",
                    ProPre = x.IdppNavigation.NombrePp ?? "Sin programa",
                    Programado = new
                    {
                        Ene = x.TotalEnero,
                        Feb = x.TotalFebrero,
                        Mar = x.TotalMarzo,
                        Abr = x.TotalAbril,
                        May = x.TotalMayo,
                        Jun = x.TotalJunio,
                        Jul = x.TotalJulio,
                        Ago = x.TotalAgosto,
                        Sep = x.TotalSeptiembre,
                        Oct = x.TotalOctubre,
                        Nov = x.TotalNoviembre,
                        Dic = x.TotalDiciembre,
                        Total = (x.TotalEnero ?? 0) + (x.TotalFebrero ?? 0) + (x.TotalMarzo ?? 0)
                            + (x.TotalAbril ?? 0) + (x.TotalMayo ?? 0) + (x.TotalJunio ?? 0)
                            + (x.TotalJulio ?? 0) + (x.TotalAgosto ?? 0) + (x.TotalSeptiembre ?? 0)
                            + (x.TotalOctubre ?? 0) + (x.TotalNoviembre ?? 0) + (x.TotalDiciembre ?? 0)
                    },
                    Realizado = new
                    {
                        Ene = x.TotalEneroRealizado,
                        Feb = x.TotalFebreroRealizado,
                        Mar = x.TotalMarzoRealizado,
                        Abr = x.TotalAbrilRealizado,
                        May = x.TotalMayoRealizado,
                        Jun = x.TotalJunioRealizado,
                        Jul = x.TotalJulioRealizado,
                        Ago = x.TotalAgostoRealizado,
                        Sep = x.TotalSeptiembreRealizado,
                        Oct = x.TotalOctubreRealizado,
                        Nov = x.TotalNoviembreRealizado,
                        Dic = x.TotalDiciembreRealizado,
                        Total = x.TotalRealizado ?? 0
                    }
                }).ToList();

                var primerRegistro = datos.FirstOrDefault();
                var llenadoInterno = new
                {
                    Ano = primerRegistro?.Ano ?? DateTime.Now.Year
                };

                return Json(new
                {
                    success = true,
                    datos = datosParaJSON,
                    llenadoInterno = llenadoInterno
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = ex.Message
                });
            }
        }

        public async Task<IActionResult> ObtenerDatosEdicion(int idProceso)
        {
            var registro = await _programacionService.ObtenerporId(idProceso);
            var medidas = await _departamentoService.ObtenerMedidas();

            if (registro == null)
            {
                return NotFound();
            }

            var datosEdicion = new VMDatosEdicionActividad
            {
                IdProceso = registro.IdProceso,
                PP = registro.Idpp,
                Componente = registro.Componente,
                Actividad = registro.Actividad,
                DescripcionActividad = registro.DescripcionActividad,
                UnidadMedida = registro.UnidadMedida.ToUpperInvariant(),
                ProgramaSocial = registro.ProgramaSocial,
                TotalEnero = registro.TotalEnero ?? 0,
                TotalFebrero = registro.TotalFebrero ?? 0,
                TotalMarzo = registro.TotalMarzo ?? 0,
                TotalAbril = registro.TotalAbril ?? 0,
                TotalMayo = registro.TotalMayo ?? 0,
                TotalJunio = registro.TotalJunio ?? 0,
                TotalJulio = registro.TotalJulio ?? 0,
                TotalAgosto = registro.TotalAgosto ?? 0,
                TotalSeptiembre = registro.TotalSeptiembre ?? 0,
                TotalOctubre = registro.TotalOctubre ?? 0,
                TotalNoviembre = registro.TotalNoviembre ?? 0,
                TotalDiciembre = registro.TotalDiciembre ?? 0,

                EneroPersona = registro.EneroPersona ?? 0,
                FebreroPersona = registro.FebreroPersona ?? 0,
                MarzoPersona = registro.MarzoPersona ?? 0,
                AbrilPersona = registro.AbrilPersona ?? 0,
                MayoPersona = registro.MayoPersona ?? 0,
                JunioPersona = registro.JunioPersona ?? 0,
                JulioPersona = registro.JulioPersona ?? 0,
                AgostoPersona = registro.AgostoPersona ?? 0,
                SeptiembrePersona = registro.SeptiembrePersona ?? 0,
                OctubrePersona = registro.OctubrePersona ?? 0,
                NoviembrePersona = registro.NoviembrePersona ?? 0,
                DiciembrePersona = registro.DiciembrePersona ?? 0,
                ListaMedidas = medidas
                    .Select(g => new SelectListItem
                    {
                        Value = g.Valor.ToUpperInvariant(),
                        Text = g.Valor,
                        Selected = g.Valor.ToUpperInvariant() == registro.UnidadMedida.ToUpperInvariant()
                    })
                    .ToList()
            };

            // Devuelve el objeto, ASP.NET Core lo serializa automáticamente a JSON
            return Json(datosEdicion);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDatosJustificacion(int idProceso)
        {
            var registro = await _programacionService.ObtenerporId(idProceso);
            if (registro == null)
            {
                return NotFound();
            }

            return Json(new
            {
                pp = registro.Pp,
                componente = registro.Componente,
                actividad = registro.Actividad,
                descripcionActividad = registro.DescripcionActividad,
                nombreRealizo = registro.NombreRealizo,
                cargoRealizo = registro.CargoRealizo,
                nombreValido = registro.NombreValido,
                cargoValido = registro.CargoValido
            });
        }

        [HttpPost]
        public async Task<IActionResult> GuardarEdicion(VMDatosEdicionActividad modelo)
        {
            try
            {
                var model = _mapper.Map<DatosEdicionDTO>(modelo);

                bool exito = await _monitoreoService.ActualizarRegistro(model);

                if (exito)
                {
                    return Json(new { success = true, message = "La actividad se ha actualizado correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "Fallo la actualización: El servicio no pudo guardar los datos." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Ocurrió un error interno del servidor. {ex.Message}" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")] // Solo admin puede agregar
        public async Task<IActionResult> GuardarNuevo(VMDatosEdicionActividad modelo)
        {
            try
            {
                // Validación manual de campos requeridos
                if (modelo.AnoFiscal == 0)
                {
                    return Json(new { success = false, message = "El año fiscal es requerido." });
                }

                if (modelo.Componente == 0)
                {
                    return Json(new { success = false, message = "El componente es requerido." });
                }

                if (modelo.Actividad == 0)
                {
                    return Json(new { success = false, message = "La actividad es requerida." });
                }


                // Normalizar UnidadMedida antes del mapeo
                modelo.UnidadMedida = modelo.UnidadMedida?.ToUpperInvariant();

                // Mapear directamente - ahora los nombres coinciden
                var nuevoRegistro = _mapper.Map<DatosEdicionDTO>(modelo);

                bool resultado = await _monitoreoService.CrearNuevoProceso(nuevoRegistro);

                if (resultado)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Registro creado exitosamente"
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "No se pudo crear el registro"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Error al guardar: {ex.Message}"
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DescargarPlantilla()
        {
            var departamentos = await _departamentoService.ObtenerDepartamentos();
            var medidas = await _departamentoService.ObtenerMedidas();

            using var workbook = new XLWorkbook();
            var hoja = workbook.Worksheets.Add("Actividades");

            string[] encabezados = new string[]
            {
                "Año Fiscal", "Programa Presupuestario", "Componente", "Actividad", "Descripción Actividad",
                "Unidad Medida", "Programa Social", "Departamento",
                "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic",
                "Ene Personas", "Feb Personas", "Mar Personas", "Abr Personas", "May Personas", "Jun Personas",
                "Jul Personas", "Ago Personas", "Sep Personas", "Oct Personas", "Nov Personas", "Dic Personas"
            };

            for (int i = 0; i < encabezados.Length; i++)
            {
                hoja.Cell(1, i + 1).Value = encabezados[i];
            }

            var rangoEncabezados = hoja.Range(1, 1, 1, encabezados.Length);
            rangoEncabezados.Style.Font.Bold = true;
            rangoEncabezados.Style.Font.FontColor = XLColor.White;
            rangoEncabezados.Style.Fill.BackgroundColor = XLColor.FromHtml("#C8B6D8");
            rangoEncabezados.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            // Listas desplegables: catálogos en hoja oculta "Listas" y validaciones
            // por referencia a rango (evita que Excel marque reparación por listas literales)
            var hojaListas = workbook.Worksheets.Add("Listas");

            hojaListas.Cell(1, 1).Value = "Programa Presupuestario";
            hojaListas.Cell(1, 2).Value = "Unidad Medida";
            hojaListas.Cell(1, 3).Value = "Departamento";
            hojaListas.Range(1, 1, 1, 3).Style.Font.Bold = true;

            hojaListas.Cell(2, 1).Value = "Agenda";
            hojaListas.Cell(3, 1).Value = "E046";
            hojaListas.Cell(4, 1).Value = "E047";

            var nombresMedidas = medidas
                .Where(m => !string.IsNullOrWhiteSpace(m.Valor))
                .Select(m => m.Valor.Trim())
                .ToList();

            for (int i = 0; i < nombresMedidas.Count; i++)
            {
                hojaListas.Cell(i + 2, 2).Value = nombresMedidas[i];
            }

            var nombresDepartamentos = departamentos
                .Where(d => !string.IsNullOrWhiteSpace(d.Departamento1))
                .Select(d => d.Departamento1.Trim())
                .ToList();

            for (int i = 0; i < nombresDepartamentos.Count; i++)
            {
                hojaListas.Cell(i + 2, 3).Value = nombresDepartamentos[i];
            }

            // Validaciones aplicadas a las filas de datos (2-150), no al encabezado
            const int FILAS_DATOS = 150;
            string refMedidas = $"'Listas'!$B$2:$B${nombresMedidas.Count + 1}";
            string refDepartamentos = $"'Listas'!$C$2:$C${nombresDepartamentos.Count + 1}";

            hoja.Range(2, 2, FILAS_DATOS, 2).CreateDataValidation().List("'Listas'!$A$2:$A$4");
            if (nombresMedidas.Any())
            {
                hoja.Range(2, 6, FILAS_DATOS, 6).CreateDataValidation().List(refMedidas);
            }
            if (nombresDepartamentos.Any())
            {
                hoja.Range(2, 8, FILAS_DATOS, 8).CreateDataValidation().List(refDepartamentos);
            }

            hojaListas.Hide();

            hoja.SheetView.FreezeRows(1);
            hoja.Columns().AdjustToContents();

            var instrucciones = workbook.Worksheets.Add("Instrucciones");
            instrucciones.Cell(1, 1).Value = "Instrucciones para llenar la plantilla";
            instrucciones.Cell(1, 1).Style.Font.Bold = true;
            instrucciones.Cell(1, 1).Style.Font.FontSize = 14;

            string[] pasos = new string[]
            {
                "1. Llena una fila por cada actividad a registrar. No dejes filas en blanco en medio.",
                "2. Año Fiscal: año del ejercicio (ej. 2026).",
                "3. Programa Presupuestario: elige de la lista desplegable (Agenda, E046 o E047).",
                "4. Componente y Actividad: números obligatorios mayores a cero.",
                "5. Descripción Actividad: texto descriptivo de la actividad.",
                "6. Unidad Medida: elige de la lista desplegable.",
                "7. Programa Social: opcional, texto.",
                "8. Departamento: elige de la lista desplegable. Puedes cargar varios departamentos en el mismo archivo.",
                "9. Columnas Ene-Dic: meta programada del mes (números, puede ser 0 o vacío).",
                "10. Columnas con 'Personas': personas programadas por mes (números, puede ser 0 o vacío).",
                "11. Guarda el archivo como .xlsx y súbelo desde el botón 'Cargar Excel' en Monitoreo.",
                "12. Si una fila tiene errores, se omitirá y se mostrará el detalle al final del proceso."
            };

            for (int i = 0; i < pasos.Length; i++)
            {
                instrucciones.Cell(i + 3, 1).Value = pasos[i];
            }
            instrucciones.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;

            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Plantilla_Actividades.xlsx");
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ValidarCargarExcel(IFormFile archivoExcel)
        {
            try
            {
                var resultado = await ParsearArchivo(archivoExcel);

                if (!resultado.CorrectoArchivo)
                {
                    return Json(new { success = false, message = resultado.MensajeError });
                }

                return Json(new
                {
                    success = true,
                    totalFilas = resultado.TotalFilas,
                    registrosValidos = resultado.RegistrosValidos.Count,
                    totalErrores = resultado.Errores.Count,
                    errores = resultado.Errores.Take(50).ToList(),
                    message = $"Se encontraron {resultado.RegistrosValidos.Count} registro(s) válido(s) de {resultado.TotalFilas} fila(s)."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al validar el archivo: {ex.Message}" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CargarExcel(IFormFile archivoExcel)
        {
            try
            {
                var resultado = await ParsearArchivo(archivoExcel);

                if (!resultado.CorrectoArchivo)
                {
                    return Json(new { success = false, message = resultado.MensajeError });
                }

                int exitosos = 0;
                int fallidos = 0;
                var errores = new List<string>(resultado.Errores);

                foreach (var dto in resultado.RegistrosValidos)
                {
                    try
                    {
                        bool exito = await _monitoreoService.CrearNuevoProceso(dto);
                        if (exito)
                        {
                            exitosos++;
                        }
                        else
                        {
                            fallidos++;
                            errores.Add("Un registro válido no se pudo guardar en la base de datos.");
                        }
                    }
                    catch (Exception ex)
                    {
                        fallidos++;
                        errores.Add($"Error al guardar: {ex.Message}");
                    }
                }

                return Json(new
                {
                    success = exitosos > 0,
                    totalProcesados = resultado.RegistrosValidos.Count,
                    exitosos,
                    fallidos,
                    errores = errores.Take(50).ToList(),
                    message = $"Se procesaron {resultado.RegistrosValidos.Count} registro(s) válido(s): {exitosos} creados, {fallidos} fallidos."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al procesar el archivo: {ex.Message}" });
            }
        }

        private async Task<ResultadoParseoExcel> ParsearArchivo(IFormFile archivoExcel)
        {
            var resultado = new ResultadoParseoExcel();

            if (archivoExcel == null || archivoExcel.Length == 0)
            {
                resultado.MensajeError = "Debes seleccionar un archivo Excel.";
                return resultado;
            }

            var extension = Path.GetExtension(archivoExcel.FileName).ToLowerInvariant();
            if (extension != ".xlsx")
            {
                resultado.MensajeError = "El archivo debe tener extensión .xlsx.";
                return resultado;
            }

            var departamentos = await _departamentoService.ObtenerDepartamentos();
            var medidas = await _departamentoService.ObtenerMedidas();

            var medidasValidas = medidas
                .Where(m => !string.IsNullOrWhiteSpace(m.Valor))
                .Select(m => m.Valor.Trim().ToUpperInvariant())
                .ToHashSet();

            using (var stream = new MemoryStream())
            {
                await archivoExcel.CopyToAsync(stream);
                stream.Position = 0;

                using (var workbook = new XLWorkbook(stream))
                {
                    IXLWorksheet hoja;
                    if (!workbook.TryGetWorksheet("Actividades", out hoja))
                    {
                        resultado.MensajeError = "El archivo no contiene la hoja 'Actividades'.";
                        return resultado;
                    }

                    var filas = hoja.RowsUsed();
                    if (!filas.Any())
                    {
                        resultado.MensajeError = "El archivo no contiene datos.";
                        return resultado;
                    }

                    resultado.CorrectoArchivo = true;

                    foreach (var fila in filas.Skip(1))
                    {
                        resultado.TotalFilas++;
                        int numeroFila = fila.RowNumber();

                        try
                        {
                            var modelo = new VMDatosEdicionActividad();

                            // Año Fiscal
                            if (!int.TryParse(fila.Cell(1).GetString().Trim(), out int anoFiscal) || anoFiscal <= 0)
                            {
                                resultado.Errores.Add($"Fila {numeroFila}: Año Fiscal inválido o vacío.");
                                continue;
                            }
                            modelo.AnoFiscal = anoFiscal;

                            // Programa Presupuestario
                            var idPP = ResolverProgramaPresupuestario(fila.Cell(2).GetString().Trim());
                            if (idPP == null)
                            {
                                resultado.Errores.Add($"Fila {numeroFila}: Programa Presupuestario inválido. Use Agenda, E046 o E047.");
                                continue;
                            }
                            modelo.PP = idPP;

                            // Componente
                            modelo.Componente = LeerEnteroEntero(fila, 3);
                            if (modelo.Componente == null || modelo.Componente <= 0)
                            {
                                resultado.Errores.Add($"Fila {numeroFila}: Componente inválido o vacío.");
                                continue;
                            }

                            // Actividad
                            modelo.Actividad = LeerEnteroEntero(fila, 4);
                            if (modelo.Actividad == null || modelo.Actividad <= 0)
                            {
                                resultado.Errores.Add($"Fila {numeroFila}: Actividad inválida o vacía.");
                                continue;
                            }

                            // Descripción Actividad
                            modelo.DescripcionActividad = fila.Cell(5).GetString().Trim();
                            if (string.IsNullOrEmpty(modelo.DescripcionActividad))
                            {
                                resultado.Errores.Add($"Fila {numeroFila}: Descripción de la actividad es requerida.");
                                continue;
                            }

                            // Unidad de Medida
                            var unidadRaw = fila.Cell(6).GetString().Trim().ToUpperInvariant();
                            if (!medidasValidas.Contains(unidadRaw))
                            {
                                resultado.Errores.Add($"Fila {numeroFila}: Unidad de medida '{fila.Cell(6).GetString().Trim()}' no válida.");
                                continue;
                            }
                            modelo.UnidadMedida = unidadRaw;

                            // Programa Social (opcional)
                            modelo.ProgramaSocial = fila.Cell(7).GetString().Trim();

                            // Departamento -> resolver Área desde BD
                            var deptoTexto = fila.Cell(8).GetString().Trim();
                            var departamento = departamentos.FirstOrDefault(d =>
                                !string.IsNullOrWhiteSpace(d.Departamento1)
                                && string.Equals(d.Departamento1.Trim(), deptoTexto, StringComparison.OrdinalIgnoreCase));
                            if (departamento == null)
                            {
                                resultado.Errores.Add($"Fila {numeroFila}: Departamento '{deptoTexto}' no encontrado.");
                                continue;
                            }
                            modelo.Departamento = departamento.Departamento1;
                            modelo.Area = departamento.Area;

                            // Metas Programadas (columnas 9-20): Ene, Feb, Mar, Abr, May, Jun, Jul, Ago, Sep, Oct, Nov, Dic
                            modelo.TotalEnero = LeerEnteroEntero(fila, 9) ?? 0;
                            modelo.TotalFebrero = LeerEnteroEntero(fila, 10) ?? 0;
                            modelo.TotalMarzo = LeerEnteroEntero(fila, 11) ?? 0;
                            modelo.TotalAbril = LeerEnteroEntero(fila, 12) ?? 0;
                            modelo.TotalMayo = LeerEnteroEntero(fila, 13) ?? 0;
                            modelo.TotalJunio = LeerEnteroEntero(fila, 14) ?? 0;
                            modelo.TotalJulio = LeerEnteroEntero(fila, 15) ?? 0;
                            modelo.TotalAgosto = LeerEnteroEntero(fila, 16) ?? 0;
                            modelo.TotalSeptiembre = LeerEnteroEntero(fila, 17) ?? 0;
                            modelo.TotalOctubre = LeerEnteroEntero(fila, 18) ?? 0;
                            modelo.TotalNoviembre = LeerEnteroEntero(fila, 19) ?? 0;
                            modelo.TotalDiciembre = LeerEnteroEntero(fila, 20) ?? 0;

                            // Personas Programadas (columnas 21-32)
                            modelo.EneroPersona = LeerEnteroEntero(fila, 21) ?? 0;
                            modelo.FebreroPersona = LeerEnteroEntero(fila, 22) ?? 0;
                            modelo.MarzoPersona = LeerEnteroEntero(fila, 23) ?? 0;
                            modelo.AbrilPersona = LeerEnteroEntero(fila, 24) ?? 0;
                            modelo.MayoPersona = LeerEnteroEntero(fila, 25) ?? 0;
                            modelo.JunioPersona = LeerEnteroEntero(fila, 26) ?? 0;
                            modelo.JulioPersona = LeerEnteroEntero(fila, 27) ?? 0;
                            modelo.AgostoPersona = LeerEnteroEntero(fila, 28) ?? 0;
                            modelo.SeptiembrePersona = LeerEnteroEntero(fila, 29) ?? 0;
                            modelo.OctubrePersona = LeerEnteroEntero(fila, 30) ?? 0;
                            modelo.NoviembrePersona = LeerEnteroEntero(fila, 31) ?? 0;
                            modelo.DiciembrePersona = LeerEnteroEntero(fila, 32) ?? 0;

                            resultado.RegistrosValidos.Add(_mapper.Map<DatosEdicionDTO>(modelo));
                        }
                        catch (Exception ex)
                        {
                            resultado.Errores.Add($"Fila {numeroFila}: {ex.Message}");
                        }
                    }
                }
            }

            return resultado;
        }

        private class ResultadoParseoExcel
        {
            public bool CorrectoArchivo { get; set; }
            public string MensajeError { get; set; } = string.Empty;
            public List<DatosEdicionDTO> RegistrosValidos { get; set; } = new List<DatosEdicionDTO>();
            public List<string> Errores { get; set; } = new List<string>();
            public int TotalFilas { get; set; }
        }

        private int? LeerEnteroEntero(IXLRow fila, int columna)
        {
            var celda = fila.Cell(columna);
            if (celda.IsEmpty())
            {
                return null;
            }

            var texto = celda.GetString().Trim().Replace(",", "").Replace(" ", "");
            if (string.IsNullOrEmpty(texto))
            {
                return null;
            }

            if (int.TryParse(texto, out int valor))
            {
                return valor;
            }

            if (celda.DataType == XLDataType.Number)
            {
                return (int)Math.Round(celda.GetDouble());
            }

            return null;
        }

        private int? ResolverProgramaPresupuestario(string valor)
        {
            if (int.TryParse(valor.Trim(), out int id))
            {
                if (id == 1 || id == 2 || id == 3)
                {
                    return id;
                }
                return null;
            }

            return valor.Trim().ToUpperInvariant() switch
            {
                "AGENDA" => 3,
                "E046" => 1,
                "E047" => 2,
                _ => null
            };
        }

        [HttpPost]
        public async Task<JsonResult> EliminarCapturaMes(int idProceso, int mes)
        {
            try
            {
                bool resultado = await _monitoreoService.EliminarCapturaMes(idProceso, mes);

                if (resultado)
                {
                    return Json(new
                    {
                        success = true,
                        mensaje = $"Se eliminó correctamente la captura de {ObtenerNombreMes(mes)}"
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        mensaje = "No se pudo eliminar la captura del mes"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    mensaje = $"Error: {ex.Message}"
                });
            }
        }

        private string ObtenerNombreMes(int mes)
        {
            return mes switch
            {
                1 => "Enero",
                2 => "Febrero",
                3 => "Marzo",
                4 => "Abril",
                5 => "Mayo",
                6 => "Junio",
                7 => "Julio",
                8 => "Agosto",
                9 => "Septiembre",
                10 => "Octubre",
                11 => "Noviembre",
                12 => "Diciembre",
                _ => "Desconocido"
            };
        }

        public async Task<IActionResult> ObtenerAreaPorDepartamento(int idDepartamento)
        {
            try
            {
                var departamentos = await _departamentoService.ObtenerDepartamentos();
                var departamento = departamentos.FirstOrDefault(d => d.IdDepartamento == idDepartamento);

                if (departamento != null)
                {
                    return Json(new { success = true, area = departamento.Area });
                }
                else
                {
                    return Json(new { success = false, message = "Departamento no encontrado" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> ObtenerUnidadesMedida()
        {
            try
            {
                var medidas = await _departamentoService.ObtenerMedidas();

                var lista = medidas.Select(m => new SelectListItem
                {
                    Value = m.Valor.ToUpperInvariant(),
                    Text = m.Valor
                }).ToList();

                return Json(lista);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> TableroCompletoDepartamento(int? anoFiscal, int? departamento)
        {
            var departamentos = await _departamentoService.ObtenerDepartamentos();

            var modelo = new VMDepartamentos
            {
                ListaDepartamentos = departamentos.Select(d => new SelectListItem
                {
                    Value = d.IdDepartamento.ToString(),
                    Text = d.Departamento1
                }).OrderBy(item => item.Text)
                .ToList()
            };

            // Pasar los valores por ViewBag para preseleccionar
            ViewBag.AnoFiscalSeleccionado = anoFiscal;
            ViewBag.DepartamentoSeleccionado = departamento;

            return View(modelo);
        }

        public async Task<IActionResult> ObtenerDatosTableroCompletoOptimizado(int anoFiscal, int? departamento)
        {
            try
            {
                var datos = await _programacionService.ObtenerDatosProgramacion(anoFiscal, departamento);

                if (datos == null || !datos.Any())
                {
                    return Json(new { success = true, datos = new List<object>() });
                }

                var idsProcesos = datos.Select(d => d.IdProceso).ToList();
                var todosLosLlenados = await _monitoreoService.ObtenerLlenadosPorProcesos(idsProcesos);

                var resultado = datos.Select(x =>
                {
                    var personasAtendidas = new Dictionary<int, object>();

                    for (int mes = 1; mes <= 12; mes++)
                    {
                        var datosExternos = todosLosLlenados?
                            .FirstOrDefault(l => l.IdProceso == x.IdProceso && l.Meses == mes);

                        personasAtendidas[mes] = new
                        {
                            hombres = datosExternos?.HombresAtendidos ?? 0,
                            mujeres = datosExternos?.MujeresAtendidas ?? 0,
                            edad0a3 = datosExternos?._03anos ?? 0,
                            edad4a8 = datosExternos?._48anos ?? 0,
                            edad9a12 = datosExternos?._912anos ?? 0,
                            edad13a17 = datosExternos?._1317anos ?? 0,
                            edad18a29 = datosExternos?._1829anos ?? 0,
                            edad30a59 = datosExternos?._3059anos ?? 0,
                            edad60mas = datosExternos?._60amasanos ?? 0,
                            noEspecifica = datosExternos?.NoDefinida ?? 0,
                            indigenas = datosExternos?.Indigena ?? 0
                        };
                    }

                    return new
                    {
                        idProceso = x.IdProceso,
                        pp = x.Pp,
                        componente = x.Componente,
                        actividad = x.Actividad,
                        descripcionActividad = x.DescripcionActividad,
                        area = x.Area,
                        departamento = x.Departamento,
                        programaSocial = x.ProgramaSocial,
                        unidadMedida = x.UnidadMedida,
                        programado = new
                        {
                            ene = x.TotalEnero ?? 0,
                            feb = x.TotalFebrero ?? 0,
                            mar = x.TotalMarzo ?? 0,
                            abr = x.TotalAbril ?? 0,
                            may = x.TotalMayo ?? 0,
                            jun = x.TotalJunio ?? 0,
                            jul = x.TotalJulio ?? 0,
                            ago = x.TotalAgosto ?? 0,
                            sep = x.TotalSeptiembre ?? 0,
                            oct = x.TotalOctubre ?? 0,
                            nov = x.TotalNoviembre ?? 0,
                            dic = x.TotalDiciembre ?? 0,
                            total = x.TotalProgramado ?? 0
                        },
                        realizado = new
                        {
                            ene = x.TotalEneroRealizado ?? 0,
                            feb = x.TotalFebreroRealizado ?? 0,
                            mar = x.TotalMarzoRealizado ?? 0,
                            abr = x.TotalAbrilRealizado ?? 0,
                            may = x.TotalMayoRealizado ?? 0,
                            jun = x.TotalJunioRealizado ?? 0,
                            jul = x.TotalJulioRealizado ?? 0,
                            ago = x.TotalAgostoRealizado ?? 0,
                            sep = x.TotalSeptiembreRealizado ?? 0,
                            oct = x.TotalOctubreRealizado ?? 0,
                            nov = x.TotalNoviembreRealizado ?? 0,
                            dic = x.TotalDiciembreRealizado ?? 0,
                            total = x.TotalRealizado ?? 0
                        },
                        personasProgramado = new
                        {
                            ene = x.EneroPersona ?? 0,
                            feb = x.FebreroPersona ?? 0,
                            mar = x.MarzoPersona ?? 0,
                            abr = x.AbrilPersona ?? 0,
                            may = x.MayoPersona ?? 0,
                            jun = x.JunioPersona ?? 0,
                            jul = x.JulioPersona ?? 0,
                            ago = x.AgostoPersona ?? 0,
                            sep = x.SeptiembrePersona ?? 0,
                            oct = x.OctubrePersona ?? 0,
                            nov = x.NoviembrePersona ?? 0,
                            dic = x.DiciembrePersona ?? 0,
                            total = (x.EneroPersona ?? 0) + (x.FebreroPersona ?? 0) + (x.MarzoPersona ?? 0) +
                                    (x.AbrilPersona ?? 0) + (x.MayoPersona ?? 0) + (x.JunioPersona ?? 0) +
                                    (x.JulioPersona ?? 0) + (x.AgostoPersona ?? 0) + (x.SeptiembrePersona ?? 0) +
                                    (x.OctubrePersona ?? 0) + (x.NoviembrePersona ?? 0) + (x.DiciembrePersona ?? 0)
                        },
                        personasAtendidas = personasAtendidas
                    };
                }).ToList();

                return Json(new { success = true, datos = resultado });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = $"Error: {ex.Message}" });
            }
        }
    }
}