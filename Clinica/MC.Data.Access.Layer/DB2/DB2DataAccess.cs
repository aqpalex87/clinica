using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using Microsoft.Data.SqlClient;

namespace MC.Data.Access.Layer.DB2
{
    public class DB2DataAccess
    {
        private readonly IConfiguration _configuration;
        private readonly int SQLApplicationNameEnableEncrip;
        private const int ENABLE_ENCRIPT_HAB = 1;
        private const int ENABLE_ENCRIPT_DES = 0;
        private readonly string SQLRegeditFolder;
        public string EsquemaDB2 { get; }

        public DB2DataAccess(IConfiguration configuration)
        {
            _configuration = configuration;
            SQLApplicationNameEnableEncrip = configuration.GetValue<int>("SQLApplicationNameEnableEncrip");
            SQLRegeditFolder = configuration.GetValue<string>("SQLRegeditFolder");
            EsquemaDB2 = configuration.GetValue<string>("EsquemaDB2");
        }

        public string ObtConnectionStringSql()
        {
            // Primero intenta obtener la cadena de conexión directamente desde la configuración
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(connectionString))
            {
                return connectionString;
            }

            // Si no existe, construye la cadena desde el registro o configuración
            var conectionStringBuilder = new SqlConnectionStringBuilder();
            
            if (SQLApplicationNameEnableEncrip == ENABLE_ENCRIPT_DES)
            {
                // Lee desde el registro de Windows
                conectionStringBuilder = new SqlConnectionStringBuilder
                {
                    DataSource = ReadKey("Server"),
                    InitialCatalog = ReadKey("Database"),
                    UserID = ReadKey("Usuario"),
                    Password = ReadKey("Clave")
                };
            }
            else
            {
                // Lee desde la configuración directamente
                conectionStringBuilder = new SqlConnectionStringBuilder
                {
                    DataSource = _configuration.GetValue<string>("ConnectionStrings:Server") ?? ReadKey("Server"),
                    InitialCatalog = _configuration.GetValue<string>("ConnectionStrings:Database") ?? ReadKey("Database"),
                    UserID = _configuration.GetValue<string>("ConnectionStrings:Usuario") ?? ReadKey("Usuario"),
                    Password = _configuration.GetValue<string>("ConnectionStrings:Clave") ?? ReadKey("Clave")
                };
            }
            
            return conectionStringBuilder.ConnectionString;
        }

        private string ReadKey(string value)
        {
            Registry.LocalMachine.OpenSubKey("SOFTWARE", true);
            var masterKey = Registry.LocalMachine.CreateSubKey(@$"SOFTWARE\AGROBANCO\{SQLRegeditFolder}");
            string valueret = "";
            if (masterKey != null)
            {
                valueret = masterKey.GetValue(value).ToString();
            }
            masterKey?.Close();
            return valueret;
        }
    }
}
