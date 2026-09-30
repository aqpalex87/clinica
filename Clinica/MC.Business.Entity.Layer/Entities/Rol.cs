namespace MC.Business.Entity.Layer.Entities
{
    public class Rol : Auditoria
    {
        public int IdRol { get; set; }
        public int IdEmpresa { get; set; }
        public string NombreEmpresa { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
    }
}
