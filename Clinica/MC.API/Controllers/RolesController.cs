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
    public class RolesController : CustomControllerBase
    {
        private readonly IRolService _rolService;

        public RolesController(IRolService rolService)
        {
            _rolService = rolService;
        }

        [HttpGet]
        public IActionResult ObtenerPaginado([FromQuery] int? idEmpresa, [FromQuery] string nombre = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (!TryEmpresaSesion(out int idEmpresaSesion, out IActionResult error))
            {
                return error;
            }
            var result = _rolService.ObtenerRolesPaginado(idEmpresaSesion, nombre, pageNumber, pageSize);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(result.Resultado);
        }

        [HttpPost]
        public IActionResult Crear([FromBody] RolDto request)
        {
            if (!TryEmpresaSesion(out int idEmpresaSesion, out IActionResult error))
            {
                return error;
            }
            request.IdEmpresa = idEmpresaSesion;
            var result = _rolService.CrearRol(request);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(result.Resultado);
        }

        [HttpPut("{idRol}")]
        public IActionResult Actualizar(int idRol, [FromBody] RolDto request)
        {
            if (!TryEmpresaSesion(out int idEmpresaSesion, out IActionResult error))
            {
                return error;
            }
            request.IdEmpresa = idEmpresaSesion;
            var result = _rolService.ActualizarRol(idRol, request);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(result.Resultado);
        }

        [HttpDelete("{idRol}")]
        public IActionResult Eliminar(int idRol)
        {
            var result = _rolService.EliminarRol(idRol);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(new { eliminado = result.Resultado });
        }
    }
}
