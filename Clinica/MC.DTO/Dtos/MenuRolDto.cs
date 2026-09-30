using System.Collections.Generic;

namespace MC.DTO.Dtos
{
    public class RolSimpleDto
    {
        public int IdRol { get; set; }
        public string Nombre { get; set; }
    }

    public class MenuRolTreeDto
    {
        public int IdMenu { get; set; }
        public string Nombre { get; set; }
        public int? IdPadre { get; set; }
        public bool Seleccionado { get; set; }
        public List<MenuRolTreeDto> Hijos { get; set; } = new();
    }

    public class GuardarMenuRolRequestDto
    {
        public int IdRol { get; set; }
        public List<int> IdsMenu { get; set; } = new();
    }
}
