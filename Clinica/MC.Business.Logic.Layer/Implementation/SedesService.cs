using AutoMapper;
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
    public class SedesService : ISedesService
    {
        private readonly ISedesRepository _sedesRepository;
        private readonly IMapper _mapper;

        public SedesService(ISedesRepository sedesRepository,IMapper mapper)
        {
            _sedesRepository = sedesRepository;
            _mapper = mapper;
        }

        public Result<List<SedesDto>> ObtenerSedesPorEmpresa(int idEmpresa)
        {
            Result<List<SedesDto>> result = new();
            if (idEmpresa <= 0)
            {
                return result.BadRequest("idEmpresa es requerido.");
            }

            var sedes = _sedesRepository.ObtenerSedesPorEmpresa(idEmpresa);
            result.Status = HttpStatusCode.OK;
            result.Resultado = _mapper.Map<List<SedesDto>>(sedes);
            return result;
        }

        public Result<SedesPaginadoDto> ObtenerSedesPaginado(int? idEmpresa, string nombre, int pageNumber, int pageSize)
        {
            Result<SedesPaginadoDto> result = new();
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return result.BadRequest("Parámetros inválidos.");
            }

            var (sedes, totalRows) = _sedesRepository.ObtenerSedesPaginado(idEmpresa, nombre, pageNumber, pageSize);
            result.Status = HttpStatusCode.OK;
            result.Resultado = new SedesPaginadoDto
            {
                TotalRows = totalRows,
                Items = _mapper.Map<List<SedesDto>>(sedes)
            };
            return result;
        }

        public Result<SedesDto> CrearSede(SedesDto request)
        {
            Result<SedesDto> result = new();
            if (!Validar(request, out string mensaje))
            {
                return result.BadRequest(mensaje);
            }

            Sedes entity = _mapper.Map<Sedes>(request);
            Sedes created = _sedesRepository.CrearSede(entity);
            result.Status = HttpStatusCode.OK;
            result.Resultado = _mapper.Map<SedesDto>(created);
            return result;
        }

        public Result<SedesDto> ActualizarSede(int idSede, SedesDto request)
        {
            Result<SedesDto> result = new();
            if (idSede <= 0)
            {
                return result.BadRequest("IdSede inválido.");
            }
            if (!Validar(request, out string mensaje))
            {
                return result.BadRequest(mensaje);
            }

            Sedes entity = _mapper.Map<Sedes>(request);
            entity.IdSede = idSede;
            bool ok = _sedesRepository.ActualizarSede(entity);
            if (!ok)
            {
                return result.NotFound("No se encontró la sede para actualizar.");
            }

            result.Status = HttpStatusCode.OK;
            result.Resultado = _mapper.Map<SedesDto>(entity);
            return result;
        }

        public Result<bool> EliminarSede(int idSede)
        {
            Result<bool> result = new();
            if (idSede <= 0)
            {
                return result.BadRequest("IdSede inválido.");
            }
            bool ok = _sedesRepository.EliminarSede(idSede);
            if (!ok)
            {
                return result.NotFound("No se encontró la sede para eliminar.");
            }
            result.Status = HttpStatusCode.OK;
            result.Resultado = true;
            return result;
        }

        private static bool Validar(SedesDto request, out string mensaje)
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
                mensaje = "Nombre de sede requerido.";
                return false;
            }
            return true;
        }
    }
}
