using Metas.AplicacionWeb.Models.ViewModels;
using Metas.BLL.Implementacion;
using Metas.BLL.Interfaces;
using Metas.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Metas.AplicacionWeb.Controllers
{
    [Authorize]
    public class MonitoreoController : Controller
    {
        private readonly IDepartamentoService _departamentoService;
        private readonly IProgramacionService _programacionService;
        private readonly IFechasService _fechasService;
        public MonitoreoController(IDepartamentoService departamentoService, IProgramacionService programacionService, IFechasService fechasService)
        {
            _departamentoService = departamentoService;
            _programacionService = programacionService;
            _fechasService = fechasService;
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

        [HttpGet]
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
                    FechasCaptura = fechasDelAno
                }).ToList();

                return Json(new { success = true, datos = resultado, esAdmin = esAdmin });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = $"Error al obtener datos: {ex.Message}" });
            }
        }

        public async Task<IActionResult> ActualizacionMeses(int idProceso, int mes)
        {
            var fechasCaptura = await _fechasService.Lista();

            var fechaRegistro = fechasCaptura
                .FirstOrDefault(f => f.IdFechaCaptura == mes);

            var datosInternos = await _programacionService.ObtenerporId(idProceso);

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

            var modelo = new VMDatosInternos
            {
                Mes = fechaRegistro.Mes,
                FechaFin = fechaRegistro.FechaFin,
                Total = totalMes,
                pp = datosInternos.Pp,
                Componente = datosInternos.Componente,
                Actividad = datosInternos.Actividad,
                UnidadMedida = datosInternos.UnidadMedida,
                ProgramaSocial = datosInternos.ProgramaSocial,
                Area = datosInternos.Area,
                Departamento = datosInternos.Departamento,
                DescripcionActividad = datosInternos.DescripcionActividad
            };

            return View(modelo);
        }
    }
}
