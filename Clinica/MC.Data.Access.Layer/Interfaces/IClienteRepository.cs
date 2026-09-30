using MC.Business.Entity.Layer;
using System.Collections.Generic;

namespace MC.Data.Access.Layer.Interfaces
{
    public interface IClienteRepository
    {
        List<Cliente> GetClienteByCodigo(decimal? codigo);
        List<Cliente> GetClienteByNroDoc(string nroDoc);
        List<Cliente> GetClienteByNombre(string nombre);
        List<Cliente> GetClienteByNombreByPage(string nombre, int pageNumber, int pageSize, out int totalRows);
    }
}
