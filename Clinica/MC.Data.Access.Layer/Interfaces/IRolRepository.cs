using MC.Business.Entity.Layer.Entities;
using System.Collections.Generic;

namespace MC.Data.Access.Layer.Interfaces
{
    public interface IRolRepository
    {
        (List<Rol> Roles, int TotalRows) ObtenerRolesPaginado(int? idEmpresa, string nombre, int pageNumber, int pageSize);
        List<Rol> ObtenerRolesPorEmpresa(int idEmpresa);
        Rol CrearRol(Rol rol);
        bool ActualizarRol(Rol rol);
        bool EliminarRol(int idRol);
    }
}
