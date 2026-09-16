using Dapper;
using EquipmentManager.Application.Contracts;
using EquipmentManager.Domain;
using Microsoft.Extensions.Configuration;

namespace EquipmentManager.Infrastructure.Repositories
{
    public class LocationsRepository : Repository, ILocationsRepository
    {
        public LocationsRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task AddAsync(Locations location)
        {
            using var connection = CreateConnection();

            string sql = @"INSERT INTO Locations (Name) VALUES (@Name); 
                                SELECT CAST(SCOPE_IDENTITY() as int);";

            await connection.QuerySingleAsync<int>(sql, location);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = CreateConnection();

            string sql = @"DELETE FROM Locations 
                                WHERE LocationId = @LocationId";

            await connection.ExecuteAsync(sql, new 
            { 
                LocationId = id 
            });
        }

        public async Task<IEnumerable<Locations>> GetAllAsync()
        {
            using var connection = CreateConnection();

            string sql = "SELECT * FROM Locations";

            return await connection.QueryAsync<Locations>(sql);
        }

        public async Task<Locations?> GetByIdAsync(int id)
        {
            using var connection = CreateConnection();

            string sql = @"SELECT * FROM Locations
                                WHERE LocationId = @LocationId";

            return await connection.QueryFirstOrDefaultAsync<Locations?>(sql, new
            {
                LocationId = id
            });
        }

        public async Task UpdateAsync(Locations location)
        {
            using var connection = CreateConnection();

            string sql = @"UPDATE Locations 
                                SET Name = @Name
                                WHERE LocationId = @LocationId";

            await connection.ExecuteAsync(sql, location);
        }
    }
}
