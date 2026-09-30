using MC.Business.Logic.Layer.Interfaces;
using MC.DTO.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace MC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuariosController : CustomControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public IActionResult ObtenerPaginado([FromQuery] int? idEmpresa, [FromQuery] string usuario = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = _usuarioService.ObtenerUsuariosPaginado(idEmpresa, usuario, pageNumber, pageSize);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(result.Resultado);
        }

        [HttpPost]
        public IActionResult Crear([FromBody] UsuarioDto request)
        {
            var result = _usuarioService.CrearUsuario(request);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(result.Resultado);
        }

        [HttpPut("{idUsuario}")]
        public IActionResult Actualizar(int idUsuario, [FromBody] UsuarioDto request)
        {
            var result = _usuarioService.ActualizarUsuario(idUsuario, request);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(result.Resultado);
        }

        [HttpDelete("{idUsuario}")]
        public IActionResult Eliminar(int idUsuario)
        {
            var result = _usuarioService.EliminarUsuario(idUsuario);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(new { eliminado = result.Resultado });
        }
    }
}
