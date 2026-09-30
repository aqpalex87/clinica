using MC.Business.Entity.Layer.Entities;
using MC.Business.Logic.Layer.Interfaces;
using MC.Data.Access.Layer.Interfaces;
using MC.DTO;
using MC.DTO.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace MC.Business.Logic.Layer.Implementation
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public Result<UsuarioPaginadoDto> ObtenerUsuariosPaginado(int? idEmpresa, string usuario, int pageNumber, int pageSize)
        {
            Result<UsuarioPaginadoDto> result = new();
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return result.BadRequest("Parámetros de paginación inválidos.");
            }

            var (usuarios, totalRows) = _usuarioRepository.ObtenerUsuariosPaginado(idEmpresa, usuario, pageNumber, pageSize);
            result.Status = HttpStatusCode.OK;
            result.Resultado = new UsuarioPaginadoDto
            {
                TotalRows = totalRows,
                Items = usuarios.Select(MapToDto).ToList()
            };
            return result;
        }

        public Result<UsuarioDto> CrearUsuario(UsuarioDto request)
        {
            Result<UsuarioDto> result = new();
            if (!Validar(request, true, out string mensaje))
            {
                return result.BadRequest(mensaje);
            }

            Usuario entity = MapToEntity(request, true);
            Usuario created = _usuarioRepository.CrearUsuario(entity);
            result.Status = HttpStatusCode.OK;
            result.Resultado = MapToDto(created);
            return result;
        }

        public Result<UsuarioDto> ActualizarUsuario(int idUsuario, UsuarioDto request)
        {
            Result<UsuarioDto> result = new();
            if (idUsuario <= 0)
            {
                return result.BadRequest("IdUsuario inválido.");
            }
            if (!Validar(request, false, out string mensaje))
            {
                return result.BadRequest(mensaje);
            }

            Usuario entity = MapToEntity(request, false);
            entity.IdUsuario = idUsuario;
            bool ok = _usuarioRepository.ActualizarUsuario(entity);
            if (!ok)
            {
                return result.NotFound("No se encontró el usuario para actualizar.");
            }
            result.Status = HttpStatusCode.OK;
            result.Resultado = MapToDto(entity);
            return result;
        }

        public Result<bool> EliminarUsuario(int idUsuario)
        {
            Result<bool> result = new();
            if (idUsuario <= 0)
            {
                return result.BadRequest("IdUsuario inválido.");
            }
            bool ok = _usuarioRepository.EliminarUsuario(idUsuario);
            if (!ok)
            {
                return result.NotFound("No se encontró el usuario para eliminar.");
            }
            result.Status = HttpStatusCode.OK;
            result.Resultado = true;
            return result;
        }

        private static bool Validar(UsuarioDto request, bool isCreate, out string mensaje)
        {
            mensaje = string.Empty;
            if (request == null)
            {
                mensaje = "Datos requeridos.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(request.UsuarioLogin))
            {
                mensaje = "Usuario es requerido.";
                return false;
            }
            if (request.IdEmpresa <= 0)
            {
                mensaje = "Empresa es requerida.";
                return false;
            }
            if (request.IdRol <= 0)
            {
                mensaje = "Rol es requerido.";
                return false;
            }
            if (isCreate && string.IsNullOrWhiteSpace(request.Password))
            {
                mensaje = "Contraseña es requerida.";
                return false;
            }
            return true;
        }

        private static UsuarioDto MapToDto(Usuario item)
        {
            return new UsuarioDto
            {
                IdUsuario = item.IdUsuario,
                UsuarioLogin = item.UsuarioLogin,
                IdEmpresa = item.IdEmpresa,
                NombreEmpresa = item.NombreEmpresa,
                IdRol = item.IdRol,
                NombreRol = item.NombreRol,
                Activo = item.Activo
            };
        }

        private static Usuario MapToEntity(UsuarioDto item, bool isCreate)
        {
            return new Usuario
            {
                IdUsuario = item.IdUsuario,
                UsuarioLogin = item.UsuarioLogin?.Trim(),
                PasswordHash = ResolvePasswordHash(item.Password, isCreate),
                IdEmpresa = item.IdEmpresa,
                NombreEmpresa = item.NombreEmpresa,
                IdRol = item.IdRol,
                NombreRol = item.NombreRol,
                Activo = item.Activo
            };
        }

        private static string ResolvePasswordHash(string password, bool isCreate)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return isCreate ? string.Empty : null;
            }

            using SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
