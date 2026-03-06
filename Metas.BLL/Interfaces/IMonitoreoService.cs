using Metas.BLL.DTO;
using Metas.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metas.BLL.Interfaces
{
    public interface IMonitoreoService
    {
        Task<bool> GuardarActualizacion(GuardarActualizacionDTO modelo, string rutaEvidencia, string rutaJustificacion);
        Task<LlenadoExterno> ObtenerLlenadoMensual(int id, int mes);
        Task<bool> HabilitarCaptura(int idProceso, int mes);
        Task<bool> ActualizarRegistro(DatosEdicionDTO modelo);
        Task<bool> EliminarCapturaMes(int idProceso, int mes);
        Task<bool> CrearNuevoProceso(DatosEdicionDTO modelo);
        Task<List<LlenadoExterno>> ObtenerLlenadosPorProcesos(List<int> idsProcesos);
    }
}
