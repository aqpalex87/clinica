using MC.DTO;
using MC.DTO.Dtos;
using System.Collections.Generic;

namespace MC.Business.Logic.Layer.Interfaces
{
    public interface IMenuRolService
    {
        Result<List<RolSimpleDto>> ObtenerRolesPorEmpresa(int idEmpresa);
        Result<List<MenuRolTreeDto>> ObtenerArbolMenusPorRol(int idRol);
        Result<bool> GuardarMenusRol(GuardarMenuRolRequestDto request);
    }
}
