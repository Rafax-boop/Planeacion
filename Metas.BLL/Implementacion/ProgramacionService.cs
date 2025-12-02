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
        private readonly IGenericRepository<LlenadoExterno> _repositorioLlenadoExterno;
        private readonly IGenericRepository<Departamento> _repositoryDepartamento;
        private readonly IGenericRepository<Programacion> _repositorioProgramacion;
        private readonly IGenericRepository<ServiciosMunicipio> _repositorioServicios;
        private readonly IGenericRepository<PersonasMunicipio> _repositorioPersonas;
        private readonly IGenericRepository<Comentario> _repositorioComentario;
        private readonly MetasContext _context;

        public ProgramacionService(IGenericRepository<LlenadoInterno> repositorioLlenadoInterno,
            IGenericRepository<Departamento> repositoryDepartamento,
            IGenericRepository<LlenadoExterno> repositorioLlenadoExterno,
            IGenericRepository<Programacion> repositorioProgramacion,
            IGenericRepository<ServiciosMunicipio> repositorioServicios,
            IGenericRepository<PersonasMunicipio> repositorioPersonas,
            IGenericRepository<Comentario> repositorioComentario,
            MetasContext context)
        {
            _repositorioLlenadoInterno = repositorioLlenadoInterno;
            _repositoryDepartamento = repositoryDepartamento;
            _repositorioProgramacion = repositorioProgramacion;
            _repositorioServicios = repositorioServicios;
            _repositorioPersonas = repositorioPersonas;
            _repositorioLlenadoExterno = repositorioLlenadoExterno;
            _repositorioComentario = repositorioComentario;
            _context = context;
        }

        public async Task<bool> GuardarProgramacion(ProgramacionDTO modelo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
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
                    NComponente = modelo.NComponente,
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
                    Componente = modelo.Componente,
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
                    Idpp = idPp
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
                        .ThenInclude(p => p.IdEstatusNavigation)
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
                var programacionEntidad = await _repositorioProgramacion.Obtener(
                    p => p.IdLlenado.HasValue && p.IdLlenado.Value == idLlenado);

                if (programacionEntidad == null)
                {
                    return false;
                }

                int idProgramacion = programacionEntidad.IdRegistro;

                // ORDEN CORRECTO: De tablas hijas a tablas padres

                // 1. Eliminar Comentarios (tabla hija de Programacion)
                var comentarios = (await _repositorioComentario.Consultar(
                    c => c.IdProgramacion == idProgramacion)).ToList();
                foreach (var item in comentarios)
                {
                    await _repositorioComentario.Eliminar(item);
                }

                // 2. Eliminar PersonasMunicipio
                var personasMunicipios = (await _repositorioPersonas.Consultar(
                    pm => pm.IdLlenado == idProgramacion)).ToList();
                foreach (var item in personasMunicipios)
                {
                    await _repositorioPersonas.Eliminar(item);
                }

                // 3. Eliminar ServiciosMunicipios
                var serviciosMunicipios = (await _repositorioServicios.Consultar(
                    sm => sm.IdLlenado == idProgramacion)).ToList();
                foreach (var item in serviciosMunicipios)
                {
                    await _repositorioServicios.Eliminar(item);
                }

                // 4. Eliminar LlenadoExterno
                var llenadoExternoEntidad = (await _repositorioLlenadoExterno.Consultar(
                    sm => sm.IdProceso == idLlenado)).ToList();
                foreach (var item in llenadoExternoEntidad)
                {
                    await _repositorioLlenadoExterno.Eliminar(item);
                }

                // 5. Eliminar LlenadoInterno
                var llenadoInternoEntidad = await _repositorioLlenadoInterno.Obtener(
                    l => l.IdProceso == idLlenado);
                if (llenadoInternoEntidad != null)
                {
                    await _repositorioLlenadoInterno.Eliminar(llenadoInternoEntidad);
                }

                // 6. FINALMENTE: Eliminar Programacion (tabla padre)
                await _repositorioProgramacion.Eliminar(programacionEntidad);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar programación {idLlenado}: {ex.Message}");
                return false; // O considera lanzar la excepción: throw;
            }
        }

        public async Task<ProgramacionDTO> ObtenerDatosCompletos(int idLlenado)
        {
            try
            {
                // 1. Obtener la programación principal
                var programacionEntidad = await _repositorioProgramacion.Obtener(
                    p => p.IdLlenado.HasValue && p.IdLlenado.Value == idLlenado);

                if (programacionEntidad == null)
                    return null;

                int idProgramacion = programacionEntidad.IdRegistro;

                // 2. Obtener PersonasMunicipio relacionadas
                var personasMunicipios = (await _repositorioPersonas.Consultar(
                    pm => pm.IdLlenado == idProgramacion)).ToList();

                // 3. Obtener ServiciosMunicipio relacionados
                var serviciosMunicipios = (await _repositorioServicios.Consultar(
                    sm => sm.IdLlenado == idProgramacion)).ToList();

                // 4. Obtener LlenadoInterno
                var llenadoInterno = await _repositorioLlenadoInterno.Obtener(
                    l => l.IdProceso == idLlenado);

                // 5. Mapear a DTO
                var dto = new ProgramacionDTO
                {
                    // Datos generales
                    Id = idProgramacion,
                    Area = programacionEntidad.Area,
                    Departamento = llenadoInterno?.Departamento,
                    CorreoContacto = programacionEntidad.CorreoElectro,
                    Pp = llenadoInterno?.Pp,
                    NComponente = programacionEntidad.NComponente,
                    NActividad = programacionEntidad.NActividad ?? 0,
                    Justificacion = programacionEntidad.Justificacion,
                    DescripcionDocumento = programacionEntidad.DescripcionDocumento,
                    RecursoFederal = programacionEntidad.RecursoFederal,
                    RecursoEstatal = programacionEntidad.RecursoEstatal,
                    ProgramaSocial = programacionEntidad.ProgramaSocial,
                    DescripcionActividad = programacionEntidad.DescripcionActividad,
                    NombreIndicador = programacionEntidad.NombreIndicador,
                    DefinicionIndicador = programacionEntidad.DefinicionIndicador,
                    UnidadMedida = programacionEntidad.UnidadMedida,
                    MediosVerificacion = programacionEntidad.MediosVerificac,
                    SerieInformacionDesde = programacionEntidad.SerieInfo,
                    SerieInformacionHasta = programacionEntidad.SerieInfo2,
                    FuenteInformacion = programacionEntidad.FuenteInfo,
                    IntervienenDelegaciones = programacionEntidad.IntervienenDelegaciones,
                    IntervienenDelegacionesManera = programacionEntidad.IntervienenDelegacionesManera,
                    SelectAcumulable = programacionEntidad.Acumulable,
                    Estatus = programacionEntidad.IdEstatus ?? 0,
                    Beneficiarios = programacionEntidad.Beneficiarios,

                    // Línea Base
                    AnoBase = programacionEntidad.Anos ?? 0,
                    PorcentajeBase = programacionEntidad.Valor ?? 0,
                    ServicioBase = programacionEntidad.BienServicio ?? 0,
                    PersonasBase = programacionEntidad.NoPersonas ?? 0,

                    // Meta Anual
                    AnoMeta = programacionEntidad.Anos2 ?? 0,
                    PorcentajeMeta = programacionEntidad.Valor2 ?? 0,
                    ServicioMeta = programacionEntidad.BienServicio2 ?? 0,
                    PersonasMeta = programacionEntidad.NoPersonas2 ?? 0,

                    // Trimestres Servicios
                    PrimerServicio = programacionEntidad.Servicio1 ?? 0,
                    SegundoServicio = programacionEntidad.Servicio2 ?? 0,
                    TercerServicio = programacionEntidad.Servicio3 ?? 0,
                    CuartoServicio = programacionEntidad.Servicio4 ?? 0,

                    // Trimestres Personas
                    PrimerPersona = programacionEntidad.Personas1 ?? 0,
                    SegundoPersona = programacionEntidad.Personas2 ?? 0,
                    TercerPersona = programacionEntidad.Personas3 ?? 0,
                    CuartoPersona = programacionEntidad.Personas4 ?? 0,

                    // Meses Servicios (12 meses)
                    MesesServicios = new List<int>
            {
                programacionEntidad.Mes1 ?? 0,
                programacionEntidad.Mes2 ?? 0,
                programacionEntidad.Mes3 ?? 0,
                programacionEntidad.Mes4 ?? 0,
                programacionEntidad.Mes5 ?? 0,
                programacionEntidad.Mes6 ?? 0,
                programacionEntidad.Mes7 ?? 0,
                programacionEntidad.Mes8 ?? 0,
                programacionEntidad.Mes9 ?? 0,
                programacionEntidad.Mes10 ?? 0,
                programacionEntidad.Mes11 ?? 0,
                programacionEntidad.Mes12 ?? 0
            },
                    TotalAnos = programacionEntidad.Totalanos ?? 0,

                    // Meses Personas (12 meses)
                    MesesPersonas = new List<int>
            {
                programacionEntidad.Mes111 ?? 0,
                programacionEntidad.Mes121 ?? 0,
                programacionEntidad.Mes13 ?? 0,
                programacionEntidad.Mes14 ?? 0,
                programacionEntidad.Mes15 ?? 0,
                programacionEntidad.Mes16 ?? 0,
                programacionEntidad.Mes17 ?? 0,
                programacionEntidad.Mes18 ?? 0,
                programacionEntidad.Mes19 ?? 0,
                programacionEntidad.Mes110 ?? 0,
                programacionEntidad.Mes1111 ?? 0,
                programacionEntidad.Mes112 ?? 0
            },
                    TotalAnos2 = programacionEntidad.Totalanos2 ?? 0,

                    // Municipios Servicios
                    MunicipiosServicios = serviciosMunicipios.Select(sm => new DTOMunicipioAgregado
                    {
                        IdMunicipio = sm.IdMunicipio ?? 0,
                        Cantidad = sm.NumeroBien ?? 0
                    }).ToList(),

                    // Municipios Personas
                    MunicipiosPersonas = personasMunicipios.Select(pm => new DTOMunicipioAgregado
                    {
                        IdMunicipio = pm.IdMunicipio ?? 0,
                        Cantidad = pm.NumeroBien ?? 0
                    }).ToList(),

                    // Acciones (1-5)
                    Acciones = new List<DTOAccion>
            {
                new DTOAccion
                {
                    Descripcion = programacionEntidad.Actividad1,
                    Frecuencia = programacionEntidad.Frecuencia1,
                    FechaInicio = programacionEntidad.FechaProgramacion1
                },
                new DTOAccion
                {
                    Descripcion = programacionEntidad.Actividad2,
                    Frecuencia = programacionEntidad.Frecuencia2,
                    FechaInicio = programacionEntidad.FechaProgramacion2
                },
                new DTOAccion
                {
                    Descripcion = programacionEntidad.Actividad3,
                    Frecuencia = programacionEntidad.Frecuencia3,
                    FechaInicio = programacionEntidad.FechaProgramacion3
                },
                new DTOAccion
                {
                    Descripcion = programacionEntidad.Actividad4,
                    Frecuencia = programacionEntidad.Frecuencia4,
                    FechaInicio = programacionEntidad.FechaProgramacion4
                },
                new DTOAccion
                {
                    Descripcion = programacionEntidad.Actividad5,
                    Frecuencia = programacionEntidad.Frecuencia5,
                    FechaInicio = programacionEntidad.FechaProgramacion5
                }
            }.Where(a => !string.IsNullOrEmpty(a.Descripcion)).ToList(), // Solo incluir acciones con descripción

                    // Firmas
                    ElaboraNombre = programacionEntidad.ElaboraNombre,
                    ElaboroCargo = programacionEntidad.ElaboroCargo,
                    RevisionNombre = programacionEntidad.ValidoNombre,
                    RevisionCargo = programacionEntidad.ValidoCargo,
                    AutorizacionNombre = programacionEntidad.AutorizoNombre,
                    AutorizacionCargo = programacionEntidad.AutorizoCargo
                };

                return dto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerDatosCompletos: {ex.Message}");
                return null;
            }
        }

        public async Task<LlenadoInterno> ObtenerporId(int idLlenado)
        {
            try
            {
                var llenadoInterno = await _repositorioLlenadoInterno.Obtener(
                    l => l.IdProceso == idLlenado);
                return llenadoInterno;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> GuardarComentarios(List<ComentarioDTO> comentarios)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (comentarios == null || !comentarios.Any())
                {
                    return false;
                }

                var idProgramacion = comentarios.First().IdProgramacion;

                // ✅ NUEVA LÓGICA: Si NO hay comentarios con texto → Estatus 1 (En Revisión)
                //                 Si SÍ hay comentarios con texto → Estatus 2 (Comentarios)
                bool tieneComentarios = comentarios.Any(c => !string.IsNullOrWhiteSpace(c.Texto));

                var programacion = await _repositorioProgramacion.Obtener(p => p.IdRegistro == idProgramacion);
                if (programacion != null)
                {
                    programacion.IdEstatus = tieneComentarios ? 2 : 1; // 2=Comentarios, 1=En Revisión
                    await _repositorioProgramacion.Editar(programacion);
                }

                // Resto del código para guardar comentarios...
                var comentarioExistente = await _context.Comentarios
                    .FirstOrDefaultAsync(c => c.IdProgramacion == idProgramacion);

                if (comentarioExistente == null)
                {
                    comentarioExistente = new Comentario { IdProgramacion = idProgramacion };
                    _context.Comentarios.Add(comentarioExistente);
                    await _context.SaveChangesAsync();
                }

                // Asignar comentarios a propiedades
                foreach (var com in comentarios)
                {
                    var nombrePropiedad = $"Comentario{com.ComentarioId}";
                    var propiedad = typeof(Comentario).GetProperty(nombrePropiedad);

                    if (propiedad != null && propiedad.CanWrite)
                    {
                        propiedad.SetValue(comentarioExistente, com.Texto);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;

            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<Comentario> ObtenerComentariosPorProgramacion(int idProgramacion)
        {
            try
            {
                // Buscar comentarios existentes para esta programación
                var comentarios = await _context.Comentarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.IdProgramacion == idProgramacion);

                // Si no existen, retornar un objeto vacío con el ID
                return comentarios ?? new Comentario { IdProgramacion = idProgramacion };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error obteniendo comentarios: {ex.Message}");
                return new Comentario { IdProgramacion = idProgramacion };
            }
        }

        public async Task<bool> ActualizarEstatusProgramacion(int idProgramacion, int nuevoEstatus)
        {
            try
            {
                // Buscar la programación por ID
                var programacion = await _repositorioProgramacion.Obtener(p => p.IdRegistro == idProgramacion);

                if (programacion != null)
                {
                    // Actualizar el estatus
                    programacion.IdEstatus = nuevoEstatus;
                    await _repositorioProgramacion.Editar(programacion);
                    return true;
                }

                Console.WriteLine($"⚠️ No se encontró programación con ID: {idProgramacion}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ActualizarProgramacion(ProgramacionDTO modelo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Buscar la programación existente
                var programacionExistente = await _repositorioProgramacion.Obtener(p => p.IdRegistro == modelo.Id);

                if (programacionExistente == null)
                {
                    return false;
                }

                // 2. Actualizar campos de Programacion
                programacionExistente.CorreoElectro = modelo.CorreoContacto;
                programacionExistente.Pp = modelo.Pp;
                programacionExistente.NComponente = modelo.NComponente;
                programacionExistente.NActividad = modelo.NActividad;
                programacionExistente.Justificacion = modelo.Justificacion;
                programacionExistente.DescripcionDocumento = modelo.DescripcionDocumento;
                programacionExistente.RecursoFederal = modelo.RecursoFederal;
                programacionExistente.RecursoEstatal = modelo.RecursoEstatal;
                programacionExistente.ProgramaSocial = modelo.ProgramaSocial;
                programacionExistente.DescripcionActividad = modelo.DescripcionActividad;
                programacionExistente.NombreIndicador = modelo.NombreIndicador;
                programacionExistente.DefinicionIndicador = modelo.DefinicionIndicador;
                programacionExistente.UnidadMedida = modelo.UnidadMedida;
                programacionExistente.MediosVerificac = modelo.MediosVerificacion;
                programacionExistente.SerieInfo = modelo.SerieInformacionDesde;
                programacionExistente.SerieInfo2 = modelo.SerieInformacionHasta;
                programacionExistente.FuenteInfo = modelo.FuenteInformacion;
                programacionExistente.IntervienenDelegaciones = modelo.IntervienenDelegaciones;
                programacionExistente.IntervienenDelegacionesManera = modelo.IntervienenDelegacionesManera;

                // Línea Base
                programacionExistente.Anos = modelo.AnoBase;
                programacionExistente.Valor = modelo.PorcentajeBase;
                programacionExistente.BienServicio = modelo.ServicioBase;
                programacionExistente.NoPersonas = modelo.PersonasBase;

                // Meta Anual
                programacionExistente.Anos2 = modelo.AnoMeta;
                programacionExistente.Valor2 = modelo.PorcentajeMeta;
                programacionExistente.BienServicio2 = modelo.ServicioMeta;
                programacionExistente.NoPersonas2 = modelo.PersonasMeta;

                // Trimestres
                programacionExistente.Servicio1 = modelo.PrimerServicio;
                programacionExistente.Servicio2 = modelo.SegundoServicio;
                programacionExistente.Servicio3 = modelo.TercerServicio;
                programacionExistente.Servicio4 = modelo.CuartoServicio;

                programacionExistente.Personas1 = modelo.PrimerPersona;
                programacionExistente.Personas2 = modelo.SegundoPersona;
                programacionExistente.Personas3 = modelo.TercerPersona;
                programacionExistente.Personas4 = modelo.CuartoPersona;

                // Meses Servicios
                programacionExistente.Mes1 = modelo.MesesServicios[0];
                programacionExistente.Mes2 = modelo.MesesServicios[1];
                programacionExistente.Mes3 = modelo.MesesServicios[2];
                programacionExistente.Mes4 = modelo.MesesServicios[3];
                programacionExistente.Mes5 = modelo.MesesServicios[4];
                programacionExistente.Mes6 = modelo.MesesServicios[5];
                programacionExistente.Mes7 = modelo.MesesServicios[6];
                programacionExistente.Mes8 = modelo.MesesServicios[7];
                programacionExistente.Mes9 = modelo.MesesServicios[8];
                programacionExistente.Mes10 = modelo.MesesServicios[9];
                programacionExistente.Mes11 = modelo.MesesServicios[10];
                programacionExistente.Mes12 = modelo.MesesServicios[11];

                // Meses Personas
                programacionExistente.Mes13 = modelo.MesesPersonas[0];
                programacionExistente.Mes14 = modelo.MesesPersonas[1];
                programacionExistente.Mes15 = modelo.MesesPersonas[2];
                programacionExistente.Mes16 = modelo.MesesPersonas[3];
                programacionExistente.Mes17 = modelo.MesesPersonas[4];
                programacionExistente.Mes18 = modelo.MesesPersonas[5];
                programacionExistente.Mes111 = modelo.MesesPersonas[6];
                programacionExistente.Mes112 = modelo.MesesPersonas[7];
                programacionExistente.Mes1111 = modelo.MesesPersonas[8];
                programacionExistente.Mes110 = modelo.MesesPersonas[9];
                programacionExistente.Mes121 = modelo.MesesPersonas[10];
                programacionExistente.Mes19 = modelo.MesesPersonas[11];

                // Acciones
                programacionExistente.Actividad1 = modelo.Acciones.Count > 0 ? modelo.Acciones[0].Descripcion : null;
                programacionExistente.Frecuencia1 = modelo.Acciones.Count > 0 ? modelo.Acciones[0].Frecuencia : null;
                programacionExistente.FechaProgramacion1 = modelo.Acciones.Count > 0 ? modelo.Acciones[0].FechaInicio : null;

                programacionExistente.Actividad2 = modelo.Acciones.Count > 1 ? modelo.Acciones[1].Descripcion : null;
                programacionExistente.Frecuencia2 = modelo.Acciones.Count > 1 ? modelo.Acciones[1].Frecuencia : null;
                programacionExistente.FechaProgramacion2 = modelo.Acciones.Count > 1 ? modelo.Acciones[1].FechaInicio : null;

                programacionExistente.Actividad3 = modelo.Acciones.Count > 2 ? modelo.Acciones[2].Descripcion : null;
                programacionExistente.Frecuencia3 = modelo.Acciones.Count > 2 ? modelo.Acciones[2].Frecuencia : null;
                programacionExistente.FechaProgramacion3 = modelo.Acciones.Count > 2 ? modelo.Acciones[2].FechaInicio : null;

                programacionExistente.Actividad4 = modelo.Acciones.Count > 3 ? modelo.Acciones[3].Descripcion : null;
                programacionExistente.Frecuencia4 = modelo.Acciones.Count > 3 ? modelo.Acciones[3].Frecuencia : null;
                programacionExistente.FechaProgramacion4 = modelo.Acciones.Count > 3 ? modelo.Acciones[3].FechaInicio : null;

                programacionExistente.Actividad5 = modelo.Acciones.Count > 4 ? modelo.Acciones[4].Descripcion : null;
                programacionExistente.Frecuencia5 = modelo.Acciones.Count > 4 ? modelo.Acciones[4].Frecuencia : null;
                programacionExistente.FechaProgramacion5 = modelo.Acciones.Count > 4 ? modelo.Acciones[4].FechaInicio : null;

                // Firmas
                programacionExistente.ElaboraNombre = modelo.ElaboraNombre;
                programacionExistente.ElaboroCargo = modelo.ElaboroCargo;
                programacionExistente.ValidoNombre = modelo.RevisionNombre;
                programacionExistente.ValidoCargo = modelo.RevisionCargo;
                programacionExistente.AutorizoNombre = modelo.AutorizacionNombre;
                programacionExistente.AutorizoCargo = modelo.AutorizacionCargo;

                programacionExistente.Acumulable = modelo.SelectAcumulable;
                programacionExistente.Totalanos = modelo.TotalAnos;
                programacionExistente.Totalanos2 = modelo.TotalAnos2;
                programacionExistente.Beneficiarios = modelo.Beneficiarios;

                await _repositorioProgramacion.Editar(programacionExistente);

                // 3. Actualizar LlenadoInterno si existe
                // 3. Actualizar LlenadoInterno si existe
                if (programacionExistente.IdLlenado.HasValue)
                {
                    var llenadoInterno = await _repositorioLlenadoInterno.Obtener(
                        l => l.IdProceso == programacionExistente.IdLlenado.Value);

                    if (llenadoInterno != null)
                    {
                        // ✅ AGREGAR ESTAS LÍNEAS - Actualizar Pp y Componente
                        llenadoInterno.Pp = modelo.Pp;
                        llenadoInterno.Componente = modelo.Componente;
                        llenadoInterno.Actividad = modelo.NActividad;

                        llenadoInterno.DescripcionActividad = modelo.DescripcionActividad;
                        llenadoInterno.ProgramaSocial = modelo.ProgramaSocial;
                        llenadoInterno.UnidadMedida = modelo.UnidadMedida;

                        // Actualizar totales mensuales
                        llenadoInterno.TotalEnero = modelo.MesesServicios[0];
                        llenadoInterno.TotalFebrero = modelo.MesesServicios[1];
                        llenadoInterno.TotalMarzo = modelo.MesesServicios[2];
                        llenadoInterno.TotalAbril = modelo.MesesServicios[3];
                        llenadoInterno.TotalMayo = modelo.MesesServicios[4];
                        llenadoInterno.TotalJunio = modelo.MesesServicios[5];
                        llenadoInterno.TotalJulio = modelo.MesesServicios[6];
                        llenadoInterno.TotalAgosto = modelo.MesesServicios[7];
                        llenadoInterno.TotalSeptiembre = modelo.MesesServicios[8];
                        llenadoInterno.TotalOctubre = modelo.MesesServicios[9];
                        llenadoInterno.TotalNoviembre = modelo.MesesServicios[10];
                        llenadoInterno.TotalDiciembre = modelo.MesesServicios[11];

                        // ✅ AGREGAR - Actualizar personas por mes
                        llenadoInterno.EneroPersona = modelo.MesesPersonas[0];
                        llenadoInterno.FebreroPersona = modelo.MesesPersonas[1];
                        llenadoInterno.MarzoPersona = modelo.MesesPersonas[2];
                        llenadoInterno.AbrilPersona = modelo.MesesPersonas[3];
                        llenadoInterno.MayoPersona = modelo.MesesPersonas[4];
                        llenadoInterno.JunioPersona = modelo.MesesPersonas[5];
                        llenadoInterno.JulioPersona = modelo.MesesPersonas[6];
                        llenadoInterno.AgostoPersona = modelo.MesesPersonas[7];
                        llenadoInterno.SeptiembrePersona = modelo.MesesPersonas[8];
                        llenadoInterno.OctubrePersona = modelo.MesesPersonas[9];
                        llenadoInterno.NoviembrePersona = modelo.MesesPersonas[10];
                        llenadoInterno.DiciembrePersona = modelo.MesesPersonas[11];

                        // ✅ AGREGAR - Actualizar totales
                        llenadoInterno.TotalProgramado = modelo.PrimerServicio + modelo.SegundoServicio + modelo.TercerServicio + modelo.CuartoServicio;
                        llenadoInterno.TotalPersona = modelo.PrimerPersona + modelo.SegundoPersona + modelo.TercerPersona + modelo.CuartoPersona;

                        await _repositorioLlenadoInterno.Editar(llenadoInterno);
                    }
                }

                // 4. Actualizar ServiciosMunicipio
                var serviciosExistentes = (await _repositorioServicios.Consultar(
                    sm => sm.IdLlenado == modelo.Id)).ToList();

                foreach (var servicio in serviciosExistentes)
                {
                    await _repositorioServicios.Eliminar(servicio);
                }

                foreach (var municipio in modelo.MunicipiosServicios)
                {
                    var nuevoServicio = new ServiciosMunicipio
                    {
                        IdLlenado = modelo.Id,
                        IdMunicipio = municipio.IdMunicipio,
                        NumeroBien = municipio.Cantidad
                    };
                    await _repositorioServicios.Crear(nuevoServicio);
                }

                // 5. Actualizar PersonasMunicipio
                var personasExistentes = (await _repositorioPersonas.Consultar(
                    pm => pm.IdLlenado == modelo.Id)).ToList();

                foreach (var persona in personasExistentes)
                {
                    await _repositorioPersonas.Eliminar(persona);
                }

                foreach (var municipio in modelo.MunicipiosPersonas)
                {
                    var nuevaPersona = new PersonasMunicipio
                    {
                        IdLlenado = modelo.Id,
                        IdMunicipio = municipio.IdMunicipio,
                        NumeroBien = municipio.Cantidad
                    };
                    await _repositorioPersonas.Crear(nuevaPersona);
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error al actualizar programación: {ex.Message}");
                return false;
            }
        }

    }
}
