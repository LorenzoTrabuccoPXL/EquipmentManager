using EquipmentManager.Application.Contracts;
using EquipmentManager.Domain;

namespace EquipmentManager.Application.Services
{
    public class LocationsService
    {
        private readonly ILocationsRepository _locationsRepository;

        public LocationsService(ILocationsRepository locationsRepository)
        {
            _locationsRepository = locationsRepository;
        }

        public async Task<IEnumerable<Locations>> GetAllAsync()
        {
            try
            {
                return await _locationsRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving locations.", ex);
            }
        }

        public async Task<Locations> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Id must be positive!");

                return await _locationsRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving location with Id {id}.", ex);
            }
        }

        public async Task AddAsync(Locations location)
        {
            try
            {
                if (location == null)
                    throw new ArgumentNullException(nameof(location), "Location cannot be null!");

                await _locationsRepository.AddAsync(location);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while adding a new location.", ex);
            }
        }

        public async Task UpdateAsync(Locations location)
        {
            try
            {
                if (location == null)
                    throw new ArgumentNullException(nameof(location), "Location cannot be null!");

                await _locationsRepository.UpdateAsync(location);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while updating the location.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Id must be positive!");

                await _locationsRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while deleting location with Id {id}.", ex);
            }
        }
    }
}
