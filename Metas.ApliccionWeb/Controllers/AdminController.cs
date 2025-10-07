using AutoMapper;
using Metas.AplicacionWeb.Models.ViewModels;
using Metas.BLL.Implementacion;
using Metas.BLL.Interfaces;
using Metas.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Metas.AplicacionWeb.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioService _usuarioService;
        private readonly IDepartamentoService _departamentoService;
        public AdminController(IUsuarioService usuarioService, IMapper mapper, IDepartamentoService departamentoService)
        {
            _usuarioService = usuarioService;
            _mapper = mapper;
            _departamentoService = departamentoService;
        }
        public async Task<ActionResult> Usuarios()
        {
            List<VMUsuario> vmUsuarioLista = _mapper.Map<List<VMUsuario>>(await _usuarioService.Lista());
            return View(vmUsuarioLista);
        }

        [HttpPost]
        public async Task<IActionResult> GetTipo(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new List<object>());
                }

                var lista = await _departamentoService.ObtenerListaPorTipo(id);
                return Json(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al obtener datos", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear(VMUsuario modelo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { mensaje = "Datos inválidos" });
                }

                var usuario = _mapper.Map<Usuario>(modelo);
                bool resultado = await _usuarioService.Crear(usuario);

                if (resultado)
                {
                    return Ok(new { mensaje = "Usuario creado exitosamente" });
                }
                else
                {
                    return BadRequest(new { mensaje = "No se pudo crear el usuario" });
                }
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno" + ex.Message, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                bool resultado = await _usuarioService.Eliminar(id);

                if (resultado)
                {
                    return Ok(new { mensaje = "Usuario eliminado exitosamente" });
                }
                else
                {
                    return BadRequest(new { mensaje = "No se pudo eliminar el usuario" });
                }
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar", error = ex.Message });
            }
        }
    }
}
