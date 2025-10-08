using Metas.BLL.Interfaces;
using Metas.DAL.Interfaces;
using Metas.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public async Task<List<Usuario>> Lista()
        {
            IQueryable<Usuario> query = await _repositorio.Consultar();
            return query.ToList();
        }

        public async Task<bool> Crear(Usuario entidad)
        {
            try
            {
                Usuario usuarioCreado = await _repositorio.Crear(entidad);

                if (usuarioCreado == null || usuarioCreado.IdUsuario == 0)
                {
                    throw new InvalidOperationException("No se pudo crear el usuario");
                }

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int idUsuario)
        {
            try
            {
                var usuarioEncontrado = await _repositorio.Obtener(u => u.IdUsuario == idUsuario);

                bool resultado = await _repositorio.Eliminar(usuarioEncontrado);

                return resultado;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
