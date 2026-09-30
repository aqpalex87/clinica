namespace MC.Business.Entity.Layer.Entities
{
    public class UsuarioAuth
    {
        public int IdUsuario { get; set; }
        public string Usuario { get; set; }
        public string PasswordHash { get; set; }
        public int IdEmpresa { get; set; }
        public string EmpresaNombre { get; set; }
        public int? IdSede { get; set; }
        public string SedeNombre { get; set; }
        public int IdRol { get; set; }
        public string RolNombre { get; set; }
        public bool Activo { get; set; }
    }
}
