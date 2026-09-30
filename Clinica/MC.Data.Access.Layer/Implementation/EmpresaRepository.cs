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
    public class EmpresaRepository : IEmpresaRepository
    {
        private readonly string cadenaConexion;
        private readonly string esquemaDB2;

        public EmpresaRepository(DB2DataAccess db2Access)
        {
            cadenaConexion = db2Access.ObtConnectionStringSql();
            esquemaDB2 = db2Access.EsquemaDB2;
        }

        public (List<Empresa> Empresas, int TotalRows) ObtenerEmpresasPaginado(string nombre, int pageNumber, int pageSize)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_SELECT_EMPRESAS_PAGINADO", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@P_NOMBRE", SqlDbType.VarChar, 150) { Value = (object)(nombre ?? string.Empty) });
            command.Parameters.Add(new SqlParameter("@P_PAGENUMBER", SqlDbType.Int) { Value = pageNumber });
            command.Parameters.Add(new SqlParameter("@P_PAGESIZE", SqlDbType.Int) { Value = pageSize });
            command.Parameters.Add(new SqlParameter("@P_TOTALROWS", SqlDbType.Int) { Direction = ParameterDirection.Output });
            sqlConnection.Open();

            List<Empresa> empresas;
            using (SqlDataReader dataReader = command.ExecuteReader())
            {
                empresas = dataReader.GetEntities<Empresa>();
            }
            int totalRows = command.Parameters["@P_TOTALROWS"].Value == DBNull.Value ? 0 : Convert.ToInt32(command.Parameters["@P_TOTALROWS"].Value);
            return (empresas, totalRows);
        }

        public Empresa CrearEmpresa(Empresa empresa)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_INSERT_EMPRESA", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.VarChar, 150) { Value = empresa.Nombre });
            command.Parameters.Add(new SqlParameter("@Ruc", SqlDbType.VarChar, 20) { Value = (object)(empresa.Ruc ?? string.Empty) });
            command.Parameters.Add(new SqlParameter("@Activo", SqlDbType.Bit) { Value = empresa.Activo });
            command.Parameters.Add(new SqlParameter("@IdEmpresa", SqlDbType.Int) { Direction = ParameterDirection.Output });
            sqlConnection.Open();
            command.ExecuteNonQuery();
            empresa.IdEmpresa = command.Parameters["@IdEmpresa"].Value == DBNull.Value ? 0 : Convert.ToInt32(command.Parameters["@IdEmpresa"].Value);
            return empresa;
        }

        public bool ActualizarEmpresa(Empresa empresa)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_UPDATE_EMPRESA", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@IdEmpresa", SqlDbType.Int) { Value = empresa.IdEmpresa });
            command.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.VarChar, 150) { Value = empresa.Nombre });
            command.Parameters.Add(new SqlParameter("@Ruc", SqlDbType.VarChar, 20) { Value = (object)(empresa.Ruc ?? string.Empty) });
            command.Parameters.Add(new SqlParameter("@Activo", SqlDbType.Bit) { Value = empresa.Activo });
            sqlConnection.Open();
            return command.ExecuteNonQuery() > 0;
        }

        public bool EliminarEmpresa(int idEmpresa)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_DELETE_EMPRESA", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@IdEmpresa", SqlDbType.Int) { Value = idEmpresa });
            sqlConnection.Open();
            return command.ExecuteNonQuery() > 0;
        }
    }
}
