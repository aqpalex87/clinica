namespace MC.Business.Entity.Layer.Entities
{
    public class MenuRol
    {
        public int IdMenu { get; set; }
        public string Nombre { get; set; }
        public string Ruta { get; set; }
        public string Icono { get; set; }
        public int? IdPadre { get; set; }
        public string NombrePadre { get; set; }
        public int? Orden { get; set; }
        public bool Activo { get; set; }
        public bool Seleccionado { get; set; }
    }
}
