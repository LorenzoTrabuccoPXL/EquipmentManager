using Dapper;
using EquipmentManager.Application.Contracts;
using EquipmentManager.Domain;
using Microsoft.Extensions.Configuration;

namespace EquipmentManager.Infrastructure.Repositories
{
    public class TechniciansRepository : Repository, ITechniciansRepository
    {
        public TechniciansRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task AddAsync(Technicians technician)
        {
            using var connection = CreateConnection();  

            string sql = @"INSERT INTO Technicians (FirstName, LastName) 
                                VALUES (@FirstName, @LastName); 
                                SELECT CAST(SCOPE_IDENTITY() as int);";

            await connection.ExecuteScalarAsync<int>(sql, technician);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = CreateConnection();

            string sql = @"DELETE FROM Technicians 
                                WHERE TechnicianId = @TechnicianId";

            await connection.ExecuteAsync(sql, new 
            { 
                TechnicianId = id 
            });
        }

        public async Task<IEnumerable<Technicians>> GetAllAsync()
        {
            using var connection = CreateConnection();

            string sql = "SELECT * FROM Technicians";

            return await connection.QueryAsync<Technicians>(sql);
        }

        public async Task<Technicians?> GetByIdAsync(int id)
        {
            using var connection = CreateConnection();

            string sql = @"SELECT * FROM Technicians
                                WHERE TechnicianId = @TechnicianId";

            return await connection.QueryFirstOrDefaultAsync<Technicians?>(sql, new
            {
                TechnicianId = id
            });
        }

        public async Task UpdateAsync(Technicians technician)
        {
            using var connection = CreateConnection();

            string sql = @"UPDATE Technicians 
                                SET FirstName = @FirstName, LastName = @LastName
                                WHERE TechnicianId = @TechnicianId";

            await connection.ExecuteAsync(sql, technician);
        }
    }
}
