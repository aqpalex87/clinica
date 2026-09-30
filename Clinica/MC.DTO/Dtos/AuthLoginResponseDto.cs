using System;

namespace MC.DTO.Dtos
{
    public class AuthLoginResponseDto
    {
        public int IdUsuario { get; set; }
        public string Usuario { get; set; }
        public int IdEmpresa { get; set; }
        public string Empresa { get; set; }
        public int? IdSede { get; set; }
        public string Sede { get; set; }
        public int IdRol { get; set; }
        public string Rol { get; set; }
        public string Token { get; set; }
        public DateTime? FechaInicioVigencia { get; set; }
        public DateTime? FechaFinVigencia { get; set; }
    }
}
