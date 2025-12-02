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
        Task<bool> GuardarProgramacion(ProgramacionDTO modelo);
        Task<List<LlenadoInterno>> ObtenerDatosProgramacion(int anoFiscal, int departamentoId);
        Task<bool> EliminarProgramacion(int idLlenado);
        Task<ProgramacionDTO> ObtenerDatosCompletos(int idLlenado);
        Task<Comentario> ObtenerComentariosPorProgramacion(int idProgramacion);
        Task<bool> ActualizarEstatusProgramacion(int idProgramacion, int nuevoEstatus);
        Task<bool> GuardarComentarios(List<ComentarioDTO> comentarios);
        Task<LlenadoInterno> ObtenerporId(int idLlenado);

        Task<bool> ActualizarProgramacion(ProgramacionDTO modelo);

    }
}