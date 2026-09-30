using MC.DTO;
using MC.DTO.Dtos;

namespace MC.Business.Logic.Layer.Interfaces
{
    public interface IPacienteService
    {
        Result<PacientePaginadoDto> ObtenerPacientesPaginado(int? idEmpresa, string nombre, string numeroDocumento, string contacto, int pageNumber, int pageSize);
        Result<PacienteDto> CrearPaciente(PacienteDto request);
        Result<PacienteDto> ActualizarPaciente(int idPaciente, PacienteDto request);
        Result<bool> EliminarPaciente(int idPaciente);
    }
}
