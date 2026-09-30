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
            if (!TryEmpresaSesion(out int idEmpresaSesion, out IActionResult error))
            {
                return error;
            }
            var result = _sedesService.ObtenerSedesPorEmpresa(idEmpresaSesion);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(result.Resultado);
        }

        [HttpGet]
        public IActionResult ObtenerSedesPaginado([FromQuery] int? idEmpresa, [FromQuery] string nombre = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (!TryEmpresaSesion(out int idEmpresaSesion, out IActionResult error))
            {
                return error;
            }
            var result = _sedesService.ObtenerSedesPaginado(idEmpresaSesion, nombre, pageNumber, pageSize);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(result.Resultado);
        }

        [HttpPost]
        public IActionResult Crear([FromBody] SedesDto request)
        {
            if (!TryEmpresaSesion(out int idEmpresaSesion, out IActionResult error))
            {
                return error;
            }
            request.IdEmpresa = idEmpresaSesion;
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
            if (!TryEmpresaSesion(out int idEmpresaSesion, out IActionResult error))
            {
                return error;
            }
            request.IdEmpresa = idEmpresaSesion;
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
