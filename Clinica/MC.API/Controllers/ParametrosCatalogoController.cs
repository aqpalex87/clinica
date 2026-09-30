using MC.Business.Logic.Layer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace MC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ParametrosCatalogoController : CustomControllerBase
    {
        private readonly IParametrosMaestroService _parametrosMaestroService;

        public ParametrosCatalogoController(IParametrosMaestroService parametrosMaestroService)
        {
            _parametrosMaestroService = parametrosMaestroService;
        }

        [HttpGet("por-tipo/{codigoTipo}")]
        public IActionResult ObtenerPorTipo(string codigoTipo)
        {
            var result = _parametrosMaestroService.ObtenerPorTipoCodigo(codigoTipo);
            if (result.Errors.Any())
            {
                return GetObjectResult(result);
            }
            return Ok(result.Resultado);
        }
    }
}
