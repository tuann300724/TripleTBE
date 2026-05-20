using TripleTBE.Models;

namespace TripleTBE.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<IEnumerable<Cart>> GetAllAsync();

        Task<Cart?> GetByIdAsync(int id);

        Task<Cart> CreateAsync(Cart cart);

        Task<Cart?> UpdateAsync(int id, Cart updatedCart);

        Task<bool> DeleteAsync(int id);
    }
}