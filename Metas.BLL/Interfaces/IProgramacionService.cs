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
    }
}
