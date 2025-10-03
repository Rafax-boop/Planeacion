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
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _repositorio;

        public UsuarioService(IGenericRepository<Usuario> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<Usuario> ObtenerPorCredenciales(string usuario, string clave)
        {
            Usuario usuarioEncontrado = await _repositorio.Obtener(
                u => u.Usuario1.Equals(usuario) && u.Pass.Equals(clave));

            return usuarioEncontrado;
        }
    }
}
