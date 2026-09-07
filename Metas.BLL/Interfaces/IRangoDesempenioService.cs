using Metas.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Metas.BLL.Interfaces
{
    public interface IRangoDesempenioService
    {
        Task<List<RangoDesempenio>> Lista();
        Task<bool> GuardarRangos(List<RangoDesempenio> rangos);
        Task<string> ClasificarColor(decimal porcentaje);
        Task<bool> RequiereJustificacion(decimal porcentaje);
    }
}
