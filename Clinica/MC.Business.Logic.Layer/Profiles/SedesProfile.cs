using AutoMapper;
using MC.Business.Entity.Layer.Entities;
using MC.DTO.Dtos;

namespace MC.Business.Logic.Layer.Profiles
{
    public class SedesProfile : Profile
    {
        public SedesProfile()
        {
            CreateMap<Sedes, SedesDto>();
            CreateMap<SedesDto, Sedes>();
        }
    }
}
