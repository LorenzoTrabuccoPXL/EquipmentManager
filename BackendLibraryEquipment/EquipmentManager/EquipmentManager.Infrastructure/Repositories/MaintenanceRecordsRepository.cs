using Dapper;
using EquipmentManager.Application.Contracts;
using EquipmentManager.Domain;
using Microsoft.Extensions.Configuration;

namespace EquipmentManager.Infrastructure.Repositories
{
    public class MaintenanceRecordsRepository : Repository, IMaintenanceRecordsRepository
    {
        public MaintenanceRecordsRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task AddAsync(MaintenanceRecords maintenanceRecord)
        {
            using var connection = CreateConnection();

            string sql = @"
                INSERT INTO MaintenanceRecords (EquipmentId, MaintenanceDate, MaintenanceType, TechnicianId, Notes)
                VALUES (@EquipmentId, @MaintenanceDate, @MaintenanceType, @TechnicianId, @Notes);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            await connection.QuerySingleAsync<int>(sql, maintenanceRecord);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = CreateConnection();

            string sql = @"DELETE FROM MaintenanceRecords 
                                WHERE MaintenanceRecordId = @MaintenanceRecordId";

            await connection.ExecuteAsync(sql, new 
            {
                MaintenanceRecordId = id 
            });
        }

        public async Task<IEnumerable<MaintenanceRecords>> GetAllAsync()
        {
            using var connection = CreateConnection();

            string sql = "SELECT * FROM MaintenanceRecords";

            return await connection.QueryAsync<MaintenanceRecords>(sql);
        }

        public async Task<MaintenanceRecords?> GetByIdAsync(int id)
        {
            using var connection = CreateConnection();

            string sql = @"SELECT * FROM MaintenanceRecords
                                WHERE MaintenanceRecordId = @MaintenanceRecordId";

            return await connection.QueryFirstOrDefaultAsync<MaintenanceRecords?>(sql, new
            {
                MaintenanceRecordId = id
            });
        }

        public async Task UpdateAsync(MaintenanceRecords maintenanceRecord)
        {
            using var connection = CreateConnection();

            string sql = @"
                UPDATE MaintenanceRecords
                SET EquipmentId = @EquipmentId,
                    MaintenanceDate = @MaintenanceDate,
                    MaintenanceType = @MaintenanceType,
                    TechnicianId = @TechnicianId,
                    Notes = @Notes
                WHERE MaintenanceRecordId = @MaintenanceRecordId";

            await connection.ExecuteAsync(sql, maintenanceRecord);
        }
    }
}
