using EquipmentManager.Application.Contracts;
using EquipmentManager.Domain;

namespace EquipmentManager.Application.Services
{
    public class CategoriesService
    {
        private readonly ICategoriesRepository _categoriesRepositorie;

        public CategoriesService(ICategoriesRepository categoriesRepositorie)
        {
            _categoriesRepositorie = categoriesRepositorie;
        }

        public async Task<IEnumerable<Categories>> GetAllAsync()
        {
            IEnumerable<Categories> categories;

            try
            {
                categories = await _categoriesRepositorie.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving all categories: {ex.Message}");
            }

            return categories;
        }

        public async Task<Categories> GetByIdAsync(int id)
        {
            try
            {
                if(id <= 0)
                    throw new ArgumentException("Id must be positive!");

                return await _categoriesRepositorie.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot retrieve category {ex.Message}");
            }

        }

        public async Task AddAsync(Categories category)
        {
            try
            {
                if(category == null)
                    throw new ArgumentNullException("Category cannot be null!");

                await _categoriesRepositorie.AddAsync(category);
            }
            catch(Exception ex)
            {
                throw new Exception($"An error occurred while adding category: {ex.Message}");
            }
        }

        public async Task UpdateAsync(Categories category)
        {
            try
            {
                if (category == null)
                    throw new ArgumentNullException("Category cannot be null!");

                await _categoriesRepositorie.UpdateAsync(category);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating category: {ex.Message}");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Id must be positive!");

                await _categoriesRepositorie.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while deleting category: {ex.Message}");
            }
        }
    }
}
