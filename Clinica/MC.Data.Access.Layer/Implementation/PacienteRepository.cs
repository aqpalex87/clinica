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
    public class PacienteRepository : IPacienteRepository
    {
        private readonly string cadenaConexion;
        private readonly string esquemaDB2;

        public PacienteRepository(DB2DataAccess db2Access)
        {
            cadenaConexion = db2Access.ObtConnectionStringSql();
            esquemaDB2 = db2Access.EsquemaDB2;
        }

        public (List<Paciente> Pacientes, int TotalRows) ObtenerPacientesPaginado(int? idEmpresa, string nombre, string numeroDocumento, string contacto, int pageNumber, int pageSize)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_SELECT_PACIENTES_PAGINADO", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@IdEmpresa", SqlDbType.Int) { Value = (object)idEmpresa ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@P_NOMBRE", SqlDbType.VarChar, 150) { Value = nombre ?? string.Empty });
            command.Parameters.Add(new SqlParameter("@P_DOCUMENTO", SqlDbType.VarChar, 20) { Value = numeroDocumento ?? string.Empty });
            command.Parameters.Add(new SqlParameter("@P_CONTACTO", SqlDbType.VarChar, 120) { Value = contacto ?? string.Empty });
            command.Parameters.Add(new SqlParameter("@P_PAGENUMBER", SqlDbType.Int) { Value = pageNumber });
            command.Parameters.Add(new SqlParameter("@P_PAGESIZE", SqlDbType.Int) { Value = pageSize });
            command.Parameters.Add(new SqlParameter("@P_TOTALROWS", SqlDbType.Int) { Direction = ParameterDirection.Output });
            sqlConnection.Open();

            List<Paciente> pacientes;
            using (SqlDataReader dataReader = command.ExecuteReader())
            {
                pacientes = dataReader.GetEntities<Paciente>();
            }

            int totalRows = command.Parameters["@P_TOTALROWS"].Value == DBNull.Value ? 0 : Convert.ToInt32(command.Parameters["@P_TOTALROWS"].Value);
            return (pacientes, totalRows);
        }

        public int CrearPaciente(Paciente paciente)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_INSERT_PACIENTE", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            AgregarDatos(command, paciente);
            SqlParameter idPaciente = new("@IdPaciente", SqlDbType.Int) { Direction = ParameterDirection.Output };
            SqlParameter codigo = new("@Codigo", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Output };
            SqlParameter resultado = new("@Resultado", SqlDbType.Int) { Direction = ParameterDirection.Output };
            command.Parameters.Add(idPaciente);
            command.Parameters.Add(codigo);
            command.Parameters.Add(resultado);
            sqlConnection.Open();
            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqlException ex) when (EsDocumentoDuplicado(ex))
            {
                return -1;
            }

            int codigoResultado = LeerEntero(resultado);
            paciente.IdPaciente = LeerEntero(idPaciente);
            paciente.Codigo = codigo.Value == DBNull.Value ? null : Convert.ToString(codigo.Value);
            return codigoResultado;
        }

        public int ActualizarPaciente(Paciente paciente)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_UPDATE_PACIENTE", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@IdPaciente", SqlDbType.Int) { Value = paciente.IdPaciente });
            AgregarDatos(command, paciente);
            SqlParameter resultado = new("@Resultado", SqlDbType.Int) { Direction = ParameterDirection.Output };
            command.Parameters.Add(resultado);
            sqlConnection.Open();
            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqlException ex) when (EsDocumentoDuplicado(ex))
            {
                return -1;
            }
            return LeerEntero(resultado);
        }

        public int EliminarPaciente(int idPaciente)
        {
            using SqlConnection sqlConnection = new(cadenaConexion);
            using SqlCommand command = new($"{esquemaDB2}.MC_DELETE_PACIENTE", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@IdPaciente", SqlDbType.Int) { Value = idPaciente });
            SqlParameter resultado = new("@Resultado", SqlDbType.Int) { Direction = ParameterDirection.Output };
            command.Parameters.Add(resultado);
            sqlConnection.Open();
            command.ExecuteNonQuery();
            return LeerEntero(resultado);
        }

        private static void AgregarDatos(SqlCommand command, Paciente paciente)
        {
            command.Parameters.Add(new SqlParameter("@IdEmpresa", SqlDbType.Int) { Value = paciente.IdEmpresa });
            command.Parameters.Add(new SqlParameter("@IdSede", SqlDbType.Int) { Value = paciente.IdSede });
            command.Parameters.Add(new SqlParameter("@TipoDocumento", SqlDbType.VarChar, 10) { Value = paciente.TipoDocumento });
            command.Parameters.Add(new SqlParameter("@NumeroDocumento", SqlDbType.VarChar, 20) { Value = paciente.NumeroDocumento });
            command.Parameters.Add(new SqlParameter("@ApellidoPaterno", SqlDbType.VarChar, 80) { Value = paciente.ApellidoPaterno });
            command.Parameters.Add(new SqlParameter("@ApellidoMaterno", SqlDbType.VarChar, 80) { Value = (object)paciente.ApellidoMaterno ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@Nombres", SqlDbType.VarChar, 120) { Value = paciente.Nombres });
            command.Parameters.Add(new SqlParameter("@FechaNacimiento", SqlDbType.Date) { Value = paciente.FechaNacimiento.Date });
            command.Parameters.Add(new SqlParameter("@Sexo", SqlDbType.VarChar, 1) { Value = paciente.Sexo });
            command.Parameters.Add(new SqlParameter("@Celular", SqlDbType.VarChar, 9) { Value = paciente.Celular });
            command.Parameters.Add(new SqlParameter("@Correo", SqlDbType.VarChar, 120) { Value = (object)paciente.Correo ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@Direccion", SqlDbType.VarChar, 200) { Value = (object)paciente.Direccion ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@Observacion", SqlDbType.VarChar, 300) { Value = (object)paciente.Observacion ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@Activo", SqlDbType.Bit) { Value = paciente.Activo });
        }

        private static bool EsDocumentoDuplicado(SqlException ex)
        {
            return ex.Number == 2601 || ex.Number == 2627;
        }

        private static int LeerEntero(SqlParameter parametro)
        {
            return parametro.Value == null || parametro.Value == DBNull.Value ? 0 : Convert.ToInt32(parametro.Value);
        }
    }
}
