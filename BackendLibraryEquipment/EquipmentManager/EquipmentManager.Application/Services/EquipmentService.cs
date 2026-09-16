using EquipmentManager.Application.Contracts;
using EquipmentManager.Domain;

namespace EquipmentManager.Application.Services
{
    public class EquipmentService
    {
        private readonly IEquipmentRepository _equipmentRepository;

        public EquipmentService(IEquipmentRepository equipmentRepository)
        {
            _equipmentRepository = equipmentRepository;
        }

        public async Task<IEnumerable<Equipment>> GetAllAsync()
        {
            try
            {
                return await _equipmentRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving all equipment: {ex.Message}");
            }
        }

        public async Task<Equipment> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Id must be positive!");

                return await _equipmentRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot retrieve equipment: {ex.Message}");
            }
        }

        public async Task AddAsync(Equipment equipment)
        {
            try
            {
                if (equipment == null)
                    throw new ArgumentNullException("Equipment cannot be null!");

                await _equipmentRepository.AddAsync(equipment);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while adding equipment: {ex.Message}");
            }
        }

        public async Task UpdateAsync(Equipment equipment)
        {
            try
            {
                if (equipment == null)
                    throw new ArgumentNullException("Equipment cannot be null!");

                await _equipmentRepository.UpdateAsync(equipment);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating equipment: {ex.Message}");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Id must be positive!");

                await _equipmentRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while deleting equipment: {ex.Message}");
            }
        }
    }
}
