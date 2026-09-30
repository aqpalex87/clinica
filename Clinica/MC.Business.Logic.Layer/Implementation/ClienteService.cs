using MC.Business.Entity.Layer;
using MC.Business.Logic.Layer.Interfaces;
using MC.Data.Access.Layer.Interfaces;
using MC.DTO;
using AutoMapper;
using System.Collections.Generic;

namespace MC.Business.Logic.Layer.Implementation
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IMapper _mapper;

        public ClienteService(IClienteRepository ClienteRepository, IMapper mapper)
        {
            _clienteRepository = ClienteRepository;
            _mapper = mapper;
        }

        public List<ClienteDto> GetClientesByFiltro(ClienteDto clienteDto)
        {
            List<Cliente> clientes = null;
            if (!string.IsNullOrWhiteSpace(clienteDto.Nombre))
            {
                clientes = _clienteRepository.GetClienteByNombre($"%{clienteDto.Nombre.ToUpper()}%");
            }
            else if (!string.IsNullOrWhiteSpace(clienteDto.NumDoc))
            {
                clientes = _clienteRepository.GetClienteByNroDoc(clienteDto.NumDoc);
            }
            else if (clienteDto.CodCliente.HasValue)
            {
                clientes = _clienteRepository.GetClienteByCodigo(clienteDto.CodCliente);
            }
            return _mapper.Map<List<ClienteDto>>(clientes);
        }
    }
}
