using Metas.BLL.Interfaces;
using Metas.BLL.DTO;
using Metas.DAL.Interfaces;
using Metas.Entity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly IGenericRepository<Pp> _repositorioProgramas;

        // El catalogo esta en espanol y la app fija la cultura en en-US, asi que con un
        // OrderBy a secas las palabras acentuadas (Area, Numero) caerian despues de la Z.
        private static readonly StringComparer ComparadorCatalogo =
            StringComparer.Create(CultureInfo.GetCultureInfo("es-ES"), false);

        public DepartamentoService(IGenericRepository<Departamento> repositorio, IGenericRepository<PpCompuesto> repositorioComponentes, IGenericRepository<UnidadMedidum> repositorioMedidas,
            IGenericRepository<Municipio> repositorioMunicipios, IGenericRepository<Pp> repositorioProgramas)
        {
            _repositorio = repositorio;
            _repositorioComponentes = repositorioComponentes;
            _repositorioMedidas = repositorioMedidas;
            _repositorioMunicipios = repositorioMunicipios;
            _repositorioProgramas = repositorioProgramas;
        }

        public async Task<List<Pp>> ObtenerProgramas()
        {
            var query = await _repositorioProgramas.Consultar();

            var programas = query
                .Where(p => !string.IsNullOrWhiteSpace(p.Clave))
                .Select(p => new Pp
                {
                    IdPp = p.IdPp,
                    Clave = p.Clave,
                    NombrePp = p.NombrePp
                })
                .OrderBy(p => p.Clave)
                .ToList();

            return programas;
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
                .Where(d => d.Activo != false)
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

        public async Task<List<Departamento>> ObtenerDepartamentosAdmin()
        {
            var query = await _repositorio.Consultar();

            return query
                .Select(d => new Departamento
                {
                    IdDepartamento = d.IdDepartamento,
                    Area = d.Area,
                    Departamento1 = d.Departamento1,
                    Activo = d.Activo
                })
                .ToList()
                .OrderBy(d => d.Area, ComparadorCatalogo)
                .ThenBy(d => d.Departamento1, ComparadorCatalogo)
                .ToList();
        }

        public async Task<Departamento> CrearDepartamento(Departamento modelo)
        {
            var area = modelo.Area?.Trim();
            var departamento = modelo.Departamento1?.Trim();

            ValidarAreaYDepartamento(area, departamento);

            if (await ExisteAreaDepartamento(area, departamento, null))
            {
                throw new InvalidOperationException("Ya existe un registro con esa área y departamento.");
            }

            var entidad = new Departamento
            {
                Area = area,
                Departamento1 = departamento,
                Unidad = null,
                UnidadRepresentante = "N/A",
                Activo = true
            };

            var creado = await _repositorio.Crear(entidad);

            if (creado == null || creado.IdDepartamento == 0)
            {
                throw new InvalidOperationException("No se pudo crear el registro.");
            }

            return creado;
        }

        public async Task<bool> EditarDepartamento(Departamento modelo)
        {
            var entidad = await _repositorio.Obtener(d => d.IdDepartamento == modelo.IdDepartamento);

            if (entidad == null)
            {
                throw new InvalidOperationException("No se encontró el registro a editar.");
            }

            var area = modelo.Area?.Trim();
            var departamento = modelo.Departamento1?.Trim();

            ValidarAreaYDepartamento(area, departamento);

            if (await ExisteAreaDepartamento(area, departamento, modelo.IdDepartamento))
            {
                throw new InvalidOperationException("Ya existe un registro con esa área y departamento.");
            }

            entidad.Area = area;
            entidad.Departamento1 = departamento;
            entidad.Activo = modelo.Activo;

            return await _repositorio.Editar(entidad);
        }

        public async Task<bool> CambiarEstadoDepartamento(int id, bool activo)
        {
            var entidad = await _repositorio.Obtener(d => d.IdDepartamento == id);

            if (entidad == null)
            {
                throw new InvalidOperationException("No se encontró el registro.");
            }

            entidad.Activo = activo;
            return await _repositorio.Editar(entidad);
        }

        private void ValidarAreaYDepartamento(string area, string departamento)
        {
            if (string.IsNullOrWhiteSpace(area))
            {
                throw new InvalidOperationException("El área es requerida.");
            }

            if (string.IsNullOrWhiteSpace(departamento))
            {
                throw new InvalidOperationException("El departamento es requerido.");
            }

            if (area.Length > 200 || departamento.Length > 200)
            {
                throw new InvalidOperationException("El área y el departamento no deben exceder 200 caracteres.");
            }
        }

        private async Task<bool> ExisteAreaDepartamento(string area, string departamento, int? excluirId)
        {
            var query = await _repositorio.Consultar();

            return query
                .ToList()
                .Any(d =>
                    (!excluirId.HasValue || d.IdDepartamento != excluirId.Value) &&
                    string.Equals(d.Area?.Trim(), area, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(d.Departamento1?.Trim(), departamento, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<List<SelectListItem>> ObtenerListaPorTipo(string tipo)
        {
            var query = await _repositorio.Consultar();
            List<SelectListItem> lista = new List<SelectListItem>();

            switch (tipo)
            {
                case "Unidad":
                    lista = query
                        .Where(p => p.Activo != false)
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
                        .Where(p => p.Activo != false)
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
                        .Where(p => p.Activo != false)
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

            // Se normaliza antes de agrupar: el Distinct() sobre el POCO comparaba por
            // referencia y no deduplicaba nada.
            var medidas = query
                .Where(d => !string.IsNullOrWhiteSpace(d.Valor))
                .Select(d => new { IdUnidad = d.IdUnidad, Valor = d.Valor!.Trim() })
                .ToList();

            return medidas
                .GroupBy(d => d.Valor, StringComparer.OrdinalIgnoreCase)
                .OrderBy(g => g.Key, ComparadorCatalogo)
                .Select(g => new UnidadMedidum
                {
                    IdUnidad = g.Min(d => d.IdUnidad),
                    Valor = g.Key
                })
                .ToList();
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
