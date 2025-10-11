using Metas.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metas.BLL.Interfaces
{
    public interface IFechasService
    {
        Task<List<FechaCaptura>> Lista();
        Task<List<CapturaProgramacion>> ListaProgramacion();
        Task<FechaCaptura> Editar(FechaCaptura entidad);
        Task<CapturaProgramacion> EditarProgramacion(CapturaProgramacion entidad);
        Task<bool> ValidarFechaHabilitada(int anoFiscal);
    }
}
