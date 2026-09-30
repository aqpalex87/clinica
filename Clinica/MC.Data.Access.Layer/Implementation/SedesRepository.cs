using MC.Business.Entity.Layer.Entities;
using MC.Data.Access.Layer.DB2;
using MC.Data.Access.Layer.Extensions;
using MC.Data.Access.Layer.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace MC.Data.Access.Layer.Implementation
{
    public class SedesRepository : ISedesRepository
    {
        private readonly string cadenaConexion = null;
        private readonly string esquemaDB2;

        public SedesRepository(DB2DataAccess db2Access)
        {
            cadenaConexion = db2Access.ObtConnectionStringSql();
            esquemaDB2 = db2Access.EsquemaDB2;
        }
        public List<Sedes> ObtenerSedesPorEmpresa(int idEmpresa)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_SELECT_SEDES_POR_EMPRESA", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@IdEmpresa", idEmpresa));
            sqlConnection.Open();

            using SqlDataReader dataReader = command.ExecuteReader();
            return dataReader.GetEntities<Sedes>();
        }

        public (List<Sedes> Sedes, int TotalRows) ObtenerSedesPaginado(int? idEmpresa, string nombre, int pageNumber, int pageSize)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_SELECT_SEDES_PAGINADO", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@IdEmpresa", SqlDbType.Int) { Value = (object)idEmpresa ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@P_NOMBRE", SqlDbType.VarChar, 150) { Value = (object)(nombre ?? string.Empty) });
            command.Parameters.Add(new SqlParameter("@P_PAGENUMBER", SqlDbType.Int) { Value = pageNumber });
            command.Parameters.Add(new SqlParameter("@P_PAGESIZE", SqlDbType.Int) { Value = pageSize });
            command.Parameters.Add(new SqlParameter("@P_TOTALROWS", SqlDbType.Int) { Direction = ParameterDirection.Output });
            sqlConnection.Open();

            List<Sedes> sedes;
            using (SqlDataReader dataReader = command.ExecuteReader())
            {
                sedes = dataReader.GetEntities<Sedes>();
            }
            int totalRows = command.Parameters["@P_TOTALROWS"].Value == DBNull.Value ? 0 : Convert.ToInt32(command.Parameters["@P_TOTALROWS"].Value);
            return (sedes, totalRows);
        }

        public Sedes CrearSede(Sedes sede)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_INSERT_SEDE", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@IdEmpresa", SqlDbType.Int) { Value = sede.IdEmpresa });
            command.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.VarChar, 150) { Value = sede.Nombre });
            command.Parameters.Add(new SqlParameter("@Codigo", SqlDbType.VarChar, 30) { Value = (object)(sede.Codigo ?? string.Empty) });
            command.Parameters.Add(new SqlParameter("@Direccion", SqlDbType.VarChar, 250) { Value = (object)(sede.Direccion ?? string.Empty) });
            command.Parameters.Add(new SqlParameter("@Activo", SqlDbType.Bit) { Value = sede.Activo });
            command.Parameters.Add(new SqlParameter("@IdSede", SqlDbType.Int) { Direction = ParameterDirection.Output });
            sqlConnection.Open();
            command.ExecuteNonQuery();
            sede.IdSede = command.Parameters["@IdSede"].Value == DBNull.Value ? 0 : Convert.ToInt32(command.Parameters["@IdSede"].Value);
            return sede;
        }

        public bool ActualizarSede(Sedes sede)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_UPDATE_SEDE", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@IdSede", SqlDbType.Int) { Value = sede.IdSede });
            command.Parameters.Add(new SqlParameter("@IdEmpresa", SqlDbType.Int) { Value = sede.IdEmpresa });
            command.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.VarChar, 150) { Value = sede.Nombre });
            command.Parameters.Add(new SqlParameter("@Codigo", SqlDbType.VarChar, 30) { Value = (object)(sede.Codigo ?? string.Empty) });
            command.Parameters.Add(new SqlParameter("@Direccion", SqlDbType.VarChar, 250) { Value = (object)(sede.Direccion ?? string.Empty) });
            command.Parameters.Add(new SqlParameter("@Activo", SqlDbType.Bit) { Value = sede.Activo });
            sqlConnection.Open();
            return command.ExecuteNonQuery() > 0;
        }

        public bool EliminarSede(int idSede)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_DELETE_SEDE", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@IdSede", SqlDbType.Int) { Value = idSede });
            sqlConnection.Open();
            return command.ExecuteNonQuery() > 0;
        }
    }
}
