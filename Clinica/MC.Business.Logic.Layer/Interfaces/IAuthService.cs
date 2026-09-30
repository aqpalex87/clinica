using MC.DTO;
using MC.DTO.Dtos;
using System.Collections.Generic;

namespace MC.Business.Logic.Layer.Interfaces
{
    public interface IAuthService
    {
        Result<AuthLoginResponseDto> Login(AuthLoginRequestDto request);
        Result<AuthLoginResponseDto> SeleccionarSede(string usuario, int idEmpresa, int idSede, int idRol, string rolNombre);
        Result<List<MenuDto>> ObtenerMenusPorUsuario(string usuario, int? idEmpresa, int? idSede);
        Result<List<EmpresaAuthDto>> ObtenerEmpresas();
        Result<List<SedeAuthDto>> ObtenerSedesPorEmpresa(int idEmpresa);
    }
}
