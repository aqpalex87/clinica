using MC.Business.Entity.Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Data.Access.Layer.Interfaces
{
    public interface ISedesRepository
    {
        List<Sedes> ObtenerSedesPorEmpresa(int idEmpresa);
        (List<Sedes> Sedes, int TotalRows) ObtenerSedesPaginado(int? idEmpresa, string nombre, int pageNumber, int pageSize);
        Sedes CrearSede(Sedes sede);
        bool ActualizarSede(Sedes sede);
        bool EliminarSede(int idSede);
    }
}
