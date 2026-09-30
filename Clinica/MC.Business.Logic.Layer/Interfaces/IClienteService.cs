using MC.DTO;
using System.Collections.Generic;

namespace MC.Business.Logic.Layer.Interfaces
{
    public interface IClienteService
    {
        List<ClienteDto> GetClientesByFiltro(ClienteDto clienteDto);
    }
}
