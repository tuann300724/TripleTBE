using TripleTBE.DTOs;
using TripleTBE.Models;

namespace TripleTBE.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<List<ProductDTOs>> GetAllAsync();

        Task<ProductDTOs?> GetByIdAsync(int id);

        Task<ProductDTOs> CreateAsync(
           Product product,
           IFormFile? thumbnail,
           List<IFormFile>? images);

        Task<ProductDTOs?> UpdateAsync(
     int id,
     Product product,
     IFormFile? thumbnail,
     List<IFormFile>? images);
        Task<bool> ChangeStatusAsync(int id, int status);
    }
}