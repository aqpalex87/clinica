using System.Collections.Generic;

namespace MC.DTO.Dtos
{
    public class MenuCrudDto
    {
        public int IdMenu { get; set; }
        public string Nombre { get; set; }
        public string Ruta { get; set; }
        public string Icono { get; set; }
        public int? IdPadre { get; set; }
        public string NombrePadre { get; set; }
        public int? Orden { get; set; }
        public bool Activo { get; set; }
    }

    public class MenuPaginadoDto
    {
        public int TotalRows { get; set; }
        public List<MenuCrudDto> Items { get; set; } = new();
    }
}
