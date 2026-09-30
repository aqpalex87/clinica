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
    public class RolRepository : IRolRepository
    {
        private readonly string cadenaConexion;
        private readonly string esquemaDB2;

        public RolRepository(DB2DataAccess db2Access)
        {
            cadenaConexion = db2Access.ObtConnectionStringSql();
            esquemaDB2 = db2Access.EsquemaDB2;
        }

        public (List<Rol> Roles, int TotalRows) ObtenerRolesPaginado(int? idEmpresa, string nombre, int pageNumber, int pageSize)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_SELECT_ROLES_PAGINADO", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@IdEmpresa", SqlDbType.Int) { Value = (object)idEmpresa ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@P_NOMBRE", SqlDbType.VarChar, 150) { Value = (object)(nombre ?? string.Empty) });
            command.Parameters.Add(new SqlParameter("@P_PAGENUMBER", SqlDbType.Int) { Value = pageNumber });
            command.Parameters.Add(new SqlParameter("@P_PAGESIZE", SqlDbType.Int) { Value = pageSize });
            command.Parameters.Add(new SqlParameter("@P_TOTALROWS", SqlDbType.Int) { Direction = ParameterDirection.Output });
            sqlConnection.Open();

            List<Rol> roles;
            using (SqlDataReader dataReader = command.ExecuteReader())
            {
                roles = dataReader.GetEntities<Rol>();
            }
            int totalRows = command.Parameters["@P_TOTALROWS"].Value == DBNull.Value ? 0 : Convert.ToInt32(command.Parameters["@P_TOTALROWS"].Value);
            return (roles, totalRows);
        }

        public List<Rol> ObtenerRolesPorEmpresa(int idEmpresa)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_SELECT_ROLES_POR_EMPRESA", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@IdEmpresa", SqlDbType.Int) { Value = idEmpresa });
            sqlConnection.Open();
            using SqlDataReader dataReader = command.ExecuteReader();
            return dataReader.GetEntities<Rol>();
        }

        public Rol CrearRol(Rol rol)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_INSERT_ROL", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@IdEmpresa", SqlDbType.Int) { Value = rol.IdEmpresa });
            command.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.VarChar, 150) { Value = rol.Nombre });
            command.Parameters.Add(new SqlParameter("@Descripcion", SqlDbType.VarChar, 300) { Value = (object)(rol.Descripcion ?? string.Empty) });
            command.Parameters.Add(new SqlParameter("@Activo", SqlDbType.Bit) { Value = rol.Activo });
            command.Parameters.Add(new SqlParameter("@IdRol", SqlDbType.Int) { Direction = ParameterDirection.Output });
            sqlConnection.Open();
            command.ExecuteNonQuery();
            rol.IdRol = command.Parameters["@IdRol"].Value == DBNull.Value ? 0 : Convert.ToInt32(command.Parameters["@IdRol"].Value);
            return rol;
        }

        public bool ActualizarRol(Rol rol)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_UPDATE_ROL", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@IdRol", SqlDbType.Int) { Value = rol.IdRol });
            command.Parameters.Add(new SqlParameter("@IdEmpresa", SqlDbType.Int) { Value = rol.IdEmpresa });
            command.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.VarChar, 150) { Value = rol.Nombre });
            command.Parameters.Add(new SqlParameter("@Descripcion", SqlDbType.VarChar, 300) { Value = (object)(rol.Descripcion ?? string.Empty) });
            command.Parameters.Add(new SqlParameter("@Activo", SqlDbType.Bit) { Value = rol.Activo });
            sqlConnection.Open();
            return command.ExecuteNonQuery() > 0;
        }

        public bool EliminarRol(int idRol)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_DELETE_ROL", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@IdRol", SqlDbType.Int) { Value = idRol });
            sqlConnection.Open();
            return command.ExecuteNonQuery() > 0;
        }
    }
}
