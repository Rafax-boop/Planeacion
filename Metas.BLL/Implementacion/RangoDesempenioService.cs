using Metas.BLL.Interfaces;
using Metas.DAL.Interfaces;
using Metas.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Metas.BLL.Implementacion
{
    public class RangoDesempenioService : IRangoDesempenioService
    {
        private readonly IGenericRepository<RangoDesempenio> _repositorio;

        public RangoDesempenioService(IGenericRepository<RangoDesempenio> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<RangoDesempenio>> Lista()
        {
            IQueryable<RangoDesempenio> query = await _repositorio.Consultar();
            return query.OrderBy(r => r.Orden).ToList();
        }

        public async Task<bool> GuardarRangos(List<RangoDesempenio> rangos)
        {
            try
            {
                IQueryable<RangoDesempenio> actuales = await _repositorio.Consultar();
                List<RangoDesempenio> listaActual = actuales.ToList();

                foreach (var rango in rangos)
                {
                    var existente = listaActual.FirstOrDefault(r => r.Id == rango.Id);
                    if (existente != null)
                    {
                        existente.Nombre = rango.Nombre;
                        existente.Minimo = rango.Minimo;
                        existente.Maximo = rango.Maximo;
                        existente.ClaseColor = rango.ClaseColor;
                        existente.RequiereJustificacion = rango.RequiereJustificacion;
                        existente.Orden = rango.Orden;
                        await _repositorio.Editar(existente);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> ClasificarColor(decimal porcentaje)
        {
            var rangos = await Lista();
            var coincidencia = rangos.FirstOrDefault(r => porcentaje >= r.Minimo && (!r.Maximo.HasValue || porcentaje <= r.Maximo.Value));
            return coincidencia?.ClaseColor ?? "error";
        }

        public async Task<bool> RequiereJustificacion(decimal porcentaje)
        {
            var rangos = await Lista();
            var coincidencia = rangos.FirstOrDefault(r => porcentaje >= r.Minimo && (!r.Maximo.HasValue || porcentaje <= r.Maximo.Value));
            return coincidencia?.RequiereJustificacion ?? false;
        }
    }
}
