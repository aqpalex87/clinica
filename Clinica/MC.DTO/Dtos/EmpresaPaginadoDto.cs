using System.Collections.Generic;

namespace MC.DTO.Dtos
{
    public class EmpresaPaginadoDto
    {
        public int TotalRows { get; set; }
        public List<EmpresaDto> Items { get; set; } = new();
    }
}
