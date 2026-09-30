using MC.DTO;
using MC.DTO.Dtos;
using System.Collections.Generic;

namespace MC.Business.Logic.Layer.Interfaces
{
    public interface IMenuService
    {
        Result<MenuPaginadoDto> ObtenerMenusPaginado(string nombre, int pageNumber, int pageSize);
        Result<List<MenuCrudDto>> ObtenerMenusPadre();
        Result<MenuCrudDto> CrearMenu(MenuCrudDto request, int? idRol);
        Result<MenuCrudDto> ActualizarMenu(int idMenu, MenuCrudDto request);
        Result<bool> EliminarMenu(int idMenu);
    }
}
