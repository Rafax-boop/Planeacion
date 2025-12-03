using AutoMapper;
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

            var modelo = new VMDepartamentos
            {
                ListaDepartamentos = departamentos.Select(d => new SelectListItem
                {
                    Value = d.IdDepartamento.ToString(),
                    Text = d.Departamento1
                }).OrderBy(item => item.Text)
                .ToList()
            };
            return View(modelo);
        }

        public async Task<IActionResult> ObtenerDatos(int anoFiscal, int departamento)
        {
            try
            {
                bool esAdmin = User.IsInRole("Administrador");

                // Obtener datos de programación
                var datos = await _programacionService.ObtenerDatosProgramacion(anoFiscal, departamento);

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

            var modelo = new VMGuardarActualizacion
            {
                IdProceso = idProceso,
                Mes = fechaRegistro.Mes,
                MesNum = mes,
                FechaFin = fechaRegistro.FechaFin,
                Total = totalMes,
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
            // Variables de ruta inicializadas a null
            string rutaEvidencia = null;
            string rutaJustificacion = null;

            // Variables de ruta para construcción
            string idProceso = modelo.IdProceso.ToString();
            string mes = modelo.Mes.ToString(); // El mes es un INT (1-12)

            // Definición de las carpetas base en wwwroot
            const string BASE_FOLDER_EVIDENCIA = "Evidencia";
            const string BASE_FOLDER_JUSTIFICACION = "Justificacion";

            // 1. Definir rutas físicas completas para las carpetas destino (ej: wwwroot/Evidencia/123/5)
            string folderPathEvidencia = Path.Combine(_hostEnvironment.WebRootPath, BASE_FOLDER_EVIDENCIA, idProceso, mes);
            string folderPathJustificacion = Path.Combine(_hostEnvironment.WebRootPath, BASE_FOLDER_JUSTIFICACION, idProceso, mes);

            try
            {
                // 2. PROCESAMIENTO DE ARCHIVOS Y CREACIÓN DE CARPETAS

                // A. Evidencia
                if (modelo.InputEvidencia != null && modelo.InputEvidencia.Length > 0)
                {
                    // Crear la estructura de carpetas para Evidencia si no existe
                    Directory.CreateDirectory(folderPathEvidencia);

                    // Generar nombre de archivo único: [IdProceso]_E_[Mes]_[Ticks].ext
                    string extension = Path.GetExtension(modelo.InputEvidencia.FileName);
                    // Opción Segura (Recomendada) para Evidencia:
                    string originalFileName = Path.GetFileName(modelo.InputEvidencia.FileName);
                    // Se asegura unicidad por proceso/mes y aún conserva el nombre original
                    string newFileName = $"{idProceso}_E_{mes}_{originalFileName}";

                    // Ruta física donde se guardará el archivo
                    string rutaFisicaEvidencia = Path.Combine(folderPathEvidencia, newFileName);

                    // Guardar el archivo físicamente
                    using (var fileStream = new FileStream(rutaFisicaEvidencia, FileMode.Create))
                    {
                        await modelo.InputEvidencia.CopyToAsync(fileStream);
                    }

                    // Guardamos la RUTA RELATIVA para la DB (ej: /Evidencia/123/5/nombre.ext)
                    rutaEvidencia = $"/{BASE_FOLDER_EVIDENCIA}/{idProceso}/{mes}/{newFileName}";
                }

                // B. Justificación
                if (modelo.InputJustificacion != null && modelo.InputJustificacion.Length > 0)
                {
                    // Crear la estructura de carpetas para Justificación si no existe
                    Directory.CreateDirectory(folderPathJustificacion);

                    // Generar nombre de archivo único: [IdProceso]_J_[Mes]_[Ticks].ext
                    string extension = Path.GetExtension(modelo.InputJustificacion.FileName);
                    // Opción Segura (Recomendada) para Evidencia:
                    string originalFileName = Path.GetFileName(modelo.InputEvidencia.FileName);
                    // Se asegura unicidad por proceso/mes y aún conserva el nombre original
                    string newFileName = $"{idProceso}_J_{mes}_{originalFileName}";

                    // Ruta física donde se guardará el archivo
                    string rutaFisicaJustificacion = Path.Combine(folderPathJustificacion, newFileName);

                    // Guardar el archivo físicamente
                    using (var fileStream = new FileStream(rutaFisicaJustificacion, FileMode.Create))
                    {
                        await modelo.InputJustificacion.CopyToAsync(fileStream);
                    }

                    // Guardamos la RUTA RELATIVA para la DB (ej: /Justificacion/123/5/nombre.ext)
                    rutaJustificacion = $"/{BASE_FOLDER_JUSTIFICACION}/{idProceso}/{mes}/{newFileName}";
                }

                // 3. LLAMADA AL SERVICIO Y PERSISTENCIA DE DATOS (Tu lógica de negocio)
                bool resultado = await _monitoreoService.GuardarActualizacion(
                    modelo,
                    rutaEvidencia,
                    rutaJustificacion
                );

                // 4. MANEJO DE RESULTADO Y RESPUESTA JSON (para la función AJAX en JS)

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
            try
            {
                // Obtener los mismos datos que en la vista Monitoreo
                var datos = await _programacionService.ObtenerDatosProgramacion(anoFiscal, departamento);

                var datosParaJSON = datos.Select(x => new
                {
                    x.IdProceso,
                    x.Componente,
                    x.Actividad,
                    x.DescripcionActividad,
                    x.UnidadMedida,
                    x.ProgramaSocial,
                    // Asumo que tu entidad x tiene estas propiedades para Programado y Realizado:
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
                        Total = x.TotalProgramado
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
                        Total = x.TotalRealizado
                    }
                }).ToList();

                // Serializar la lista a una cadena JSON
                string datosJson = System.Text.Json.JsonSerializer.Serialize(datosParaJSON);

                // Obtener datos de llenado interno
                var primerRegistro = datos.FirstOrDefault();

                // Agregar logs para depuración
                System.Diagnostics.Debug.WriteLine($"NombreRealizo: {primerRegistro?.NombreRealizo}");
                System.Diagnostics.Debug.WriteLine($"NombreValido: {primerRegistro?.NombreValido}");

                var llenadoInterno = new
                {
                    NombreReviso = primerRegistro?.NombreRealizo ?? "Sin asignar",
                    NombreValido = primerRegistro?.NombreValido ?? "Sin asignar",
                    CargoReviso = primerRegistro?.CargoRealizo ?? "Sin asignar",
                    CargoValido = primerRegistro?.CargoValido ?? "Sin asignar",  
                    Ano = primerRegistro?.Ano ?? DateTime.Now.Year
                };

                string llenadoInternoJson = System.Text.Json.JsonSerializer.Serialize(llenadoInterno);
                ViewBag.LlenadoInternoJson = llenadoInternoJson;

                ViewBag.AnoFiscal = anoFiscal;
                ViewBag.Departamento = departamento;
                ViewBag.NumeroRegistros = datosParaJSON.Count;
                ViewBag.DatosJson = datosJson;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.NumeroRegistros = 0;
                ViewBag.LlenadoInternoJson = System.Text.Json.JsonSerializer.Serialize(new
                {
                    NombreReviso = "Sin asignar",
                    NombreValido = "Sin asignar"
                });
                return View();
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
    }
}
