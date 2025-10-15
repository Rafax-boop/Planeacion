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

        public ProgramacionService(IGenericRepository<LlenadoInterno> repositorioLlenadoInterno, IGenericRepository<Departamento> repositoryDepartamento)
        {
            _repositorioLlenadoInterno = repositorioLlenadoInterno;
            _repositoryDepartamento = repositoryDepartamento;
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
