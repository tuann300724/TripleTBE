using TripleTBE.Models;

namespace TripleTBE.Repositories.Interfaces
{
    public interface IBrandRepository
    {
        Task<IEnumerable<Brand>> GetAllBrands();

        Task<Brand?> GetByIdBrand(int id);

        Task<Brand> CreateBrand(
            Brand brand,
            IFormFile? logo);

        Task<Brand?> UpdateBrand(
            int id,
            Brand updatedBrand,
            IFormFile? logo);

        Task<bool> DeleteBrand(int id);
    }
}