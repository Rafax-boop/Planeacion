using Metas.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metas.BLL.Interfaces
{
    public interface IDepartamentoService
    {
        Task<List<Departamento>> ObtenerDepartamentos();
    }
}
