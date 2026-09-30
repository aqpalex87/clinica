using System;

namespace MC.Business.Entity.Layer.Entities
{
    public class Paciente
    {
        public int IdPaciente { get; set; }
        public int IdEmpresa { get; set; }
        public string NombreEmpresa { get; set; }
        public int IdSede { get; set; }
        public string NombreSede { get; set; }
        public string Codigo { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Nombres { get; set; }
        public string NombreCompleto { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int Edad { get; set; }
        public string Sexo { get; set; }
        public string Celular { get; set; }
        public string Correo { get; set; }
        public string Direccion { get; set; }
        public string Observacion { get; set; }
        public bool Activo { get; set; }
    }
}
