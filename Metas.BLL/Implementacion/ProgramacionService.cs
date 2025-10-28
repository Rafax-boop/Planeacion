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
        private readonly IGenericRepository<Comentario> _repositorioComentario;
        private readonly MetasContext _context;

        public ProgramacionService(IGenericRepository<LlenadoInterno> repositorioLlenadoInterno,
            IGenericRepository<Departamento> repositoryDepartamento,
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

                var personasMunicipios = (await _repositorioPersonas.Consultar(
                    pm => pm.IdLlenado == idProgramacion)).ToList();

                foreach (var pm in personasMunicipios)
                {
                    await _repositorioPersonas.Eliminar(pm);
                }

                var serviciosMunicipios = (await _repositorioServicios.Consultar(
                    sm => sm.IdLlenado == idProgramacion)).ToList();

                foreach (var sm in serviciosMunicipios)
                {
                    await _repositorioServicios.Eliminar(sm);
                }

                await _repositorioProgramacion.Eliminar(programacionEntidad);

                var llenadoInternoEntidad = await _repositorioLlenadoInterno.Obtener(
                    l => l.IdProceso == idLlenado);

                if (llenadoInternoEntidad != null)
                {
                    await _repositorioLlenadoInterno.Eliminar(llenadoInternoEntidad);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
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
        public async Task<bool> GuardarComentarios(List<ComentarioDTO> comentarios)
        {
            try
            {
                Console.WriteLine("🔍 INICIO GuardarComentarios");

                // Validar que lleguen comentarios
                if (comentarios == null || !comentarios.Any())
                {
                    Console.WriteLine("❌ Lista de comentarios vacía o nula");
                    return false;
                }

                var idRegistro = comentarios.First().IdProgramacion;
                Console.WriteLine($"🔍 IdProgramacion: {idRegistro}");
                Console.WriteLine($"🔍 Cantidad de comentarios: {comentarios.Count}");

                // Buscar si ya existe un registro de comentarios para esta programación
                var comentarioExistente = await _context.Comentarios
                    .FirstOrDefaultAsync(c => c.IdProgramacion == idRegistro);

                // Si no existe, crear uno nuevo
                if (comentarioExistente == null)
                {
                    Console.WriteLine("🔍 No existe registro, creando nuevo...");
                    comentarioExistente = new Comentario { IdProgramacion = idRegistro };
                    _context.Comentarios.Add(comentarioExistente);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("✅ Registro creado");
                }
                else
                {
                    Console.WriteLine($"🔍 Registro existente encontrado: Id={comentarioExistente.IdComentario}");
                }

                // Asignar cada comentario a su propiedad correspondiente usando reflexión
                foreach (var com in comentarios)
                {
                    Console.WriteLine($"🔍 Procesando ComentarioId: {com.ComentarioId}, Texto: {com.Texto}");

                    // Construir el nombre de la propiedad: "Comentario1", "Comentario2", etc.
                    var nombrePropiedad = $"Comentario{com.ComentarioId}";

                    // Obtener la propiedad de la clase Comentario
                    var propiedad = typeof(Comentario).GetProperty(nombrePropiedad);

                    // Si la propiedad existe, asignar el valor
                    if (propiedad != null && propiedad.CanWrite)
                    {
                        propiedad.SetValue(comentarioExistente, com.Texto);
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ Advertencia: Propiedad {nombrePropiedad} no encontrada");
                    }
                }

                Console.WriteLine("🔍 Guardando cambios en BD...");
                await _context.SaveChangesAsync();
                Console.WriteLine("✅ Cambios guardados correctamente");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR GuardarComentarios: {ex.Message}");
                Console.WriteLine($"❌ StackTrace: {ex.StackTrace}");
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
    }
}
