using MC.Business.Entity.Layer.Entities;
using System.Collections.Generic;

namespace MC.Data.Access.Layer.Interfaces
{
    public interface IUsuarioRepository
    {
        (List<Usuario> Usuarios, int TotalRows) ObtenerUsuariosPaginado(int? idEmpresa, string usuario, int pageNumber, int pageSize);
        Usuario CrearUsuario(Usuario usuario);
        bool ActualizarUsuario(Usuario usuario);
        bool EliminarUsuario(int idUsuario);
    }
}
