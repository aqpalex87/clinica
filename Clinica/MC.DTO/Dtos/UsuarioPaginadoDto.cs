using System.Collections.Generic;

namespace MC.DTO.Dtos
{
    public class UsuarioPaginadoDto
    {
        public int TotalRows { get; set; }
        public List<UsuarioDto> Items { get; set; } = new();
    }
}
