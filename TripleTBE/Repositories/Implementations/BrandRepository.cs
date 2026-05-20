using Microsoft.EntityFrameworkCore;
using System;

using TripleTBE.Repositories.Interfaces;
using TripleTBE.Models;

namespace TripleTBE.Repositories.Implementations
{
    public class BrandRepository : IBrandRepository
    {
        private readonly BadmintonStoreDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BrandRepository(
            BadmintonStoreDbContext context,
            IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET ALL
        public async Task<IEnumerable<Brand>> GetAllBrands()
        {
            return await _context.Brands.ToListAsync();
        }

        // GET BY ID
        public async Task<Brand?> GetByIdBrand(int id)
        {
            return await _context.Brands
                .FirstOrDefaultAsync(x => x.BrandId == id);
        }

        // CREATE
        public async Task<Brand> CreateBrand(
            Brand brand,
            IFormFile? logo)
        {
            var request = _httpContextAccessor.HttpContext!.Request;

            // upload logo
            if (logo != null)
            {
                var fileName =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(logo.FileName);

                var filePath = Path.Combine(
                    _env.WebRootPath,
                    "images/brands",
                    fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await logo.CopyToAsync(stream);
                }

                brand.Logo =
                    $"{request.Scheme}://{request.Host}/images/brands/{fileName}";
            }

            brand.CreatedAt = DateTime.Now;

            _context.Brands.Add(brand);

            await _context.SaveChangesAsync();

            return brand;
        }

        // UPDATE
        public async Task<Brand?> UpdateBrand(
            int id,
            Brand updatedBrand,
            IFormFile? logo)
        {
            var request = _httpContextAccessor.HttpContext!.Request;

            var brand = await _context.Brands
                .FirstOrDefaultAsync(x => x.BrandId == id);

            if (brand == null)
                return null;

            // update data
            if (!string.IsNullOrEmpty(updatedBrand.BrandName))
            {
                brand.BrandName = updatedBrand.BrandName;
            }

            if (!string.IsNullOrEmpty(updatedBrand.Country))
            {
                brand.Country = updatedBrand.Country;
            }

            // update logo
            if (logo != null)
            {
                // delete old logo
                if (!string.IsNullOrEmpty(brand.Logo))
                {
                    var oldLogoName =
                        Path.GetFileName(brand.Logo);

                    var oldLogoPath = Path.Combine(
                        _env.WebRootPath,
                        "images/brands",
                        oldLogoName);

                    if (File.Exists(oldLogoPath))
                    {
                        File.Delete(oldLogoPath);
                    }
                }

                // save new logo
                var fileName =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(logo.FileName);

                var filePath = Path.Combine(
                    _env.WebRootPath,
                    "images/brands",
                    fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await logo.CopyToAsync(stream);
                }

                brand.Logo =
                    $"{request.Scheme}://{request.Host}/images/brands/{fileName}";
            }

            await _context.SaveChangesAsync();

            return brand;
        }

        // DELETE
        public async Task<bool> DeleteBrand(int id)
        {
            var brand = await _context.Brands
                .FirstOrDefaultAsync(x => x.BrandId == id);

            if (brand == null)
                return false;

            // delete logo file
            if (!string.IsNullOrEmpty(brand.Logo))
            {
                var logoName =
                    Path.GetFileName(brand.Logo);

                var logoPath = Path.Combine(
                    _env.WebRootPath,
                    "images/brands",
                    logoName);

                if (File.Exists(logoPath))
                {
                    File.Delete(logoPath);
                }
            }

            _context.Brands.Remove(brand);

            await _context.SaveChangesAsync();

            return true;
        }

       
    }
}