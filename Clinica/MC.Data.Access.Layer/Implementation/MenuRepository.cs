using MC.Business.Entity.Layer.Entities;
using MC.Data.Access.Layer.DB2;
using MC.Data.Access.Layer.Extensions;
using MC.Data.Access.Layer.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace MC.Data.Access.Layer.Implementation
{
    public class MenuRepository : IMenuRepository
    {
        private readonly string cadenaConexion;
        private readonly string esquemaDB2;

        public MenuRepository(DB2DataAccess db2Access)
        {
            cadenaConexion = db2Access.ObtConnectionStringSql();
            esquemaDB2 = db2Access.EsquemaDB2;
        }

        public (List<MenuRol> Menus, int TotalRows) ObtenerMenusPaginado(string nombre, int pageNumber, int pageSize)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_SELECT_MENUS_PAGINADO", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@P_NOMBRE", SqlDbType.VarChar, 100) { Value = (object)(nombre ?? string.Empty) });
            command.Parameters.Add(new SqlParameter("@P_PAGENUMBER", SqlDbType.Int) { Value = pageNumber });
            command.Parameters.Add(new SqlParameter("@P_PAGESIZE", SqlDbType.Int) { Value = pageSize });
            command.Parameters.Add(new SqlParameter("@P_TOTALROWS", SqlDbType.Int) { Direction = ParameterDirection.Output });
            sqlConnection.Open();
            List<MenuRol> menus;
            using (SqlDataReader dataReader = command.ExecuteReader())
            {
                menus = dataReader.GetEntities<MenuRol>();
            }
            int totalRows = command.Parameters["@P_TOTALROWS"].Value == DBNull.Value ? 0 : (int)command.Parameters["@P_TOTALROWS"].Value;
            return (menus, totalRows);
        }

        public List<MenuRol> ObtenerMenusPadre()
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_SELECT_MENUS_PADRE", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            sqlConnection.Open();
            using SqlDataReader dataReader = command.ExecuteReader();
            return dataReader.GetEntities<MenuRol>();
        }

        public MenuRol CrearMenu(MenuRol menu)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_INSERT_MENU", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@P_NOMBRE", SqlDbType.VarChar, 100) { Value = menu.Nombre });
            command.Parameters.Add(new SqlParameter("@P_RUTA", SqlDbType.VarChar, 200) { Value = (object)(menu.Ruta ?? string.Empty) });
            command.Parameters.Add(new SqlParameter("@P_ICONO", SqlDbType.VarChar, 50) { Value = (object)(menu.Icono ?? string.Empty) });
            command.Parameters.Add(new SqlParameter("@P_IDPADRE", SqlDbType.Int) { Value = (object)menu.IdPadre ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@P_ORDEN", SqlDbType.Int) { Value = (object)menu.Orden ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@P_ACTIVO", SqlDbType.Bit) { Value = menu.Activo });
            command.Parameters.Add(new SqlParameter("@P_IDMENU", SqlDbType.Int) { Direction = ParameterDirection.Output });
            sqlConnection.Open();
            command.ExecuteNonQuery();
            int idMenu = command.Parameters["@P_IDMENU"].Value == DBNull.Value ? 0 : (int)command.Parameters["@P_IDMENU"].Value;
            menu.IdMenu = idMenu;
            return menu;
        }

        public int? ObtenerRolPorUsuario(string usuario)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_SELECT_ROL_POR_USUARIO", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@P_USUARIO", SqlDbType.VarChar, 50) { Value = usuario });
            sqlConnection.Open();
            object result = command.ExecuteScalar();
            if (result == null || result == DBNull.Value)
            {
                return null;
            }
            return Convert.ToInt32(result);
        }

        public bool AsignarMenuARol(int idRol, int idMenu)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_INSERT_ROLMENU", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@P_IDROL", SqlDbType.Int) { Value = idRol });
            command.Parameters.Add(new SqlParameter("@P_IDMENU", SqlDbType.Int) { Value = idMenu });
            sqlConnection.Open();
            return command.ExecuteNonQuery() > 0;
        }

        public List<MenuRol> ObtenerMenusPorRol(int idRol)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_SELECT_MENUS_POR_ROL", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@IdRol", SqlDbType.Int) { Value = idRol });
            sqlConnection.Open();
            using SqlDataReader dataReader = command.ExecuteReader();
            return dataReader.GetEntities<MenuRol>();
        }

        public bool GuardarMenusPorRol(int idRol, string idsMenuCsv)
        {
            try
            {
                using SqlConnection sqlConnection = new(cadenaConexion);
                using SqlCommand command = new($"{esquemaDB2}.MC_SAVE_ROLMENU", sqlConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@IdRol", SqlDbType.Int) { Value = idRol });
                command.Parameters.Add(new SqlParameter("@IdsMenu", SqlDbType.VarChar, -1) { Value = (object)(idsMenuCsv ?? string.Empty) });
                sqlConnection.Open();
                object result = command.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                {
                    return false;
                }
                return Convert.ToInt32(result) == 1;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool ActualizarMenu(MenuRol menu)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_UPDATE_MENU", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@P_IDMENU", SqlDbType.Int) { Value = menu.IdMenu });
            command.Parameters.Add(new SqlParameter("@P_NOMBRE", SqlDbType.VarChar, 100) { Value = menu.Nombre });
            command.Parameters.Add(new SqlParameter("@P_RUTA", SqlDbType.VarChar, 200) { Value = (object)(menu.Ruta ?? string.Empty) });
            command.Parameters.Add(new SqlParameter("@P_ICONO", SqlDbType.VarChar, 50) { Value = (object)(menu.Icono ?? string.Empty) });
            command.Parameters.Add(new SqlParameter("@P_IDPADRE", SqlDbType.Int) { Value = (object)menu.IdPadre ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@P_ORDEN", SqlDbType.Int) { Value = (object)menu.Orden ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@P_ACTIVO", SqlDbType.Bit) { Value = menu.Activo });
            sqlConnection.Open();
            return command.ExecuteNonQuery() > 0;
        }

        public bool EliminarMenu(int idMenu)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_DELETE_MENU", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@P_IDMENU", SqlDbType.Int) { Value = idMenu });
            sqlConnection.Open();
            return command.ExecuteNonQuery() > 0;
        }
    }
}
