using Metas.BLL.Interfaces;
using Metas.DAL.Interfaces;
using Metas.Entity;
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
        public DepartamentoService(IGenericRepository<Departamento> repositorio)
        {
            _repositorio = repositorio;
        }
        public async Task<List<Departamento>> ObtenerDepartamentos()
        {
            var query = await _repositorio.Consultar();

            var departamentos = query
                .Select(d => new Departamento
                {
                    IdDepartamento = d.IdDepartamento,
                    Departamento1 = d.Departamento1,
                })
                .Distinct()
                .ToList();

            return departamentos;
        }
    }
}
