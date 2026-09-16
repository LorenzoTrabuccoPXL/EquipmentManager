using EquipmentManager.Application.Contracts;
using EquipmentManager.Domain;

namespace EquipmentManager.Application.Services
{
    public class TechniciansService
    {
        private readonly ITechniciansRepository _techniciansRepository;

        public TechniciansService(ITechniciansRepository techniciansRepository)
        {
            _techniciansRepository = techniciansRepository;
        }

        public async Task<IEnumerable<Technicians>> GetAllAsync()
        {
            try
            {
                return await _techniciansRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving technicians.", ex);
            }
        }

        public async Task<Technicians> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Id must be positive!");

                return await _techniciansRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving technician with Id {id}.", ex);
            }
        }

        public async Task AddAsync(Technicians technician)
        {
            try
            {
                if (technician == null)
                    throw new ArgumentNullException(nameof(technician), "Technician cannot be null!");

                await _techniciansRepository.AddAsync(technician);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while adding a new technician.", ex);
            }
        }

        public async Task UpdateAsync(Technicians technician)
        {
            try
            {
                if (technician == null)
                    throw new ArgumentNullException(nameof(technician), "Technician cannot be null!");

                await _techniciansRepository.UpdateAsync(technician);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while updating the technician.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Id must be positive!");

                await _techniciansRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while deleting technician with Id {id}.", ex);
            }
        }
    }
}
