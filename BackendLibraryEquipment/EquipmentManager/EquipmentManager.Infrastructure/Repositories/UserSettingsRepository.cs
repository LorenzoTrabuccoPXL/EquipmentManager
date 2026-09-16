using Dapper;
using EquipmentManager.Application.Contracts;
using EquipmentManager.Domain;
using Microsoft.Extensions.Configuration;

namespace EquipmentManager.Infrastructure.Repositories
{
    public class UserSettingsRepository : Repository, IUserSettingsRepository
    {
        public UserSettingsRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task AddAsync(UserSettings userSettings)
        {
            using var connection = CreateConnection();

            string sql = @"
                INSERT INTO UserSettings (ItemsPerPage, DateFormat, OverdueAlerts, UpcomingAlerts, OfflineAlerts, EmailDigest, ReminderDays, DefaultTechnicianId)
                VALUES (@ItemsPerPage, @DateFormat, @OverdueAlerts, @UpcomingAlerts, @OfflineAlerts, @EmailDigest, @ReminderDays, @DefaultTechnicianId);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            await connection.QuerySingleAsync<int>(sql, userSettings);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = CreateConnection();

            string sql = @"DELETE FROM UserSettings 
                                WHERE UserSettingsId = @UserSettingsId";

            await connection.ExecuteAsync(sql, new
            {
                UserSettingsId = id
            });
        }

        public async Task<IEnumerable<UserSettings>> GetAllAsync()
        {
            using var connection = CreateConnection();

            string sql = "SELECT * FROM UserSettings";

            return await connection.QueryAsync<UserSettings>(sql);
        }

        public async Task<UserSettings?> GetByIdAsync(int id)
        {
            using var connection = CreateConnection();

            string sql = @"SELECT * FROM UserSettings
                                WHERE UserSettingsId = @UserSettingsId";

            return await connection.QueryFirstOrDefaultAsync<UserSettings?>(sql, new
            {
                UserSettingsId = id
            });
        }

        public async Task UpdateAsync(UserSettings userSettings)
        {
            using var connection = CreateConnection();

            string sql = @"
                UPDATE UserSettings
                SET ItemsPerPage = @ItemsPerPage,
                    DateFormat = @DateFormat,
                    OverdueAlerts = @OverdueAlerts,
                    UpcomingAlerts = @UpcomingAlerts,
                    OfflineAlerts = @OfflineAlerts,
                    EmailDigest = @EmailDigest,
                    ReminderDays = @ReminderDays,
                    DefaultTechnicianId = @DefaultTechnicianId
                WHERE UserSettingsId = @UserSettingsId";

            await connection.ExecuteAsync(sql, userSettings);
        }
    }
}
