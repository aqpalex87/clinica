using MC.Business.Entity.Layer;
using MC.Data.Access.Layer.DB2;
using MC.Data.Access.Layer.Extensions;
using MC.Data.Access.Layer.Interfaces;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace MC.Data.Access.Layer.Implementation
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly string cadenaConexion = null;
        private readonly string esquemaDB2;

        public ClienteRepository(DB2DataAccess db2Access)
        {
            cadenaConexion = db2Access.ObtConnectionStringSql();
            esquemaDB2 = db2Access.EsquemaDB2;
        }

        public List<Cliente> GetClienteByCodigo(decimal? codigo)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.SP_SPC_CLIENTE_X_CODIGO", sqlConnection);
            command.Parameters.Add(new SqlParameter("P_CODCLIENTE", SqlDbType.Decimal) { Value = codigo, Precision = 7, Scale = 0 });
            command.CommandType = CommandType.StoredProcedure;
            sqlConnection.Open();
            using SqlDataReader dr = command.ExecuteReader();
            var clientes = dr.GetEntities<Cliente>();
            return clientes;
        }

        public List<Cliente> GetClienteByNombre(string nombre)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.SP_SPC_CLIENTE_X_NOMBRE", sqlConnection);
            command.Parameters.Add(new SqlParameter("P_NOMBRE", SqlDbType.VarChar, 50) { Value = nombre });
            command.CommandType = CommandType.StoredProcedure;
            sqlConnection.Open();
            using SqlDataReader dr = command.ExecuteReader();
            var clientes = dr.GetEntities<Cliente>();
            return clientes;
        }

        public List<Cliente> GetClienteByNombreByPage(string nombre, int pageNumber, int pageSize, out int totalRows)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.SP_SPC_CLIENTE_X_NOMBRE_PAGE", sqlConnection);
            command.Parameters.Add(new SqlParameter("P_NOMBRE", SqlDbType.VarChar, 50) { Value = nombre });
            command.Parameters.Add(new SqlParameter("P_PageNumber", SqlDbType.Int) { Value = pageNumber });
            command.Parameters.Add(new SqlParameter("P_PageSize", SqlDbType.Int) { Value = pageSize });
            command.Parameters.Add(new SqlParameter("P_TotalRows", SqlDbType.Int) { Value = pageSize, Direction = ParameterDirection.Output });
            command.CommandType = CommandType.StoredProcedure;
            sqlConnection.Open();
            using SqlDataReader dr = command.ExecuteReader();
            var clientes = dr.GetEntities<Cliente>();
            totalRows = (int)command.Parameters["P_TotalRows"].Value;
            return clientes;
        }

        public List<Cliente> GetClienteByNroDoc(string nroDoc)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.SP_SPC_CLIENTE_X_DOCUMENTO", sqlConnection);
            command.Parameters.Add(new SqlParameter("P_NRODOC", SqlDbType.VarChar, 50) { Value = nroDoc });
            command.CommandType = CommandType.StoredProcedure;
            sqlConnection.Open();
            using SqlDataReader dr = command.ExecuteReader();
            var clientes = dr.GetEntities<Cliente>();
            return clientes;
        }

    }
}
