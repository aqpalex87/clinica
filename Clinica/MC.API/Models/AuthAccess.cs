using System;
using System.ComponentModel.DataAnnotations;

namespace MC.API.Models
{
    #region request
    public class AuthAccessRequest
    {
        [Required]
        public string CorreoElectronico { get; set; }
        [Required]
        public string NombreUsuario { get; set; }
        [Required]
        public string CodigoUsuario { get; set; }
        
        //[Required]
        public string Perfil { get; set; }
        public string NumeroDocumento { get; set; }
    }
    #endregion

    #region response

    public class AuthAccessResponse
    {
        public AuthAccessResponseData AuthAccess { get; set; }
    }

    public class AuthAccessResponseData
    {
        public string NombreUsuario { get; set; }

        public string CorreoElectronico { get; set; }

        public string CodigoUsuario { get; set; }

        public DateTime? FechaInicioVigencia { get; set; }

        public DateTime? FechaFinVigencia { get; set; }
        public string Perfil { get; internal set; }
        public string NumeroDocumento { get; set; }
    }
    #endregion
}
