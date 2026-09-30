using MC.Business.Entity.Layer.Entities;
using System.Collections.Generic;

namespace MC.Data.Access.Layer.Interfaces
{
    public interface IMenuRepository
    {
        (List<MenuRol> Menus, int TotalRows) ObtenerMenusPaginado(string nombre, int pageNumber, int pageSize);
        List<MenuRol> ObtenerMenusPadre();
        MenuRol CrearMenu(MenuRol menu);
        int? ObtenerRolPorUsuario(string usuario);
        bool AsignarMenuARol(int idRol, int idMenu);
        List<MenuRol> ObtenerMenusPorRol(int idRol);
        bool GuardarMenusPorRol(int idRol, string idsMenuCsv);
        bool ActualizarMenu(MenuRol menu);
        bool EliminarMenu(int idMenu);
    }
}
