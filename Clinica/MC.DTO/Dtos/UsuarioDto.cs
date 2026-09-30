namespace MC.DTO.Dtos
{
    public class UsuarioDto
    {
        public int IdUsuario { get; set; }
        public string UsuarioLogin { get; set; }
        public string Password { get; set; }
        public int IdEmpresa { get; set; }
        public string NombreEmpresa { get; set; }
        public int IdRol { get; set; }
        public string NombreRol { get; set; }
        public bool Activo { get; set; }
    }
}
