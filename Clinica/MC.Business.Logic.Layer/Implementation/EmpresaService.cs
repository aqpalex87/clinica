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
    public class EmpresaService : IEmpresaService
    {
        private readonly IEmpresaRepository _empresaRepository;

        public EmpresaService(IEmpresaRepository empresaRepository)
        {
            _empresaRepository = empresaRepository;
        }

        public Result<EmpresaPaginadoDto> ObtenerEmpresasPaginado(string nombre, int pageNumber, int pageSize)
        {
            Result<EmpresaPaginadoDto> result = new();
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return result.BadRequest("Parámetros de paginación inválidos.");
            }

            var (empresas, totalRows) = _empresaRepository.ObtenerEmpresasPaginado(nombre, pageNumber, pageSize);
            result.Status = HttpStatusCode.OK;
            result.Resultado = new EmpresaPaginadoDto
            {
                TotalRows = totalRows,
                Items = empresas.Select(MapToDto).ToList()
            };
            return result;
        }

        public Result<EmpresaDto> CrearEmpresa(EmpresaDto request)
        {
            Result<EmpresaDto> result = new();
            if (!Validar(request, out string mensaje))
            {
                return result.BadRequest(mensaje);
            }

            Empresa entity = MapToEntity(request);
            Empresa created = _empresaRepository.CrearEmpresa(entity);
            result.Status = HttpStatusCode.OK;
            result.Resultado = MapToDto(created);
            return result;
        }

        public Result<EmpresaDto> ActualizarEmpresa(int idEmpresa, EmpresaDto request)
        {
            Result<EmpresaDto> result = new();
            if (idEmpresa <= 0)
            {
                return result.BadRequest("IdEmpresa inválido.");
            }
            if (!Validar(request, out string mensaje))
            {
                return result.BadRequest(mensaje);
            }

            Empresa entity = MapToEntity(request);
            entity.IdEmpresa = idEmpresa;
            bool ok = _empresaRepository.ActualizarEmpresa(entity);
            if (!ok)
            {
                return result.NotFound("No se encontró la empresa para actualizar.");
            }
            result.Status = HttpStatusCode.OK;
            result.Resultado = MapToDto(entity);
            return result;
        }

        public Result<bool> EliminarEmpresa(int idEmpresa)
        {
            Result<bool> result = new();
            if (idEmpresa <= 0)
            {
                return result.BadRequest("IdEmpresa inválido.");
            }
            bool ok = _empresaRepository.EliminarEmpresa(idEmpresa);
            if (!ok)
            {
                return result.NotFound("No se encontró la empresa para eliminar.");
            }
            result.Status = HttpStatusCode.OK;
            result.Resultado = true;
            return result;
        }

        private static bool Validar(EmpresaDto request, out string mensaje)
        {
            mensaje = string.Empty;
            if (request == null)
            {
                mensaje = "Datos requeridos.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                mensaje = "Nombre de empresa requerido.";
                return false;
            }
            return true;
        }

        private static EmpresaDto MapToDto(Empresa item)
        {
            return new EmpresaDto
            {
                IdEmpresa = item.IdEmpresa,
                Nombre = item.Nombre,
                Ruc = item.Ruc,
                Activo = item.Activo
            };
        }

        private static Empresa MapToEntity(EmpresaDto item)
        {
            return new Empresa
            {
                IdEmpresa = item.IdEmpresa,
                Nombre = item.Nombre?.Trim(),
                Ruc = item.Ruc?.Trim(),
                Activo = item.Activo
            };
        }
    }
}
