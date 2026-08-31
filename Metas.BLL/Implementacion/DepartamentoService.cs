using Metas.BLL.Interfaces;
using Metas.BLL.DTO;
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
                    ComponenteCompuesto = d.ComponenteCompuesto,
                    Componente = d.Componente
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

        public async Task<Departamento> ObtenerDepartamento(int idDepartamento)
        {
            var resultado = await _repositorio.Obtener(d => d.IdDepartamento == idDepartamento);
            return resultado;
        }

        public async Task<bool> GuardarDocumentos(DepartamentoDocumentosDTO modelo)
        {
            try
            {
                var departamento = await _repositorio.Obtener(d => d.IdDepartamento == modelo.IdDepartamento);
                if (departamento == null)
                {
                    return false;
                }

                // Conservar rutas existentes y aplicar las nuevas cuando se subió archivo
                if (!string.IsNullOrEmpty(modelo.RutaReglasOperacion))
                    departamento.ReglasOperacion = modelo.RutaReglasOperacion;
                if (!string.IsNullOrEmpty(modelo.RutaLineamientosOperacion))
                    departamento.LineamientosOperacion = modelo.RutaLineamientosOperacion;
                if (!string.IsNullOrEmpty(modelo.RutaArbolProblemasObjetivos))
                    departamento.ArbolProblemasObjetivos = modelo.RutaArbolProblemasObjetivos;
                if (!string.IsNullOrEmpty(modelo.RutaMetodologiaPadron))
                    departamento.MetodologiaPadron = modelo.RutaMetodologiaPadron;
                if (!string.IsNullOrEmpty(modelo.RutaDiagnostico))
                    departamento.Diagnostico = modelo.RutaDiagnostico;

                if (!string.IsNullOrEmpty(modelo.Justificacion))
                    departamento.Justificacion = modelo.Justificacion;

                return await _repositorio.Editar(departamento);
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> GuardarJustificacion(int idDepartamento, string justificacion)
        {
            try
            {
                var departamento = await _repositorio.Obtener(d => d.IdDepartamento == idDepartamento);
                if (departamento == null)
                {
                    return false;
                }

                departamento.Justificacion = justificacion;
                return await _repositorio.Editar(departamento);
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> EliminarDocumento(int idDepartamento, string tipoDocumento)
        {
            try
            {
                var departamento = await _repositorio.Obtener(d => d.IdDepartamento == idDepartamento);
                if (departamento == null)
                {
                    return null;
                }

                string rutaEliminada = null;
                switch (tipoDocumento.ToLower())
                {
                    case "reglas":
                        rutaEliminada = departamento.ReglasOperacion;
                        departamento.ReglasOperacion = null;
                        break;
                    case "lineamientos":
                        rutaEliminada = departamento.LineamientosOperacion;
                        departamento.LineamientosOperacion = null;
                        break;
                    case "arbol":
                        rutaEliminada = departamento.ArbolProblemasObjetivos;
                        departamento.ArbolProblemasObjetivos = null;
                        break;
                    case "metodologia":
                        rutaEliminada = departamento.MetodologiaPadron;
                        departamento.MetodologiaPadron = null;
                        break;
                    case "diagnostico":
                        rutaEliminada = departamento.Diagnostico;
                        departamento.Diagnostico = null;
                        break;
                    default:
                        return null;
                }

                bool guardado = await _repositorio.Editar(departamento);
                return guardado ? rutaEliminada : null;
            }
            catch
            {
                throw;
            }
        }
    }
}
