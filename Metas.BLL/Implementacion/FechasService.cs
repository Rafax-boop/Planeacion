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
    public class FechasService : IFechasService
    {
        private readonly IGenericRepository<FechaCaptura> _repositorioCaptura;
        private readonly IGenericRepository<CapturaProgramacion> _repositorioProgramacion;

        public FechasService(IGenericRepository<FechaCaptura> repositorioCaptura, IGenericRepository<CapturaProgramacion> repositorioProgramacion)
        {
            _repositorioCaptura = repositorioCaptura;
            _repositorioProgramacion = repositorioProgramacion;
        }

        public async Task<FechaCaptura> Editar(FechaCaptura entidad)
        {
            try
            {
                IQueryable<FechaCaptura> query = await _repositorioCaptura.Consultar(u => u.IdFechaCaptura == entidad.IdFechaCaptura);

                FechaCaptura fechaEditar = query.First();
                fechaEditar.FechaInicio = entidad.FechaInicio;
                fechaEditar.FechaFin = entidad.FechaFin;
                fechaEditar.Ano = entidad.Ano;

                bool respuesta = await _repositorioCaptura.Editar(fechaEditar);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo modificar las fechas");

                FechaCaptura fechaEditada = query.First();

                return fechaEditada;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<CapturaProgramacion> EditarProgramacion(CapturaProgramacion entidad)
        {
            try
            {
                IQueryable<CapturaProgramacion> query = await _repositorioProgramacion.Consultar(u => u.IdFechaCaptura == entidad.IdFechaCaptura);

                CapturaProgramacion fechaEditar = query.First();
                fechaEditar.FechaInicio = entidad.FechaInicio;
                fechaEditar.FechaFin = entidad.FechaFin;
                fechaEditar.Ano = entidad.Ano;

                bool respuesta = await _repositorioProgramacion.Editar(fechaEditar);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo modificar las fechas");

                CapturaProgramacion fechaEditada = query.First();

                return fechaEditada;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<FechaCaptura>> Lista()
        {
            IQueryable<FechaCaptura> query = await _repositorioCaptura.Consultar();
            return query.ToList();
        }

        public async Task<List<CapturaProgramacion>> ListaProgramacion()
        {
            IQueryable<CapturaProgramacion> query = await _repositorioProgramacion.Consultar();
            return query.ToList();
        }

        public async Task<bool> ValidarFechaHabilitada(int anoFiscal)
        {
            try
            {
                var fechaActual = DateTime.Now.Date;

                // Buscar el registro de fechas para el año fiscal
                var captura = await _repositorioProgramacion.Obtener(x => x.Ano == anoFiscal);

                if (captura == null)
                    return false;

                // Validar que la fecha actual esté dentro del rango
                if (captura.FechaInicio.HasValue && captura.FechaFin.HasValue)
                {
                    var fechaInicio = captura.FechaInicio.Value.ToDateTime(TimeOnly.MinValue);
                    var fechaFin = captura.FechaFin.Value.ToDateTime(TimeOnly.MaxValue);

                    return fechaActual >= fechaInicio && fechaActual <= fechaFin;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
