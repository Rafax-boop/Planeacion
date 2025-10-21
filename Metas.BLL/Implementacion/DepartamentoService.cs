using Metas.BLL.Interfaces;
using Metas.DAL.Interfaces;
using Metas.Entity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metas.BLL.Implementacion
{
    public class DepartamentoService : IDepartamentoService
    {
        private readonly IGenericRepository<Departamento> _repositorio;
        private readonly IGenericRepository<PpCompuesto> _repositorioComponentes;
        private readonly IGenericRepository<UnidadMedidum> _repositorioMedidas;
        private readonly IGenericRepository<Municipio> _repositorioMunicipios;
        public DepartamentoService(IGenericRepository<Departamento> repositorio, IGenericRepository<PpCompuesto> repositorioComponentes, IGenericRepository<UnidadMedidum> repositorioMedidas,
            IGenericRepository<Municipio> repositorioMunicipios)
        {
            _repositorio = repositorio;
            _repositorioComponentes = repositorioComponentes;
            _repositorioMedidas = repositorioMedidas;
            _repositorioMunicipios = repositorioMunicipios;
        }

        public async Task<List<PpCompuesto>> ObtenerComponentes()
        {
            var query = await _repositorioComponentes.Consultar();

            var programas = query
                .Select(d => new PpCompuesto
                {
                    IdPp = d.IdPp,
                    Pp = d.Pp,
                    PpCompuesto1 = d.PpCompuesto1,
                    ComponenteCompuesto = d.ComponenteCompuesto
                })
                .Distinct()
                .ToList();

            return programas;
        }

        public async Task<List<Departamento>> ObtenerDepartamentos()
        {
            var query = await _repositorio.Consultar();

            var departamentos = query
                .Select(d => new Departamento
                {
                    IdDepartamento = d.IdDepartamento,
                    Departamento1 = d.Departamento1,
                    Area = d.Area
                })
                .Distinct()
                .ToList();

            return departamentos;
        }

        public async Task<List<SelectListItem>> ObtenerListaPorTipo(string tipo)
        {
            var query = await _repositorio.Consultar();
            List<SelectListItem> lista = new List<SelectListItem>();

            switch (tipo)
            {
                case "Unidad":
                    lista = query
                        .Select(p => new SelectListItem
                        {
                            Text = p.UnidadRepresentante,
                            Value = p.UnidadRepresentante
                        })
                        .GroupBy(p => p.Text)
                        .Select(g => g.First())
                        .ToList();
                    break;

                case "Dirección":
                    lista = query
                        .Select(p => new SelectListItem
                        {
                            Text = p.Area,
                            Value = p.Area
                        })
                        .GroupBy(p => p.Text)
                        .Select(g => g.First())
                        .ToList();
                    break;

                case "Departamento":
                    lista = query
                        .Select(p => new SelectListItem
                        {
                            Text = p.Departamento1,
                            Value = p.Departamento1
                        })
                        .GroupBy(p => p.Text)
                        .Select(g => g.First())
                        .ToList();
                    break;

                default:
                    lista = new List<SelectListItem>();
                    break;
            }

            return lista;
        }

        public async Task<List<UnidadMedidum>> ObtenerMedidas()
        {
            var query = await _repositorioMedidas.Consultar();

            var medidas = query
                .Select(d => new UnidadMedidum
                {
                    IdUnidad = d.IdUnidad,
                    Valor = d.Valor
                })
                .Distinct()
                .ToList();

            return medidas;
        }

        public async Task<List<Municipio>> ObtenerMunicipios()
        {
            var query = await _repositorioMunicipios.Consultar();

            var municipios = query
                .Select(d => new Municipio
                {
                    IdMunicipio = d.IdMunicipio,
                    NumeroRegion = d.NumeroRegion,
                    NombreMunicipios = d.NombreMunicipios,
                    NombreRegion = d.NombreRegion,
                    ClaveMuni = d.ClaveMuni
                })
                .Distinct()
                .ToList();

            return municipios;
        }
    }
}
