using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EquipmentManager.Infrastructure
{
    public class Repository
    {
        private readonly string _connectionString;

        public Repository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' ontbreekt.");
        }

        protected IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
