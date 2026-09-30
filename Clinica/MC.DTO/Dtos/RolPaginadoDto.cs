using System.Collections.Generic;

namespace MC.DTO.Dtos
{
    public class RolPaginadoDto
    {
        public int TotalRows { get; set; }
        public List<RolDto> Items { get; set; } = new();
    }
}
