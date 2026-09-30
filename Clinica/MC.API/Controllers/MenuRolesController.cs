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
    public class MenuRolesController : CustomControllerBase
    {
        private readonly IMenuRolService _menuRolService;

        public MenuRolesController(IMenuRolService menuRolService)
        {
            _menuRolService = menuRolService;
        }

        [HttpGet("roles/{idEmpresa}")]
        public IActionResult ObtenerRolesPorEmpresa(int idEmpresa)
        {
            var result = _menuRolService.ObtenerRolesPorEmpresa(idEmpresa);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(result.Resultado);
        }

        [HttpGet("menus/{idRol}")]
        public IActionResult ObtenerMenusPorRol(int idRol)
        {
            var result = _menuRolService.ObtenerArbolMenusPorRol(idRol);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(result.Resultado);
        }

        [HttpPost("guardar")]
        public IActionResult Guardar([FromBody] GuardarMenuRolRequestDto request)
        {
            var result = _menuRolService.GuardarMenusRol(request);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(new { guardado = result.Resultado });
        }
    }
}
