using MC.Business.Logic.Layer.Interfaces;
using MC.DTO.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace MC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]

 
    public class SedesController : CustomControllerBase
    {
        private readonly ISedesService _sedesService;

        public SedesController(ISedesService sedesService)
        {
            _sedesService = sedesService;
        }
        [HttpGet("empresa/{idEmpresa}")]
        public IActionResult ObtenerSedesPorEmpresa(int idEmpresa)
        {
            var result = _sedesService.ObtenerSedesPorEmpresa(idEmpresa);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(result.Resultado);
        }

        [HttpGet]
        public IActionResult ObtenerSedesPaginado([FromQuery] int? idEmpresa, [FromQuery] string nombre = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = _sedesService.ObtenerSedesPaginado(idEmpresa, nombre, pageNumber, pageSize);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(result.Resultado);
        }

        [HttpPost]
        public IActionResult Crear([FromBody] SedesDto request)
        {
            var result = _sedesService.CrearSede(request);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(result.Resultado);
        }

        [HttpPut("{idSede}")]
        public IActionResult Actualizar(int idSede, [FromBody] SedesDto request)
        {
            var result = _sedesService.ActualizarSede(idSede, request);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(result.Resultado);
        }

        [HttpDelete("{idSede}")]
        public IActionResult Eliminar(int idSede)
        {
            var result = _sedesService.EliminarSede(idSede);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(new { eliminado = result.Resultado });
        }
    }
}
