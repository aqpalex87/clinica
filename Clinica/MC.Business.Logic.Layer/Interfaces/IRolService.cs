using MC.DTO;
using MC.DTO.Dtos;

namespace MC.Business.Logic.Layer.Interfaces
{
    public interface IRolService
    {
        Result<RolPaginadoDto> ObtenerRolesPaginado(int? idEmpresa, string nombre, int pageNumber, int pageSize);
        Result<RolDto> CrearRol(RolDto request);
        Result<RolDto> ActualizarRol(int idRol, RolDto request);
        Result<bool> EliminarRol(int idRol);
    }
}
