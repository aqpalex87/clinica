using System.Collections.Generic;

namespace MC.DTO.Dtos
{
    public class MenuDto
    {
        public int IdMenu { get; set; }
        public string Nombre { get; set; }
        public string Ruta { get; set; }
        public string Icono { get; set; }
        public int? IdPadre { get; set; }
        public int? Orden { get; set; }
        public List<MenuDto> Hijos { get; set; } = new();
    }
}
