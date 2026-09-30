using MC.API.Models;
using MC.Control.DTO;
using MC.Control.Filter;
using MC.Control.Interface;
using SAR.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using MC.Control.Implementation;

namespace MC.API.Controllers
{
    [AllowAnonymous]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("api/acceso")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly IAccessControl _accesscontrol;
        public AccessController(IAccessControl accessControl)
        {
            _accesscontrol = accessControl;
        }

        [Required]
        [FromHeader(Name = "X-AppKey")]
        public string AppKey { get; set; }
        [Required]
        [FromHeader(Name = "X-AppCode")]
        public string AppCode { get; set; }
        #region Access 

        /// <summary>
        /// Método para autenticar al usuario y generar token de sesión
        /// </summary>
        /// <param name="appkey">AppKey de aplicación</param>
        /// <param name="appcode">AppCode de aplicación</param>
        /// <param name="access">Objeto con correo, nombre y codigo del usuario</param>
        /// <response code="200">Ok - Retorna objeto con los datos del token</response> 
        /// <response code="400">Bad Request - Solicitud Errada o contenido incorrecto</response> 
        /// <response code="500">Server Error - Errores no controlados</response>  
        /// <response code="502">Bad Gateway - Servidor remoto sin conexión o no disponible</response>  
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(AuthAccessResponse))]
        [ProducesResponseType(400, Type = typeof(ResponseError))]
        [ProducesResponseType(500, Type = typeof(ResponseError))]
        [ProducesResponseType(502, Type = typeof(ResponseError))]
        [ValidateAppHeadersRequest]
        public object GenerarToken([Required][FromBody] AuthAccessRequest access)
        {
            var authAccessdto = new AccessDto
            {
                CorreoElectronico = access.CorreoElectronico,
                CodigoUsuario = access.CodigoUsuario,
                NombreUsuario = access.NombreUsuario,
                NumeroDocumento = access.NumeroDocumento,
                Perfil = access.Perfil
            };

            var (result, token) = _accesscontrol.GenerateToken(authAccessdto);
            AuthAccessResponseData dataResponse = new()
            {
                FechaInicioVigencia = result.FechaInicioVigencia,
                FechaFinVigencia = result.FechaFinVigencia,
                NombreUsuario = result.NombreUsuario,
                CodigoUsuario = result.CodigoUsuario,
                CorreoElectronico = result.CorreoElectronico,
                Perfil = result.Perfil
            };

            var response = new AuthAccessResponse
            {
                AuthAccess = dataResponse
            };

            Response.Headers.Add("Access-Control-Expose-Headers", "Authorization");
            Response.Headers.Add("Authorization", "Bearer " + token);

            return Ok(response);

        }

        #endregion
    }
}
