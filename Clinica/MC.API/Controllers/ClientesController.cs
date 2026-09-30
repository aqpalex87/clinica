using MC.Business.Logic.Layer.Interfaces;
using MC.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientesController : CustomControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpPost]
        public IActionResult BusquedaClientes(ClienteDto filtroDto)
        {
            var Clientes = _clienteService.GetClientesByFiltro(filtroDto);
            return Ok(Clientes);
        }
    }
}
