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
    public class EmpresasController : CustomControllerBase
    {
        private readonly IEmpresaService _empresaService;

        public EmpresasController(IEmpresaService empresaService)
        {
            _empresaService = empresaService;
        }

        [HttpGet]
        public IActionResult ObtenerPaginado([FromQuery] string nombre = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = _empresaService.ObtenerEmpresasPaginado(nombre, pageNumber, pageSize);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(result.Resultado);
        }

        [HttpPost]
        public IActionResult Crear([FromBody] EmpresaDto request)
        {
            var result = _empresaService.CrearEmpresa(request);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(result.Resultado);
        }

        [HttpPut("{idEmpresa}")]
        public IActionResult Actualizar(int idEmpresa, [FromBody] EmpresaDto request)
        {
            var result = _empresaService.ActualizarEmpresa(idEmpresa, request);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(result.Resultado);
        }

        [HttpDelete("{idEmpresa}")]
        public IActionResult Eliminar(int idEmpresa)
        {
            var result = _empresaService.EliminarEmpresa(idEmpresa);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(new { eliminado = result.Resultado });
        }
    }
}
