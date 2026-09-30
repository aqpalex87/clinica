using MC.Business.Entity.Layer;
using MC.DTO;
using AutoMapper;

namespace MC.Business.Logic.Layer.Profiles
{
    public class ClienteProfile : Profile
    {
        public ClienteProfile()
        {
            CreateMap<ClienteDto, Cliente>().ReverseMap();
        }
    }
}
