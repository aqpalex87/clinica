using MC.Business.Entity.Layer;
using MC.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static MC.Control.Util.Constants;

namespace MC.API.Controllers
{
    public class CustomControllerBase : ControllerBase
    {
        public UserJwt UserJwt
        {
            get
            {
                UserJwt userJWT = new();
                userJWT.Perfil = User.Claims.FirstOrDefault(claim => claim.Type == ConstantesUsuario.Perfil)?.Value;
                userJWT.NroDocumento = User.Claims.FirstOrDefault(claim => claim.Type == ConstantesUsuario.NumeroDocumento)?.Value;
                userJWT.Nombre = User.Claims.FirstOrDefault(claim => claim.Type == ConstantesUsuario.NombreUsuario)?.Value;
                userJWT.CodUsuario = User.Claims.FirstOrDefault(claim => claim.Type == ConstantesUsuario.CodigoUsuario)?.Value;
                userJWT.IdRol = ParseIntClaim(ConstantesUsuario.IdRol);
                userJWT.IdEmpresa = ParseIntClaim(ConstantesUsuario.IdEmpresa);
                userJWT.IdSede = ParseIntClaim(ConstantesUsuario.IdSede);
                return userJWT;
            }

        }

        private int? ParseIntClaim(string claimType)
        {
            string claimValue = User.Claims.FirstOrDefault(claim => claim.Type == claimType)?.Value;
            if (int.TryParse(claimValue, out int value))
            {
                return value;
            }
            return null;
        }

        public ObjectResult GetObjectResult<T>(Result<T> result)
        {
            return StatusCode((int)result.Status, new ValidationProblemDetails(result.Errors));
        }

        protected bool TryEmpresaSesion(out int idEmpresa, out IActionResult error)
        {
            idEmpresa = UserJwt.IdEmpresa ?? 0;
            if (idEmpresa > 0)
            {
                error = null;
                return true;
            }

            error = SesionInvalida("La sesión no tiene una empresa. Vuelva a iniciar sesión.");
            return false;
        }

        protected bool TrySedeSesion(out int idSede, out IActionResult error)
        {
            idSede = UserJwt.IdSede ?? 0;
            if (idSede > 0)
            {
                error = null;
                return true;
            }

            error = SesionInvalida("La sesión no tiene una sede. Vuelva a iniciar sesión.");
            return false;
        }

        private IActionResult SesionInvalida(string mensaje)
        {
            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                ["error"] = new[] { mensaje }
            }));
        }

    }
}
