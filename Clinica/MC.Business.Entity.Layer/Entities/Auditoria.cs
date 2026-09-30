namespace MC.Business.Entity.Layer.Entities
{
    public abstract class Auditoria
    {
        public string UsuarioRegistro { get; set; }
        public decimal FechaRegistro { get; set; }
        public decimal HoraRegistro { get; set; }
        public string UsuarioModificacion { get; set; }
        public decimal? FechaModificacion { get; set; }
        public decimal? HoraModificacion { get; set; }
    }
}
