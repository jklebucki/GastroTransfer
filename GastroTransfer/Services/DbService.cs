using GastroTransfer.Models;
using System.Data.SqlClient;

namespace GastroTransfer.Services
{
    public class DbService : IDbService
    {
        private Config config { get; set; }
        public string ErrorMessage { get; protected set; }

        public DbService(Config config)
        {
            this.config = config;
        }
        public bool CheckConnection()
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString(5)))
            {
                try
                {
                    connection.Open();
                    connection.Close();
                    return true;
                }
                catch (SqlException ex)
                {
                    ErrorMessage = ex.Message;
                    return false;
                }
            }
        }

        public bool CheckLsiConnection()
        {
            using (SqlConnection connection = new SqlConnection(GetLsiConnectionString(30)))
            {
                try
                {
                    connection.Open();
                    connection.Close();
                    return true;
                }
                catch (SqlException ex)
                {
                    ErrorMessage = ex.Message;
                    return false;
                }
            }
        }

        public string GetConnectionString(int timeout = 0)
        {
            string connectionString = "";
            var timeoutSection = timeout > 0 ? $"Connection Timeout={timeout};" : "";
            if (config.IsTrustedConnection)
            {
                connectionString = $"Server={config.ServerAddress};Database={config.DatabaseName};Trusted_Connection=True;{timeoutSection}";
            }
            else
            {
                connectionString = $"Server={config.ServerAddress};Database={config.DatabaseName};User id={config.UserName};Password={config.Password};{timeoutSection}";
            }

            if (!string.IsNullOrEmpty(config.AdditionalConnectionStringDirective))
                connectionString += config.AdditionalConnectionStringDirective;

            return connectionString;
        }

        public string GetLsiConnectionString(int timeout = 0)
        {
            string connectionString = "";
            var timeoutSection = timeout > 0 ? $"Connection Timeout={timeout};" : "";
            if (config.IsTrustedConnection)
            {
                connectionString = $"Server={config.ExternalDbServerAddress};Database={config.ExternalDbDatabaseName};Trusted_Connection=True;{timeoutSection}";
            }
            else
            {
                connectionString = $"Server={config.ExternalDbServerAddress};Database={config.ExternalDbDatabaseName};User id={config.ExternalDbUserName};Password={config.ExternalDbPassword};{timeoutSection}";
            }

            if (!string.IsNullOrEmpty(config.ExternalDbAdditionalConnectionStringDirective))
                connectionString += config.ExternalDbAdditionalConnectionStringDirective;

            return connectionString;
        }
    }
}
