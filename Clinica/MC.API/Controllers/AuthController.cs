using MC.Business.Logic.Layer.Interfaces;
using MC.DTO.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace MC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : CustomControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthLoginRequestDto request)
        {
            var result = _authService.Login(request);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }

            Response.Headers.Add("Access-Control-Expose-Headers", "Authorization");
            Response.Headers.Add("Authorization", result.Resultado.Token);
            return Ok(result.Resultado);
        }

        [Authorize]
        [HttpGet("menus")]
        public IActionResult Menus()
        {
            var result = _authService.ObtenerMenusPorUsuario(UserJwt.CodUsuario, UserJwt.IdEmpresa, UserJwt.IdSede);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }

            return Ok(result.Resultado);
        }

        [Authorize]
        [HttpPost("seleccionar-sede")]
        public IActionResult SeleccionarSede([FromBody] AuthSeleccionarSedeRequestDto request)
        {
            int idEmpresa = UserJwt.IdEmpresa ?? 0;
            int idRol = UserJwt.IdRol ?? 0;
            string usuario = UserJwt.CodUsuario;
            string rol = UserJwt.Perfil;
            int idSede = request?.IdSede ?? 0;

            var result = _authService.SeleccionarSede(usuario, idEmpresa, idSede, idRol, rol);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }

            Response.Headers.Add("Access-Control-Expose-Headers", "Authorization");
            Response.Headers.Add("Authorization", result.Resultado.Token);
            return Ok(result.Resultado);
        }

        [AllowAnonymous]
        [HttpGet("empresas")]
        public IActionResult Empresas()
        {
            var result = _authService.ObtenerEmpresas();
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }

            return Ok(result.Resultado);
        }

        [AllowAnonymous]
        [HttpGet("sedes/{idEmpresa}")]
        public IActionResult Sedes(int idEmpresa)
        {
            var result = _authService.ObtenerSedesPorEmpresa(idEmpresa);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }

            return Ok(result.Resultado);
        }
    }
}
