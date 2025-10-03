using Metas.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metas.BLL.Interfaces
{
    public interface IUsuarioService
    {
        Task<Usuario> ObtenerPorCredenciales(string usuario, string clave);
    }
}
