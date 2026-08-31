using Metas.ApliccionWeb.Models;
using Metas.AplicacionWeb.Models.ViewModels;
using Metas.BLL.DTO;
using Metas.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Security.Claims;

namespace Metas.ApliccionWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDepartamentoService _departamentoService;
        private readonly IWebHostEnvironment _hostEnvironment;

        private const string BASE_FOLDER_DOCUMENTOS = "Documentos";
        private const long TAMANO_MAXIMO_BYTES = 50L * 1024 * 1024; // 50 MB
        private static readonly string[] ExtensionesPermitidas = { ".pdf", ".xlsx" };

        public HomeController(ILogger<HomeController> logger, IDepartamentoService departamentoService, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _departamentoService = departamentoService;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Documentos(int idDepartamento = 0)
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
                    .OrderBy(item => item.Text)
                    .ToList();
            }
            else
            {
                if (User.IsInRole("Departamento"))
                {
                    listaDepartamentos = departamentos
                        .Where(d => d.Departamento1 == areaClaim)
                        .Select(d => new SelectListItem
                        {
                            Value = d.IdDepartamento.ToString(),
                            Text = d.Departamento1
                        })
                        .ToList();
                }
                else
                {
                    listaDepartamentos = departamentos
                        .Where(d => d.Area == areaClaim)
                        .Select(d => new SelectListItem
                        {
                            Value = d.IdDepartamento.ToString(),
                            Text = d.Departamento1
                        })
                        .OrderBy(item => item.Text)
                        .ToList();
                }
            }

            var seleccionado = listaDepartamentos.ToList();
            int departamentoId = idDepartamento;

            if (departamentoId == 0 && seleccionado.Count > 0)
            {
                departamentoId = int.Parse(seleccionado[0].Value);
            }

            var departamento = await _departamentoService.ObtenerDepartamento(departamentoId);

            var modelo = new VMDocumentosDepartamento
            {
                ListaDepartamentos = seleccionado,
                DepartamentoSeleccionado = departamentoId,
                EsAdministrador = esAdmin,
                ReglasOperacion = departamento?.ReglasOperacion,
                LineamientosOperacion = departamento?.LineamientosOperacion,
                ArbolProblemasObjetivos = departamento?.ArbolProblemasObjetivos,
                MetodologiaPadron = departamento?.MetodologiaPadron,
                Diagnostico = departamento?.Diagnostico,
                Justificacion = departamento?.Justificacion
            };

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarDocumentos(DepartamentoDocumentosDTO modelo)
        {
            try
            {
                var departamento = await _departamentoService.ObtenerDepartamento(modelo.IdDepartamento);
                if (departamento == null)
                {
                    return Json(new { success = false, mensaje = "Departamento no encontrado." });
                }

                // Validar obligatorios: Reglas de Operación y Lineamientos de Operación
                bool tieneReglas = departamento.ReglasOperacion != null
                    || (modelo.InputReglasOperacion != null && modelo.InputReglasOperacion.Length > 0);
                bool tieneLineamientos = departamento.LineamientosOperacion != null
                    || (modelo.InputLineamientosOperacion != null && modelo.InputLineamientosOperacion.Length > 0);

                bool faltaObligatorio = !tieneReglas || !tieneLineamientos;

                if (faltaObligatorio && string.IsNullOrWhiteSpace(modelo.Justificacion))
                {
                    return Json(new
                    {
                        success = false,
                        mensaje = "Debe cargar el documento de 'Reglas de Operación' y 'Lineamientos de Operación', o en su defecto proporcionar una justificación."
                    });
                }

                // Validar tipos de archivo y tamaño
                var archivos = new Dictionary<string, IFormFile>
                {
                    { "reglas", modelo.InputReglasOperacion },
                    { "lineamientos", modelo.InputLineamientosOperacion },
                    { "arbol", modelo.InputArbolProblemasObjetivos },
                    { "metodologia", modelo.InputMetodologiaPadron },
                    { "diagnostico", modelo.InputDiagnostico }
                };

                foreach (var kv in archivos)
                {
                    if (kv.Value == null || kv.Value.Length == 0) continue;

                    string ext = Path.GetExtension(kv.Value.FileName).ToLowerInvariant();
                    if (!ExtensionesPermitidas.Contains(ext))
                    {
                        return Json(new { success = false, mensaje = "Solo se permiten archivos PDF o XLSX." });
                    }
                    if (kv.Value.Length > TAMANO_MAXIMO_BYTES)
                    {
                        return Json(new { success = false, mensaje = "El archivo no puede superar los 50 MB." });
                    }
                }

                // Guardar archivos físicos y construir rutas
                string idDepto = modelo.IdDepartamento.ToString();

                if (modelo.InputReglasOperacion != null && modelo.InputReglasOperacion.Length > 0)
                    modelo.RutaReglasOperacion = await GuardarArchivo(modelo.InputReglasOperacion, idDepto, "reglas", departamento.ReglasOperacion);
                else
                    modelo.RutaReglasOperacion = departamento.ReglasOperacion;

                if (modelo.InputLineamientosOperacion != null && modelo.InputLineamientosOperacion.Length > 0)
                    modelo.RutaLineamientosOperacion = await GuardarArchivo(modelo.InputLineamientosOperacion, idDepto, "lineamientos", departamento.LineamientosOperacion);
                else
                    modelo.RutaLineamientosOperacion = departamento.LineamientosOperacion;

                if (modelo.InputArbolProblemasObjetivos != null && modelo.InputArbolProblemasObjetivos.Length > 0)
                    modelo.RutaArbolProblemasObjetivos = await GuardarArchivo(modelo.InputArbolProblemasObjetivos, idDepto, "arbol", departamento.ArbolProblemasObjetivos);
                else
                    modelo.RutaArbolProblemasObjetivos = departamento.ArbolProblemasObjetivos;

                if (modelo.InputMetodologiaPadron != null && modelo.InputMetodologiaPadron.Length > 0)
                    modelo.RutaMetodologiaPadron = await GuardarArchivo(modelo.InputMetodologiaPadron, idDepto, "metodologia", departamento.MetodologiaPadron);
                else
                    modelo.RutaMetodologiaPadron = departamento.MetodologiaPadron;

                if (modelo.InputDiagnostico != null && modelo.InputDiagnostico.Length > 0)
                    modelo.RutaDiagnostico = await GuardarArchivo(modelo.InputDiagnostico, idDepto, "diagnostico", departamento.Diagnostico);
                else
                    modelo.RutaDiagnostico = departamento.Diagnostico;

                if (string.IsNullOrWhiteSpace(modelo.Justificacion))
                {
                    modelo.Justificacion = departamento.Justificacion;
                }

                bool resultado = await _departamentoService.GuardarDocumentos(modelo);

                if (resultado)
                {
                    return Json(new { success = true, mensaje = "Documentos guardados exitosamente." });
                }
                else
                {
                    return Json(new { success = false, mensaje = "No se pudieron guardar los documentos." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = $"Error al guardar: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EliminarDocumento(int idDepartamento, string tipoDocumento)
        {
            try
            {
                string rutaEliminada = await _departamentoService.EliminarDocumento(idDepartamento, tipoDocumento);

                if (rutaEliminada == null)
                {
                    return Json(new { success = false, mensaje = "No se pudo eliminar el documento." });
                }

                // Eliminar archivo físico
                string rutaFisica = Path.Combine(_hostEnvironment.WebRootPath, rutaEliminada.TrimStart('/'));
                if (System.IO.File.Exists(rutaFisica))
                {
                    System.IO.File.Delete(rutaFisica);
                }

                return Json(new { success = true, mensaje = "Documento eliminado exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = $"Error al eliminar: {ex.Message}" });
            }
        }

        private async Task<string> GuardarArchivo(IFormFile archivo, string idDepartamento, string tipo, string rutaExistente)
        {
            string folderPath = Path.Combine(_hostEnvironment.WebRootPath, BASE_FOLDER_DOCUMENTOS, idDepartamento, tipo);
            Directory.CreateDirectory(folderPath);

            // Eliminar archivo anterior si existía
            if (!string.IsNullOrEmpty(rutaExistente))
            {
                string rutaFisicaAnterior = Path.Combine(_hostEnvironment.WebRootPath, rutaExistente.TrimStart('/'));
                if (System.IO.File.Exists(rutaFisicaAnterior))
                {
                    System.IO.File.Delete(rutaFisicaAnterior);
                }
            }

            string originalFileName = Path.GetFileName(archivo.FileName);
            string rutaFisica = Path.Combine(folderPath, originalFileName);

            using (var fileStream = new FileStream(rutaFisica, FileMode.Create))
            {
                await archivo.CopyToAsync(fileStream);
            }

            return $"/{BASE_FOLDER_DOCUMENTOS}/{idDepartamento}/{tipo}/{originalFileName}";
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
