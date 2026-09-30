using MC.Business.Entity.Layer.Entities;
using MC.Business.Logic.Layer.Interfaces;
using MC.Data.Access.Layer.Interfaces;
using MC.DTO;
using MC.DTO.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace MC.Business.Logic.Layer.Implementation
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public Result<MenuPaginadoDto> ObtenerMenusPaginado(string nombre, int pageNumber, int pageSize)
        {
            Result<MenuPaginadoDto> result = new();
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return result.BadRequest("Parámetros de paginación inválidos.");
            }

            var (menus, totalRows) = _menuRepository.ObtenerMenusPaginado(nombre, pageNumber, pageSize);
            result.Status = HttpStatusCode.OK;
            result.Resultado = new MenuPaginadoDto
            {
                TotalRows = totalRows,
                Items = menus.Select(MapToDto).ToList()
            };
            return result;
        }

        public Result<List<MenuCrudDto>> ObtenerMenusPadre()
        {
            Result<List<MenuCrudDto>> result = new();
            List<MenuRol> menus = _menuRepository.ObtenerMenusPadre();
            result.Status = HttpStatusCode.OK;
            result.Resultado = menus.Select(MapToDto).ToList();
            return result;
        }

        public Result<MenuCrudDto> CrearMenu(MenuCrudDto request, int? idRol)
        {
            Result<MenuCrudDto> result = new();
            if (!ValidarMenu(request, out string mensaje))
            {
                return result.BadRequest(mensaje);
            }

            MenuRol entity = MapToEntity(request);
            MenuRol creado = _menuRepository.CrearMenu(entity);

            if (idRol.HasValue && idRol.Value > 0)
            {
                _menuRepository.AsignarMenuARol(idRol.Value, creado.IdMenu);
            }

            result.Status = HttpStatusCode.OK;
            result.Resultado = MapToDto(creado);
            return result;
        }

        public Result<MenuCrudDto> ActualizarMenu(int idMenu, MenuCrudDto request)
        {
            Result<MenuCrudDto> result = new();
            if (idMenu <= 0)
            {
                return result.BadRequest("Id de menú inválido.");
            }

            if (!ValidarMenu(request, out string mensaje))
            {
                return result.BadRequest(mensaje);
            }

            MenuRol entity = MapToEntity(request);
            entity.IdMenu = idMenu;
            bool actualizado = _menuRepository.ActualizarMenu(entity);
            if (!actualizado)
            {
                return result.NotFound("No se encontró el menú para actualizar.");
            }

            result.Status = HttpStatusCode.OK;
            result.Resultado = MapToDto(entity);
            return result;
        }

        public Result<bool> EliminarMenu(int idMenu)
        {
            Result<bool> result = new();
            if (idMenu <= 0)
            {
                return result.BadRequest("Id de menú inválido.");
            }

            bool eliminado = _menuRepository.EliminarMenu(idMenu);
            if (!eliminado)
            {
                return result.NotFound("No se encontró el menú para eliminar.");
            }

            result.Status = HttpStatusCode.OK;
            result.Resultado = true;
            return result;
        }

        private static bool ValidarMenu(MenuCrudDto request, out string mensaje)
        {
            mensaje = string.Empty;
            if (request == null)
            {
                mensaje = "Datos de menú requeridos.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                mensaje = "Nombre es requerido.";
                return false;
            }

            if (request.IdPadre.HasValue && request.IdPadre.Value == request.IdMenu)
            {
                mensaje = "Un menú no puede ser padre de sí mismo.";
                return false;
            }

            return true;
        }

        private static MenuCrudDto MapToDto(MenuRol item)
        {
            return new MenuCrudDto
            {
                IdMenu = item.IdMenu,
                Nombre = item.Nombre,
                Ruta = item.Ruta,
                Icono = item.Icono,
                IdPadre = item.IdPadre,
                NombrePadre = item.NombrePadre,
                Orden = item.Orden,
                Activo = item.Activo
            };
        }

        private static MenuRol MapToEntity(MenuCrudDto item)
        {
            return new MenuRol
            {
                IdMenu = item.IdMenu,
                Nombre = item.Nombre?.Trim(),
                Ruta = item.Ruta?.Trim(),
                Icono = item.Icono?.Trim(),
                IdPadre = item.IdPadre,
                Orden = item.Orden,
                Activo = item.Activo
            };
        }
    }
}
