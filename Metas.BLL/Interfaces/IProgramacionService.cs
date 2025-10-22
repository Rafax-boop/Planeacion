using Metas.BLL.DTO;
using Metas.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metas.BLL.Interfaces
{
    public interface IProgramacionService
    {
        Task<List<LlenadoInterno>> ObtenerDatosProgramacion(int anoFiscal, int departamentoId);
        Task<bool> GuardarProgramacion(ProgramacionDTO modelo);
        Task<bool> EliminarProgramacion(int idLlenado);
        Task<ProgramacionDTO> ObtenerDatosCompletos(int id);
    }
}
