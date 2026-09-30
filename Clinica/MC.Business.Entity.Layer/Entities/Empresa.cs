namespace MC.Business.Entity.Layer.Entities
{
    public class Empresa : Auditoria
    {
        public int IdEmpresa { get; set; }
        public string Nombre { get; set; }
        public string Ruc { get; set; }
        public bool Activo { get; set; }
    }
}
