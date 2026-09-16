using Dapper;
using EquipmentManager.Application.Contracts;
using EquipmentManager.Domain;
using Microsoft.Extensions.Configuration;

namespace EquipmentManager.Infrastructure.Repositories
{
    public class UsersRepository : Repository, IUsersRepository
    {
        public UsersRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task AddAsync(Users user)
        {
            using var connection = CreateConnection();

            string sql = @"INSERT INTO Users (Firstname, Lastname, Email, PasswordHash, Role, Phone) 
                                VALUES (@Firstname, @Lastname, @Email, @PasswordHash, @Role, @Phone); 
                                SELECT CAST(SCOPE_IDENTITY() as int);";

            await connection.QuerySingleAsync<int>(sql, user);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = CreateConnection();

            string sql = @"DELETE FROM Users 
                                WHERE UserId = @UserId";

            await connection.ExecuteAsync(sql, new 
            { 
                UserId = id 
            });
        }

        public async Task<IEnumerable<Users>> GetAllAsync()
        {
            using var connection = CreateConnection();

            string sql = "SELECT * FROM Users";

            return await connection.QueryAsync<Users>(sql);
        }

        public async Task<Users?> GetByIdAsync(int id)
        {
            using var connection = CreateConnection();

            string sql = @"SELECT * FROM Users
                                WHERE UserId = @UserId";

            return await connection.QueryFirstOrDefaultAsync<Users?>(sql, new
            {
                UserId = id
            });
        }

        public async Task<Users?> GetByEmailAsync(string email)
        {
            using var connection = CreateConnection();

            string sql = @"SELECT * FROM Users
                                WHERE Email = @Email";

            return await connection.QueryFirstOrDefaultAsync<Users?>(sql, new
            {
                Email = email
            });
        }

        public async Task UpdateAsync(Users user)
        {
            using var connection = CreateConnection();

            string sql = @"UPDATE Users 
                                SET Firstname = @Firstname, 
                                    Lastname = @Lastname, 
                                    Email = @Email, 
                                    PasswordHash = @PasswordHash, 
                                    Role = @Role, 
                                    Phone = @Phone
                                WHERE UserId = @UserId";

            await connection.ExecuteAsync(sql, user);
        }
    }
}
