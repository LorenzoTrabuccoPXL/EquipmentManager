using Dapper;
using EquipmentManager.Application.Contracts;
using EquipmentManager.Domain;
using Microsoft.Extensions.Configuration;

namespace EquipmentManager.Infrastructure.Repositories
{
    public class NotificationsRepository : Repository, INotificationsRepository
    {
        public NotificationsRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task AddAsync(Notifications notification)
        {
            using var connection = CreateConnection();

            string sql = @"
                INSERT INTO Notifications (UserId, EquipmentId, EventType, Severity, Title, Message, IsRead, CreatedAt)
                VALUES (@UserId, @EquipmentId, @EventType, @Severity, @Title, @Message, @IsRead, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            await connection.QuerySingleAsync<int>(sql, notification);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = CreateConnection();

            string sql = @"DELETE FROM Notifications 
                                WHERE NotificationId = @NotificationId";

            await connection.ExecuteAsync(sql, new 
            {
                NotificationId = id 
            });
        }

        public async Task<IEnumerable<Notifications>> GetAllAsync()
        {
            using var connection = CreateConnection();

            string sql = "SELECT * FROM Notifications";

            return await connection.QueryAsync<Notifications>(sql);
        }

        public async Task<Notifications?> GetByIdAsync(int id)
        {
            using var connection = CreateConnection();

            string sql = @"SELECT * FROM Notifications
                                WHERE NotificationId = @NotificationId";

            return await connection.QueryFirstOrDefaultAsync<Notifications?>(sql, new
            {
                NotificationId = id
            });
        }

        public async Task UpdateAsync(Notifications notification)
        {
            using var connection = CreateConnection();

            string sql = @"
                UPDATE Notifications 
                SET UserId = @UserId, EquipmentId = @EquipmentId, EventType = @EventType, Severity = @Severity, Title = @Title, Message = @Message, IsRead = @IsRead, CreatedAt = @CreatedAt
                WHERE NotificationId = @NotificationId";

            await connection.ExecuteAsync(sql, notification);
        }
    }
}
