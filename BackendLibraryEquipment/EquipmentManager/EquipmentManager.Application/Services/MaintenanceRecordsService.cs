using EquipmentManager.Application.Contracts;
using EquipmentManager.Domain;

namespace EquipmentManager.Application.Services
{
    public class MaintenanceRecordsService
    {
        private readonly IMaintenanceRecordsRepository _maintenanceRecordsRepository;

        public MaintenanceRecordsService(IMaintenanceRecordsRepository maintenanceRecordsRepository)
        {
            _maintenanceRecordsRepository = maintenanceRecordsRepository;
        }

        public async Task<IEnumerable<MaintenanceRecords>> GetAllAsync()
        {
            try
            {
                return await _maintenanceRecordsRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving maintenance records.", ex);
            }
        }

        public async Task<MaintenanceRecords> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Id must be positive!");

                return await _maintenanceRecordsRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving maintenance record with Id {id}.", ex);
            }
        }

        public async Task AddAsync(MaintenanceRecords maintenanceRecord)
        {
            try
            {
                if (maintenanceRecord == null)
                    throw new ArgumentNullException(nameof(maintenanceRecord), "Maintenance record cannot be null!");

                await _maintenanceRecordsRepository.AddAsync(maintenanceRecord);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while adding a new maintenance record.", ex);
            }
        }

        public async Task UpdateAsync(MaintenanceRecords maintenanceRecord)
        {
            try
            {
                if (maintenanceRecord == null)
                    throw new ArgumentNullException(nameof(maintenanceRecord), "Maintenance record cannot be null!");

                await _maintenanceRecordsRepository.UpdateAsync(maintenanceRecord);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while updating the maintenance record.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Id must be positive!");

                await _maintenanceRecordsRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while deleting maintenance record with Id {id}.", ex);
            }
        }
    }
}
