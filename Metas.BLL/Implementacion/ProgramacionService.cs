using Metas.BLL.DTO;
using Metas.BLL.Interfaces;
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

        public ProgramacionService(IGenericRepository<LlenadoInterno> repositorioLlenadoInterno,
            IGenericRepository<Departamento> repositoryDepartamento,
            IGenericRepository<Programacion> repositorioProgramacion,
            IGenericRepository<ServiciosMunicipio> repositorioServicios,
            IGenericRepository<PersonasMunicipio> repositorioPersonas)
        {
            _repositorioLlenadoInterno = repositorioLlenadoInterno;
            _repositoryDepartamento = repositoryDepartamento;
            _repositorioProgramacion = repositorioProgramacion;
            _repositorioServicios = repositorioServicios;
            _repositorioPersonas = repositorioPersonas;
        }

        public async Task<bool> GuardarProgramacion(ProgramacionDTO modelo)
        {
            try
            {
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
                    FuenteInfo = modelo.FuenteInformacion,
                    IntervienenDelegaciones = modelo.IntervienenDelegaciones,
                    IntervienenDelegacionesManera = modelo.IntervienenDelegacionesManera,

                    // Línea Base
                    Anos = modelo.AnoBase,
                    Valor = modelo.PorcentajeBase,
                    BienServicio = modelo.ServicioBase,
                    NoPersonas = modelo.PersonasBase,

                    // Meta Anual
                    Anos2 = modelo.AnoMeta,
                    Valor2 = modelo.PorcentajeMeta,
                    BienServicio2 = modelo.ServicioMeta,
                    NoPersonas2 = modelo.PersonasMeta,

                    // Trimestres Servicios
                    Servicio1 = modelo.PrimerServicio,
                    Servicio2 = modelo.SegundoServicio,
                    Servicio3 = modelo.TercerServicio,
                    Servicio4 = modelo.CuartoServicio,

                    // Trimestres Personas
                    Personas1 = modelo.PrimerPersona,
                    Personas2 = modelo.SegundoPersona,
                    Personas3 = modelo.TercerPersona,
                    Personas4 = modelo.CuartoPersona,

                    // Meses Servicios (Grupo 1)
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

                    // Meses Personas (Grupo 2)
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

                    // Acciones
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

                    // Firmas
                    ElaboraNombre = modelo.ElaboraNombre,
                    ElaboroCargo = modelo.ElaboroCargo,
                    ValidoNombre = modelo.RevisionNombre,
                    ValidoCargo = modelo.RevisionCargo,
                    AutorizoNombre = modelo.AutorizacionNombre,
                    AutorizoCargo = modelo.AutorizacionCargo
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
                    ProgramaSocial = modelo.ProgramaSocial,
                    Ano = modelo.AnoMeta,
                    UnidadMedida = modelo.UnidadMedida,
                    TotalProgramado = modelo.PrimerServicio + modelo.SegundoServicio + modelo.TercerServicio + modelo.CuartoServicio,
                    PersonasProgramadas = modelo.PrimerPersona + modelo.SegundoPersona + modelo.TercerPersona + modelo.CuartoPersona,
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

                return true;
            }
            catch (Exception ex)
            {
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
    }
}
