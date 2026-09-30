using MC.Business.Entity.Layer.Entities;
using MC.Business.Logic.Layer.Interfaces;
using MC.Data.Access.Layer.Interfaces;
using MC.DTO;
using MC.DTO.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace MC.Business.Logic.Layer.Implementation
{
    public class MenuRolService : IMenuRolService
    {
        private readonly IRolRepository _rolRepository;
        private readonly IMenuRepository _menuRepository;

        public MenuRolService(IRolRepository rolRepository, IMenuRepository menuRepository)
        {
            _rolRepository = rolRepository;
            _menuRepository = menuRepository;
        }

        public Result<List<RolSimpleDto>> ObtenerRolesPorEmpresa(int idEmpresa)
        {
            Result<List<RolSimpleDto>> result = new();
            if (idEmpresa <= 0)
            {
                return result.BadRequest("IdEmpresa inválido.");
            }

            var roles = _rolRepository.ObtenerRolesPorEmpresa(idEmpresa);
            result.Status = HttpStatusCode.OK;
            result.Resultado = roles.Select(x => new RolSimpleDto
            {
                IdRol = x.IdRol,
                Nombre = x.Nombre
            }).ToList();
            return result;
        }

        public Result<List<MenuRolTreeDto>> ObtenerArbolMenusPorRol(int idRol)
        {
            Result<List<MenuRolTreeDto>> result = new();
            if (idRol <= 0)
            {
                return result.BadRequest("IdRol inválido.");
            }

            var menus = _menuRepository.ObtenerMenusPorRol(idRol);
            result.Status = HttpStatusCode.OK;
            result.Resultado = ConstruirArbol(menus, null);
            return result;
        }

        public Result<bool> GuardarMenusRol(GuardarMenuRolRequestDto request)
        {
            Result<bool> result = new();
            if (request == null || request.IdRol <= 0)
            {
                return result.BadRequest("Datos inválidos para guardar.");
            }

            var ids = (request.IdsMenu ?? new List<int>()).Distinct().Where(x => x > 0).ToList();
            string csv = string.Join(",", ids);
            bool ok = _menuRepository.GuardarMenusPorRol(request.IdRol, csv);
            if (!ok)
            {
                return result.BadRequest("No se pudo guardar la configuración de menús.");
            }

            result.Status = HttpStatusCode.OK;
            result.Resultado = true;
            return result;
        }

        private static List<MenuRolTreeDto> ConstruirArbol(List<MenuRol> source, int? idPadre)
        {
            return source
                .Where(x => x.IdPadre == idPadre)
                .OrderBy(x => x.Orden ?? 0)
                .ThenBy(x => x.IdMenu)
                .Select(x => new MenuRolTreeDto
                {
                    IdMenu = x.IdMenu,
                    Nombre = x.Nombre,
                    IdPadre = x.IdPadre,
                    Seleccionado = x.Seleccionado,
                    Hijos = ConstruirArbol(source, x.IdMenu)
                })
                .ToList();
        }
    }
}
