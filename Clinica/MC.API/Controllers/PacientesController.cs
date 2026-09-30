using MC.Business.Logic.Layer.Interfaces;
using MC.DTO.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace MC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PacientesController : CustomControllerBase
    {
        private readonly IPacienteService _pacienteService;

        public PacientesController(IPacienteService pacienteService)
        {
            _pacienteService = pacienteService;
        }

        [HttpGet]
        public IActionResult ObtenerPaginado(
            [FromQuery] int? idEmpresa,
            [FromQuery] string nombre = "",
            [FromQuery] string numeroDocumento = "",
            [FromQuery] string contacto = "",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (!TryEmpresaSesion(out int idEmpresaSesion, out IActionResult error))
            {
                return error;
            }
            var result = _pacienteService.ObtenerPacientesPaginado(idEmpresaSesion, nombre, numeroDocumento, contacto, pageNumber, pageSize);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(result.Resultado);
        }

        [HttpPost]
        public IActionResult Crear([FromBody] PacienteDto request)
        {
            if (!AplicarContexto(request, out IActionResult error))
            {
                return error;
            }
            var result = _pacienteService.CrearPaciente(request);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(result.Resultado);
        }

        [HttpPut("{idPaciente}")]
        public IActionResult Actualizar(int idPaciente, [FromBody] PacienteDto request)
        {
            if (!AplicarContexto(request, out IActionResult error))
            {
                return error;
            }
            var result = _pacienteService.ActualizarPaciente(idPaciente, request);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(result.Resultado);
        }

        [HttpDelete("{idPaciente}")]
        public IActionResult Eliminar(int idPaciente)
        {
            var result = _pacienteService.EliminarPaciente(idPaciente);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(new { eliminado = result.Resultado });
        }

        private bool AplicarContexto(PacienteDto request, out IActionResult error)
        {
            if (request == null)
            {
                error = BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    ["error"] = new[] { "Datos requeridos." }
                }));
                return false;
            }
            if (!TryEmpresaSesion(out int idEmpresa, out error))
            {
                return false;
            }
            if (!TrySedeSesion(out int idSede, out error))
            {
                return false;
            }
            request.IdEmpresa = idEmpresa;
            request.IdSede = idSede;
            return true;
        }
    }
}
