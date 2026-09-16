using EquipmentManager.Domain;

namespace EquipmentManager.Application.Contracts
{
    public interface IUsersRepository : IRepository<Users>
    {
        Task<Users> GetByEmailAsync(string email);
    }
}
