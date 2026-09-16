namespace EquipmentManager.Application.Contracts
{
    public interface IRepository<TEntity>
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(int id);
        Task UpdateAsync(TEntity entity);
        Task AddAsync(TEntity entity);
        Task DeleteAsync(int id);
    }
}   
