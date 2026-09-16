using EquipmentManager.Application.Contracts;
using EquipmentManager.Domain;

namespace EquipmentManager.Application.Services
{
    public class UserSettingsService
    {
        private readonly IUserSettingsRepository _userSettingsRepository;

        public UserSettingsService(IUserSettingsRepository userSettingsRepository)
        {
            _userSettingsRepository = userSettingsRepository;
        }

        public async Task<IEnumerable<UserSettings>> GetAllAsync()
        {
            return await _userSettingsRepository.GetAllAsync();
        }


        public async Task<UserSettings?> GetByIdAsync(int id)
        {
            try
            {
                if(id <= 0)
                    throw new ArgumentException("Id must be positive!");

                return await _userSettingsRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving user settings with ID {id}.", ex);
            }
        }

        public async Task AddAsync(UserSettings userSettings)
        {
            try
            {
                if (userSettings == null)
                    throw new ArgumentNullException(nameof(userSettings), "User settings cannot be null!");

                await _userSettingsRepository.AddAsync(userSettings);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while adding new user settings.", ex);
            }
        }

        public async Task UpdateAsync(UserSettings userSettings)
        {
            try
            {
                if (userSettings == null)
                    throw new ArgumentNullException(nameof(userSettings), "User settings cannot be null!");

                await _userSettingsRepository.UpdateAsync(userSettings);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while updating user settings.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                if(id <= 0)
                    throw new ArgumentException("Id must be positive!");

                await _userSettingsRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while deleting user settings with ID {id}.", ex);
            }
        }
    }
}
