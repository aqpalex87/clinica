using MC.Business.Entity.Layer.Entities;
using System.Collections.Generic;

namespace MC.Data.Access.Layer.Interfaces
{
    public interface IPacienteRepository
    {
        (List<Paciente> Pacientes, int TotalRows) ObtenerPacientesPaginado(int? idEmpresa, string nombre, string numeroDocumento, string contacto, int pageNumber, int pageSize);
        int CrearPaciente(Paciente paciente);
        int ActualizarPaciente(Paciente paciente);
        int EliminarPaciente(int idPaciente);
    }
}
