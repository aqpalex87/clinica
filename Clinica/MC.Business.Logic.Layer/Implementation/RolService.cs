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
    public class RolService : IRolService
    {
        private readonly IRolRepository _rolRepository;

        public RolService(IRolRepository rolRepository)
        {
            _rolRepository = rolRepository;
        }

        public Result<RolPaginadoDto> ObtenerRolesPaginado(int? idEmpresa, string nombre, int pageNumber, int pageSize)
        {
            Result<RolPaginadoDto> result = new();
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return result.BadRequest("Parámetros de paginación inválidos.");
            }

            var (roles, totalRows) = _rolRepository.ObtenerRolesPaginado(idEmpresa, nombre, pageNumber, pageSize);
            result.Status = HttpStatusCode.OK;
            result.Resultado = new RolPaginadoDto
            {
                TotalRows = totalRows,
                Items = roles.Select(MapToDto).ToList()
            };
            return result;
        }

        public Result<RolDto> CrearRol(RolDto request)
        {
            Result<RolDto> result = new();
            if (!Validar(request, out string mensaje))
            {
                return result.BadRequest(mensaje);
            }

            Rol entity = MapToEntity(request);
            Rol created = _rolRepository.CrearRol(entity);
            result.Status = HttpStatusCode.OK;
            result.Resultado = MapToDto(created);
            return result;
        }

        public Result<RolDto> ActualizarRol(int idRol, RolDto request)
        {
            Result<RolDto> result = new();
            if (idRol <= 0)
            {
                return result.BadRequest("IdRol inválido.");
            }
            if (!Validar(request, out string mensaje))
            {
                return result.BadRequest(mensaje);
            }

            Rol entity = MapToEntity(request);
            entity.IdRol = idRol;
            bool ok = _rolRepository.ActualizarRol(entity);
            if (!ok)
            {
                return result.NotFound("No se encontró el rol para actualizar.");
            }
            result.Status = HttpStatusCode.OK;
            result.Resultado = MapToDto(entity);
            return result;
        }

        public Result<bool> EliminarRol(int idRol)
        {
            Result<bool> result = new();
            if (idRol <= 0)
            {
                return result.BadRequest("IdRol inválido.");
            }
            bool ok = _rolRepository.EliminarRol(idRol);
            if (!ok)
            {
                return result.NotFound("No se encontró el rol para eliminar.");
            }
            result.Status = HttpStatusCode.OK;
            result.Resultado = true;
            return result;
        }

        private static bool Validar(RolDto request, out string mensaje)
        {
            mensaje = string.Empty;
            if (request == null)
            {
                mensaje = "Datos requeridos.";
                return false;
            }
            if (request.IdEmpresa <= 0)
            {
                mensaje = "Empresa requerida.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                mensaje = "Nombre del rol requerido.";
                return false;
            }
            return true;
        }

        private static RolDto MapToDto(Rol item)
        {
            return new RolDto
            {
                IdRol = item.IdRol,
                IdEmpresa = item.IdEmpresa,
                NombreEmpresa = item.NombreEmpresa,
                Nombre = item.Nombre,
                Descripcion = item.Descripcion,
                Activo = item.Activo
            };
        }

        private static Rol MapToEntity(RolDto item)
        {
            return new Rol
            {
                IdRol = item.IdRol,
                IdEmpresa = item.IdEmpresa,
                NombreEmpresa = item.NombreEmpresa,
                Nombre = item.Nombre?.Trim(),
                Descripcion = item.Descripcion?.Trim(),
                Activo = item.Activo
            };
        }
    }
}
