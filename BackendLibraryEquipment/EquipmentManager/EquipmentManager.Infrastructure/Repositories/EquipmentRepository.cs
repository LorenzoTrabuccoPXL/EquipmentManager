using Dapper;
using EquipmentManager.Application.Contracts;
using EquipmentManager.Domain;
using Microsoft.Extensions.Configuration;

namespace EquipmentManager.Infrastructure.Repositories
{
    public class EquipmentRepository : Repository, IEquipmentRepository
    {
        public EquipmentRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task AddAsync(Equipment equipment)
        {
            using var connection = CreateConnection();

            string sql = @"
                INSERT INTO Equipment (Name, Model, SerialNumber, CategoryId, LocationId, Status, PurchasePrice, WarrantyExpiry, LastMaintenance, NextMaintenance, Description)
                VALUES (@Name, @Model, @SerialNumber, @CategoryId, @LocationId, @Status, @PurchasePrice, @WarrantyExpiry, @LastMaintenance, @NextMaintenance, @Description);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            await connection.QueryFirstAsync<int>(sql, equipment);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = CreateConnection();

            string sql = @"DELETE FROM Equipment WHERE EquipmentId = @EquipmentId";

            await connection.ExecuteAsync(sql, new 
            { 
                EquipmentId = id 
            });
        }

        public async Task<IEnumerable<Equipment>> GetAllAsync()
        {
            using var connection = CreateConnection();

            string sql = "SELECT * FROM Equipment";

            return await connection.QueryAsync<Equipment>(sql);
        }

        public async Task<Equipment?> GetByIdAsync(int id)
        {
            using var connection = CreateConnection();

            string sql = @"SELECT * FROM Equipment WHERE EquipmentId = @EquipmentId";

            return await connection.QueryFirstOrDefaultAsync<Equipment?>(sql, new 
            { 
                EquipmentId = id 
            });
        }

        public async Task UpdateAsync(Equipment equipment)
        {
            using var connection = CreateConnection();
            string sql = @"
                UPDATE Equipment 
                SET Name = @Name, 
                    Model = @Model, 
                    SerialNumber = @SerialNumber, 
                    CategoryId = @CategoryId, 
                    LocationId = @LocationId,
                    Status = @Status,
                    PurchasePrice = @PurchasePrice,
                    WarrantyExpiry = @WarrantyExpiry,
                    LastMaintenance = @LastMaintenance,
                    NextMaintenance = @NextMaintenance,
                    Description = @Description
                WHERE EquipmentId = @EquipmentId";

            await connection.ExecuteAsync(sql, equipment);
        }
    }
}
