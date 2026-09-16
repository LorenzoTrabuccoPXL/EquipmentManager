using EquipmentManager.Application.Contracts;
using EquipmentManager.Domain;

namespace EquipmentManager.Application.Services
{
    public class NotificationsService
    {
        private readonly INotificationsRepository _notificationsRepository;

        public NotificationsService(INotificationsRepository notificationsRepository)
        {
            _notificationsRepository = notificationsRepository;
        }

        public async Task<IEnumerable<Notifications>> GetAllAsync()
        {
            try
            {
                return await _notificationsRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving notifications.", ex);
            }
        }

        public async Task<Notifications> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Id must be positive!");

                return await _notificationsRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving notification with Id {id}.", ex);
            }
        }

        public async Task AddAsync(Notifications notification)
        {
            try
            {
                if (notification == null)
                    throw new ArgumentNullException(nameof(notification), "Notification cannot be null!");

                await _notificationsRepository.AddAsync(notification);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while adding a new notification.", ex);
            }
        }

        public async Task UpdateAsync(Notifications notification)
        {
            try
            {
                if (notification == null)
                    throw new ArgumentNullException(nameof(notification), "Notification cannot be null!");

                await _notificationsRepository.UpdateAsync(notification);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while updating the notification.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Id must be positive!");

                await _notificationsRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while deleting notification with Id {id}.", ex);
            }
        }
    }
}
