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
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string cadenaConexion;
        private readonly string esquemaDB2;

        public UsuarioRepository(DB2DataAccess db2Access)
        {
            cadenaConexion = db2Access.ObtConnectionStringSql();
            esquemaDB2 = db2Access.EsquemaDB2;
        }

        public (List<Usuario> Usuarios, int TotalRows) ObtenerUsuariosPaginado(int? idEmpresa, string usuario, int pageNumber, int pageSize)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_SELECT_USUARIOS_PAGINADO", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@IdEmpresa", SqlDbType.Int) { Value = (object)idEmpresa ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@P_USUARIO", SqlDbType.VarChar, 50) { Value = (object)(usuario ?? string.Empty) });
            command.Parameters.Add(new SqlParameter("@P_PAGENUMBER", SqlDbType.Int) { Value = pageNumber });
            command.Parameters.Add(new SqlParameter("@P_PAGESIZE", SqlDbType.Int) { Value = pageSize });
            command.Parameters.Add(new SqlParameter("@P_TOTALROWS", SqlDbType.Int) { Direction = ParameterDirection.Output });
            sqlConnection.Open();

            List<Usuario> usuarios;
            using (SqlDataReader dataReader = command.ExecuteReader())
            {
                usuarios = dataReader.GetEntities<Usuario>();
            }

            int totalRows = command.Parameters["@P_TOTALROWS"].Value == DBNull.Value ? 0 : Convert.ToInt32(command.Parameters["@P_TOTALROWS"].Value);
            return (usuarios, totalRows);
        }

        public Usuario CrearUsuario(Usuario usuario)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_INSERT_USUARIO", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@Usuario", SqlDbType.VarChar, 50) { Value = usuario.UsuarioLogin });
            command.Parameters.Add(new SqlParameter("@PasswordHash", SqlDbType.VarChar, 256) { Value = usuario.PasswordHash });
            command.Parameters.Add(new SqlParameter("@IdEmpresa", SqlDbType.Int) { Value = usuario.IdEmpresa });
            command.Parameters.Add(new SqlParameter("@IdRol", SqlDbType.Int) { Value = usuario.IdRol });
            command.Parameters.Add(new SqlParameter("@Activo", SqlDbType.Bit) { Value = usuario.Activo });
            command.Parameters.Add(new SqlParameter("@IdUsuario", SqlDbType.Int) { Direction = ParameterDirection.Output });
            sqlConnection.Open();
            command.ExecuteNonQuery();
            usuario.IdUsuario = command.Parameters["@IdUsuario"].Value == DBNull.Value ? 0 : Convert.ToInt32(command.Parameters["@IdUsuario"].Value);
            return usuario;
        }

        public bool ActualizarUsuario(Usuario usuario)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_UPDATE_USUARIO", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@IdUsuario", SqlDbType.Int) { Value = usuario.IdUsuario });
            command.Parameters.Add(new SqlParameter("@Usuario", SqlDbType.VarChar, 50) { Value = usuario.UsuarioLogin });
            command.Parameters.Add(new SqlParameter("@PasswordHash", SqlDbType.VarChar, 256) { Value = (object)usuario.PasswordHash ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@IdEmpresa", SqlDbType.Int) { Value = usuario.IdEmpresa });
            command.Parameters.Add(new SqlParameter("@IdRol", SqlDbType.Int) { Value = usuario.IdRol });
            command.Parameters.Add(new SqlParameter("@Activo", SqlDbType.Bit) { Value = usuario.Activo });
            sqlConnection.Open();
            return command.ExecuteNonQuery() > 0;
        }

        public bool EliminarUsuario(int idUsuario)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_DELETE_USUARIO", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@IdUsuario", SqlDbType.Int) { Value = idUsuario });
            sqlConnection.Open();
            return command.ExecuteNonQuery() > 0;
        }
    }
}
