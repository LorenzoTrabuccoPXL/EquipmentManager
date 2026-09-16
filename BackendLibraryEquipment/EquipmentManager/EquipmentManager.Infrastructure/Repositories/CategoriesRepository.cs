using Dapper;
using EquipmentManager.Application.Contracts;
using EquipmentManager.Domain;
using Microsoft.Extensions.Configuration;

namespace EquipmentManager.Infrastructure.Repositories
{
    public class CategoriesRepository : Repository, ICategoriesRepository
    {
        public CategoriesRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task<IEnumerable<Categories>> GetAllAsync()
        {
            using var connection = CreateConnection();

            string sql = "SELECT * FROM categories";

            return await connection.QueryAsync<Categories>(sql);
        }

        public async Task<Categories?> GetByIdAsync(int id)
        {
            using var connection = CreateConnection();

            string sql = @"SELECT * FROM Categories
                                WHERE CategoryId = @CategoryId";

            return await connection.QueryFirstOrDefaultAsync<Categories?>(sql, new
            {
                CategoryId = id
            });
        }

        public async Task AddAsync(Categories category)
        {
            using var connection = CreateConnection();

            string sql = @"INSERT INTO Categories (CategoryId, Name)
                                VALUES (@CategoryId, @Name)";

            await connection.ExecuteAsync(sql, category);
        }

        public async Task UpdateAsync(Categories category)
        {
            using var connection = CreateConnection();

            string sql = @"UPDATE Categories
                                SET Name = @Name
                                WHERE CategoryId = @CategoryId";

            await connection.ExecuteAsync(sql, category);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = CreateConnection();

            string sql = @"DELETE FROM categories
                                Where CategoryId = @CategoryId";

            await connection.ExecuteAsync(sql, new
            {
                CategoryId = id
            });
        }
    }
}
