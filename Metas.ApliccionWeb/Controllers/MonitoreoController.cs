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
            var areaClaim = User.Claims.FirstOrDefault(c => c.Type == "Departamento")?.Value;
            bool esAdmin = User.IsInRole("Administrador");

            IEnumerable<SelectListItem> listaDepartamentos;

            if (esAdmin)
            {
                listaDepartamentos = departamentos
                    .Select(d => new SelectListItem
                    {
                        Value = d.IdDepartamento.ToString(),
                        Text = d.Departamento1
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
                            Text = d.Departamento1
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
                            Text = d.Departamento1
                        })
                        .OrderBy(item => item.Text);
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