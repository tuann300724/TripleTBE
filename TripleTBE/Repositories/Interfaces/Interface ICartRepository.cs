using Microsoft.AspNetCore.Http;
using TripleTBE.DTOs;
using TripleTBE.Models;

namespace TripleTBE.Interfaces
{
    public interface IBrandRepository
    {
        Task<IEnumerable<Brand>> GetAllAsync();

        Task<Brand?> GetByIdAsync(int id);

        Task<Brand> CreateAsync(
            Brand brand,
            IFormFile? logo);

        Task<Brand?> UpdateAsync(
            int id,
            Brand updatedBrand,
            IFormFile? logo);

        Task<bool> DeleteAsync(int id);
    }
}