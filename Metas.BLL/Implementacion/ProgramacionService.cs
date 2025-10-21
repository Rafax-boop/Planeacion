using Metas.BLL.DTO;
using Metas.BLL.Interfaces;
using Metas.DAL.DBContext;
using Metas.DAL.Interfaces;
using Metas.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metas.BLL.Implementacion
{
    public class ProgramacionService : IProgramacionService
    {
        private readonly IGenericRepository<LlenadoInterno> _repositorioLlenadoInterno;
        private readonly IGenericRepository<Departamento> _repositoryDepartamento;
        private readonly IGenericRepository<Programacion> _repositorioProgramacion;
        private readonly IGenericRepository<ServiciosMunicipio> _repositorioServicios;
        private readonly IGenericRepository<PersonasMunicipio> _repositorioPersonas;
        private readonly MetasContext _context;

        public ProgramacionService(IGenericRepository<LlenadoInterno> repositorioLlenadoInterno,
            IGenericRepository<Departamento> repositoryDepartamento,
            IGenericRepository<Programacion> repositorioProgramacion,
            IGenericRepository<ServiciosMunicipio> repositorioServicios,
            IGenericRepository<PersonasMunicipio> repositorioPersonas,
            MetasContext context)
        {
            _repositorioLlenadoInterno = repositorioLlenadoInterno;
            _repositoryDepartamento = repositoryDepartamento;
            _repositorioProgramacion = repositorioProgramacion;
            _repositorioServicios = repositorioServicios;
            _repositorioPersonas = repositorioPersonas;
            _context = context;
        }

        public async Task<bool> GuardarProgramacion(ProgramacionDTO modelo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var componenteTexto = await _context.PpCompuestos
                .Where(x => x.IdPp == int.Parse(modelo.NComponente))
                .Select(x => x.ComponenteCompuesto)
                .FirstOrDefaultAsync();

                var idPp = await _context.Pps // o _context.PPS según tu DbContext
                .Where(x => x.Clave == modelo.Pp) // Ajusta según el nombre de la columna
                .Select(x => x.IdPp)
                .FirstOrDefaultAsync();
                var programacion = new Programacion
                {
                    FechaSolicitud = DateOnly.FromDateTime(DateTime.Now),
                    Area = modelo.Area,
                    CorreoElectro = modelo.CorreoContacto,
                    Pp = modelo.Pp,
                    NComponente = componenteTexto,
                    NActividad = modelo.NActividad,
                    Justificacion = modelo.Justificacion,
                    DescripcionDocumento = modelo.DescripcionDocumento,
                    RecursoFederal = modelo.RecursoFederal,
                    RecursoEstatal = modelo.RecursoEstatal,
                    ProgramaSocial = modelo.ProgramaSocial,
                    DescripcionActividad = modelo.DescripcionActividad,
                    NombreIndicador = modelo.NombreIndicador,
                    DefinicionIndicador = modelo.DefinicionIndicador,
                    UnidadMedida = modelo.UnidadMedida,
                    MediosVerificac = modelo.MediosVerificacion,
                    SerieInfo = modelo.SerieInformacionDesde,
                    SerieInfo2 = modelo.SerieInformacionHasta,
                    FuenteInfo = modelo.FuenteInformacion,
                    IntervienenDelegaciones = modelo.IntervienenDelegaciones,
                    IntervienenDelegacionesManera = modelo.IntervienenDelegacionesManera,

                    Anos = modelo.AnoBase,
                    Valor = modelo.PorcentajeBase,
                    BienServicio = modelo.ServicioBase,
                    NoPersonas = modelo.PersonasBase,

                    Anos2 = modelo.AnoMeta,
                    Valor2 = modelo.PorcentajeMeta,
                    BienServicio2 = modelo.ServicioMeta,
                    NoPersonas2 = modelo.PersonasMeta,

                    Servicio1 = modelo.PrimerServicio,
                    Servicio2 = modelo.SegundoServicio,
                    Servicio3 = modelo.TercerServicio,
                    Servicio4 = modelo.CuartoServicio,

                    Personas1 = modelo.PrimerPersona,
                    Personas2 = modelo.SegundoPersona,
                    Personas3 = modelo.TercerPersona,
                    Personas4 = modelo.CuartoPersona,

                    Mes1 = modelo.MesesServicios[0],
                    Mes2 = modelo.MesesServicios[1],
                    Mes3 = modelo.MesesServicios[2],
                    Mes4 = modelo.MesesServicios[3],
                    Mes5 = modelo.MesesServicios[4],
                    Mes6 = modelo.MesesServicios[5],
                    Mes7 = modelo.MesesServicios[6],
                    Mes8 = modelo.MesesServicios[7],
                    Mes9 = modelo.MesesServicios[8],
                    Mes10 = modelo.MesesServicios[9],
                    Mes11 = modelo.MesesServicios[10],
                    Mes12 = modelo.MesesServicios[11],

                    Mes13 = modelo.MesesPersonas[0],
                    Mes14 = modelo.MesesPersonas[1],
                    Mes15 = modelo.MesesPersonas[2],
                    Mes16 = modelo.MesesPersonas[3],
                    Mes17 = modelo.MesesPersonas[4],
                    Mes18 = modelo.MesesPersonas[5],
                    Mes111 = modelo.MesesPersonas[6],
                    Mes112 = modelo.MesesPersonas[7],
                    Mes1111 = modelo.MesesPersonas[8],
                    Mes110 = modelo.MesesPersonas[9],
                    Mes121 = modelo.MesesPersonas[10],
                    Mes19 = modelo.MesesPersonas[11],

                    Actividad1 = modelo.Acciones.Count > 0 ? modelo.Acciones[0].Descripcion : null,
                    Frecuencia1 = modelo.Acciones.Count > 0 ? modelo.Acciones[0].Frecuencia : null,
                    FechaProgramacion1 = modelo.Acciones.Count > 0 ? modelo.Acciones[0].FechaInicio : null,

                    Actividad2 = modelo.Acciones.Count > 1 ? modelo.Acciones[1].Descripcion : null,
                    Frecuencia2 = modelo.Acciones.Count > 1 ? modelo.Acciones[1].Frecuencia : null,
                    FechaProgramacion2 = modelo.Acciones.Count > 1 ? modelo.Acciones[1].FechaInicio : null,

                    Actividad3 = modelo.Acciones.Count > 2 ? modelo.Acciones[2].Descripcion : null,
                    Frecuencia3 = modelo.Acciones.Count > 2 ? modelo.Acciones[2].Frecuencia : null,
                    FechaProgramacion3 = modelo.Acciones.Count > 2 ? modelo.Acciones[2].FechaInicio : null,

                    Actividad4 = modelo.Acciones.Count > 3 ? modelo.Acciones[3].Descripcion : null,
                    Frecuencia4 = modelo.Acciones.Count > 3 ? modelo.Acciones[3].Frecuencia : null,
                    FechaProgramacion4 = modelo.Acciones.Count > 3 ? modelo.Acciones[3].FechaInicio : null,

                    Actividad5 = modelo.Acciones.Count > 4 ? modelo.Acciones[4].Descripcion : null,
                    Frecuencia5 = modelo.Acciones.Count > 4 ? modelo.Acciones[4].Frecuencia : null,
                    FechaProgramacion5 = modelo.Acciones.Count > 4 ? modelo.Acciones[4].FechaInicio : null,

                    ElaboraNombre = modelo.ElaboraNombre,
                    ElaboroCargo = modelo.ElaboroCargo,
                    ValidoNombre = modelo.RevisionNombre,
                    ValidoCargo = modelo.RevisionCargo,
                    AutorizoNombre = modelo.AutorizacionNombre,
                    AutorizoCargo = modelo.AutorizacionCargo,

                    IdEstatus = 1,
                    Acumulable = modelo.SelectAcumulable,
                    Totalanos = modelo.TotalAnos,
                    Totalanos2 = modelo.TotalAnos2,
                    Beneficiarios = modelo.Beneficiarios
                };

                var programacionGuardada = await _repositorioProgramacion.Crear(programacion);
                int idProgramacion = programacionGuardada.IdRegistro;

                var llenadoInterno = new LlenadoInterno
                {
                    Pp = modelo.Pp,
                    Componente = int.Parse(modelo.NComponente),
                    Actividad = modelo.NActividad,
                    DescripcionActividad = modelo.DescripcionActividad,
                    Area = modelo.Area,
                    Departamento = modelo.Departamento,
                    ProgramaSocial = modelo.ProgramaSocial,
                    Ano = modelo.AnoMeta,
                    TotalEnero = modelo.MesesServicios[0],
                    TotalFebrero = modelo.MesesServicios[1],
                    TotalMarzo = modelo.MesesServicios[2],
                    TotalAbril = modelo.MesesServicios[3],
                    TotalMayo = modelo.MesesServicios[4],
                    TotalJunio = modelo.MesesServicios[5],
                    TotalJulio = modelo.MesesServicios[6],
                    TotalAgosto = modelo.MesesServicios[7],
                    TotalSeptiembre = modelo.MesesServicios[8],
                    TotalOctubre = modelo.MesesServicios[9],
                    TotalNoviembre = modelo.MesesServicios[10],
                    TotalDiciembre = modelo.MesesServicios[11],
                    UnidadMedida = modelo.UnidadMedida,
                    TotalProgramado = modelo.PrimerServicio + modelo.SegundoServicio + modelo.TercerServicio + modelo.CuartoServicio,
                    TotalPersona = modelo.PrimerPersona + modelo.SegundoPersona + modelo.TercerPersona + modelo.CuartoPersona,
                    EneroPersona = modelo.MesesPersonas[0],
                    FebreroPersona = modelo.MesesPersonas[1],
                    MarzoPersona = modelo.MesesPersonas[2],
                    AbrilPersona = modelo.MesesPersonas[3],
                    MayoPersona = modelo.MesesPersonas[4],
                    JunioPersona = modelo.MesesPersonas[5],
                    JulioPersona = modelo.MesesPersonas[6],
                    AgostoPersona = modelo.MesesPersonas[7],
                    SeptiembrePersona = modelo.MesesPersonas[8],
                    OctubrePersona = modelo.MesesPersonas[9],
                    NoviembrePersona = modelo.MesesPersonas[10],
                    DiciembrePersona = modelo.MesesPersonas[11],
                    Idpp = idPp,
                    NombreRealizo = modelo.ElaboraNombre,
                    CargoRealizo = modelo.ElaboroCargo
                };

                await _repositorioLlenadoInterno.Crear(llenadoInterno);

                programacionGuardada.IdLlenado = llenadoInterno.IdProceso;
                await _repositorioProgramacion.Editar(programacionGuardada);

                foreach (var municipio in modelo.MunicipiosServicios)
                {
                    var servicioMunicipio = new ServiciosMunicipio
                    {
                        IdLlenado = idProgramacion,
                        IdMunicipio = municipio.IdMunicipio,
                        NumeroBien = municipio.Cantidad
                    };
                    await _repositorioServicios.Crear(servicioMunicipio);
                }

                foreach (var municipio in modelo.MunicipiosPersonas)
                {
                    var personaMunicipio = new PersonasMunicipio
                    {
                        IdLlenado = idProgramacion,
                        IdMunicipio = municipio.IdMunicipio,
                        NumeroBien = municipio.Cantidad
                    };
                    await _repositorioPersonas.Crear(personaMunicipio);
                }

                // Si todo salió bien, confirmar la transacción
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Si algo falla, hacer rollback
                await transaction.RollbackAsync();

                Console.WriteLine($"Error al guardar programación: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<List<LlenadoInterno>> ObtenerDatosProgramacion(int anoFiscal, int departamentoId)
        {
            try
            {
                // Primero obtener el nombre del departamento
                var departamento = await _repositoryDepartamento.Obtener(x => x.IdDepartamento == departamentoId);

                if (departamento == null)
                    return new List<LlenadoInterno>();

                string nombreDepartamento = departamento.Departamento1;

                var query = await _repositorioLlenadoInterno.Consultar(x =>
                    x.Ano == anoFiscal &&
                    x.Departamento == nombreDepartamento
                );

                var resultado = await query
                    .Include(x => x.Programacions)
                    .ToListAsync();

                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> EliminarProgramacion(int idLlenado)
        {
            try
            {
                var programacionEntidad = await _repositorioProgramacion.Obtener(p => p.IdLlenado.HasValue && p.IdLlenado.Value == idLlenado);

                if (programacionEntidad == null)
                {
                    return false;
                }

                int idProgramacion = programacionEntidad.IdRegistro;

                var personasMunicipios = await _repositorioPersonas.Consultar(pm => pm.IdLlenado == idProgramacion);
                foreach (var pm in personasMunicipios)
                {
                    await _repositorioPersonas.Eliminar(pm);
                }

                var serviciosMunicipios = await _repositorioServicios.Consultar(sm => sm.IdLlenado == idProgramacion);
                foreach (var sm in serviciosMunicipios)
                {
                    await _repositorioServicios.Eliminar(sm);
                }
                
                await _repositorioProgramacion.Eliminar(programacionEntidad);

                var llenadoInternoEntidad = await _repositorioLlenadoInterno.Obtener(l => l.IdProceso == idLlenado);
                if (llenadoInternoEntidad != null)
                {
                    await _repositorioLlenadoInterno.Eliminar(llenadoInternoEntidad);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
