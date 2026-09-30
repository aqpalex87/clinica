namespace MC.Business.Entity.Layer
{
    public class UserJwt
    {
        public string Perfil { get; set; }
        public string NroDocumento { get; set; }
        public string Nombre { get; set; }
        public string CodUsuario { get; set; }
        public int? IdRol { get; set; }
        public int? IdEmpresa { get; set; }
        public int? IdSede { get; set; }
    }
}
