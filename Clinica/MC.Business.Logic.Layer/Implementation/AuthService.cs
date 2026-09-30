using MC.Business.Entity.Layer.Entities;
using MC.Business.Logic.Layer.Interfaces;
using MC.Control.DTO;
using MC.Control.Interface;
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
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IAccessControl _accessControl;

        public AuthService(IAuthRepository authRepository, IAccessControl accessControl)
        {
            _authRepository = authRepository;
            _accessControl = accessControl;
        }

        public Result<AuthLoginResponseDto> Login(AuthLoginRequestDto request)
        {
            Result<AuthLoginResponseDto> result = new();

            if (request == null || string.IsNullOrWhiteSpace(request.Usuario) || string.IsNullOrWhiteSpace(request.Password))
            {
                return result.BadRequest("Usuario y contraseña son requeridos.");
            }

            UsuarioAuth usuario = _authRepository.ObtenerUsuario(request.Usuario.Trim());
            if (usuario == null)
            {
                return result.BadRequest("Usuario no existe.");
            }

            if (!usuario.Activo)
            {
                return result.BadRequest("Usuario inactivo.");
            }

            if (!ValidarPassword(request.Password, usuario.PasswordHash))
            {
                return result.BadRequest("Credenciales inválidas.");
            }

            AccessDto accessDto = new()
            {
                CodigoUsuario = usuario.Usuario,
                CorreoElectronico = $"{usuario.Usuario}@local",
                NombreUsuario = usuario.Usuario,
                NumeroDocumento = usuario.Usuario,
                Perfil = usuario.RolNombre,
                IdRol = usuario.IdRol,
                IdEmpresa = usuario.IdEmpresa,
                IdSede = usuario.IdSede
            };

            var (tokenResponse, token) = _accessControl.GenerateToken(accessDto);

            result.Status = HttpStatusCode.OK;
            result.Resultado = new AuthLoginResponseDto
            {
                IdUsuario = usuario.IdUsuario,
                Usuario = usuario.Usuario,
                IdEmpresa = usuario.IdEmpresa,
                Empresa = usuario.EmpresaNombre,
                IdSede = usuario.IdSede,
                Sede = usuario.SedeNombre,
                IdRol = usuario.IdRol,
                Rol = usuario.RolNombre,
                Token = $"Bearer {token}",
                FechaInicioVigencia = tokenResponse.FechaInicioVigencia,
                FechaFinVigencia = tokenResponse.FechaFinVigencia
            };
            return result;
        }

        public Result<AuthLoginResponseDto> SeleccionarSede(string usuario, int idEmpresa, int idSede, int idRol, string rolNombre)
        {
            Result<AuthLoginResponseDto> result = new();
            if (string.IsNullOrWhiteSpace(usuario))
            {
                return result.BadRequest("Usuario no identificado.");
            }
            if (idEmpresa <= 0)
            {
                return result.BadRequest("Empresa inválida.");
            }
            if (idSede <= 0)
            {
                return result.BadRequest("Sede inválida.");
            }

            // Validar que la sede pertenezca a la empresa
            var sedes = _authRepository.ObtenerSedesPorEmpresa(idEmpresa) ?? new List<SedeAuth>();
            var sede = sedes.FirstOrDefault(x => x.IdSede == idSede);
            if (sede == null)
            {
                return result.BadRequest("La sede seleccionada no pertenece a la empresa.");
            }

            AccessDto accessDto = new()
            {
                CodigoUsuario = usuario.Trim(),
                CorreoElectronico = $"{usuario.Trim()}@local",
                NombreUsuario = usuario.Trim(),
                NumeroDocumento = usuario.Trim(),
                Perfil = string.IsNullOrWhiteSpace(rolNombre) ? "Usuario" : rolNombre,
                IdRol = idRol > 0 ? idRol : null,
                IdEmpresa = idEmpresa,
                IdSede = idSede
            };

            var (tokenResponse, token) = _accessControl.GenerateToken(accessDto);

            result.Status = HttpStatusCode.OK;
            result.Resultado = new AuthLoginResponseDto
            {
                IdUsuario = 0,
                Usuario = usuario.Trim(),
                IdEmpresa = idEmpresa,
                Empresa = string.Empty,
                IdSede = idSede,
                Sede = sede.Nombre,
                IdRol = idRol,
                Rol = accessDto.Perfil,
                Token = $"Bearer {token}",
                FechaInicioVigencia = tokenResponse.FechaInicioVigencia,
                FechaFinVigencia = tokenResponse.FechaFinVigencia
            };
            return result;
        }

        public Result<List<MenuDto>> ObtenerMenusPorUsuario(string usuario, int? idEmpresa, int? idSede)
        {
            Result<List<MenuDto>> result = new();
            if (string.IsNullOrWhiteSpace(usuario))
            {
                return result.BadRequest("Usuario no identificado.");
            }

            List<MenuRol> menusFlat = _authRepository.ObtenerMenusPorUsuario(usuario.Trim(), idEmpresa, idSede);
            List<MenuDto> menuTree = ConstruirArbol(menusFlat);

            result.Status = HttpStatusCode.OK;
            result.Resultado = menuTree;
            return result;
        }

        public Result<List<EmpresaAuthDto>> ObtenerEmpresas()
        {
            Result<List<EmpresaAuthDto>> result = new();
            var empresas = _authRepository.ObtenerEmpresas()
                .Select(x => new EmpresaAuthDto
                {
                    IdEmpresa = x.IdEmpresa,
                    Nombre = x.Nombre
                })
                .ToList();
            result.Status = HttpStatusCode.OK;
            result.Resultado = empresas;
            return result;
        }

        public Result<List<SedeAuthDto>> ObtenerSedesPorEmpresa(int idEmpresa)
        {
            Result<List<SedeAuthDto>> result = new();
            if (idEmpresa <= 0)
            {
                return result.BadRequest("Empresa inválida.");
            }

            var sedes = _authRepository.ObtenerSedesPorEmpresa(idEmpresa)
                .Select(x => new SedeAuthDto
                {
                    IdSede = x.IdSede,
                    IdEmpresa = x.IdEmpresa,
                    Nombre = x.Nombre
                })
                .ToList();
            result.Status = HttpStatusCode.OK;
            result.Resultado = sedes;
            return result;
        }

        private static bool ValidarPassword(string passwordIngresada, string passwordHashGuardada)
        {
            if (string.IsNullOrWhiteSpace(passwordHashGuardada))
            {
                return false;
            }

            if (passwordIngresada == passwordHashGuardada)
            {
                return true;
            }

            string sha256 = ObtenerSha256(passwordIngresada);
            return sha256.Equals(passwordHashGuardada, StringComparison.OrdinalIgnoreCase);
        }

        private static string ObtenerSha256(string texto)
        {
            using SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(texto));
            StringBuilder builder = new();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        private static List<MenuDto> ConstruirArbol(List<MenuRol> menusFlat)
        {
            Dictionary<int, MenuDto> menus = menusFlat
                .GroupBy(x => x.IdMenu)
                .Select(g => g.First())
                .Select(x => new MenuDto
                {
                    IdMenu = x.IdMenu,
                    Nombre = x.Nombre,
                    Ruta = x.Ruta,
                    Icono = x.Icono,
                    IdPadre = x.IdPadre,
                    Orden = x.Orden
                })
                .ToDictionary(x => x.IdMenu, x => x);

            foreach (MenuDto menu in menus.Values)
            {
                if (menu.IdPadre.HasValue && menus.TryGetValue(menu.IdPadre.Value, out MenuDto padre))
                {
                    padre.Hijos.Add(menu);
                }
            }

            foreach (MenuDto menu in menus.Values)
            {
                menu.Hijos = menu.Hijos
                    .OrderBy(x => x.Orden ?? int.MaxValue)
                    .ThenBy(x => x.IdMenu)
                    .ToList();
            }

            return menus.Values
                .Where(x => !x.IdPadre.HasValue || !menus.ContainsKey(x.IdPadre.Value))
                .OrderBy(x => x.Orden ?? int.MaxValue)
                .ThenBy(x => x.IdMenu)
                .ToList();
        }
    }
}
