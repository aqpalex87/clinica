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
    public class AuthRepository : IAuthRepository
    {
        private readonly string cadenaConexion;
        private readonly string esquemaDB2;

        public AuthRepository(DB2DataAccess db2Access)
        {
            cadenaConexion = db2Access.ObtConnectionStringSql();
            esquemaDB2 = db2Access.EsquemaDB2;
        }

        public UsuarioAuth ObtenerUsuario(string usuario)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_SELECT_USUARIO_POR_LOGIN", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@Usuario", SqlDbType.VarChar, 50) { Value = usuario });
            sqlConnection.Open();
            using SqlDataReader dataReader = command.ExecuteReader();
            return dataReader.GetEntity<UsuarioAuth>();
        }

        public List<MenuRol> ObtenerMenusPorUsuario(string usuario, int? idEmpresa, int? idSede)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_SELECT_MENUS_POR_USUARIO", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@Usuario", SqlDbType.VarChar, 50) { Value = usuario });
            command.Parameters.Add(new SqlParameter("@IdEmpresa", SqlDbType.Int) { Value = (object)idEmpresa ?? DBNull.Value });
            sqlConnection.Open();
            using SqlDataReader dataReader = command.ExecuteReader();
            return dataReader.GetEntities<MenuRol>();
        }

        public List<EmpresaAuth> ObtenerEmpresas()
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_SELECT_EMPRESAS_ACTIVAS", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            sqlConnection.Open();
            using SqlDataReader dataReader = command.ExecuteReader();
            return dataReader.GetEntities<EmpresaAuth>();
        }

        public List<SedeAuth> ObtenerSedesPorEmpresa(int idEmpresa)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_SELECT_SEDES_POR_EMPRESA", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@IdEmpresa", SqlDbType.Int) { Value = idEmpresa });
            sqlConnection.Open();
            using SqlDataReader dataReader = command.ExecuteReader();
            return dataReader.GetEntities<SedeAuth>();
        }
    }
}
