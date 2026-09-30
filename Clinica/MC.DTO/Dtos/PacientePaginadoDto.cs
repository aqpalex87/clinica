using System.Collections.Generic;

namespace MC.DTO.Dtos
{
    public class PacientePaginadoDto
    {
        public int TotalRows { get; set; }
        public List<PacienteDto> Items { get; set; } = new();
    }
}
