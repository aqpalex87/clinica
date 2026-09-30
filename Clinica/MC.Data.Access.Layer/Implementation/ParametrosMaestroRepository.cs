using MC.Business.Entity.Layer.Entities;
using MC.Data.Access.Layer.DB2;
using MC.Data.Access.Layer.Extensions;
using MC.Data.Access.Layer.Interfaces;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace MC.Data.Access.Layer.Implementation
{
    public class ParametrosMaestroRepository : IParametrosMaestroRepository
    {
        private readonly string cadenaConexion;
        private readonly string esquemaDB2;

        public ParametrosMaestroRepository(DB2DataAccess db2Access)
        {
            cadenaConexion = db2Access.ObtConnectionStringSql();
            esquemaDB2 = db2Access.EsquemaDB2;
        }

        public List<ParametroListaItem> ObtenerParametrosPorTipoCodigo(string codigoTipo)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_SELECT_PARAMETROS_POR_TIPO_CODIGO", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@CodigoTipo", SqlDbType.VarChar, 50) { Value = codigoTipo ?? string.Empty });
            sqlConnection.Open();
            using SqlDataReader dataReader = command.ExecuteReader();
            return dataReader.GetEntities<ParametroListaItem>();
        }
    }
}
