using Azure.Core;
using Microsoft.EntityFrameworkCore;

using TripleTBE.DTOs;
using TripleTBE.Models;
using TripleTBE.Repositories.Interfaces;

namespace TripleTBE.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly BadmintonDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductRepository(
              BadmintonDbContext context,
              IWebHostEnvironment env,
              IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET ALL
        public async Task<List<ProductDTOs>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .Select(p => new ProductDTOs
                {
                    ProductId = p.ProductId,

                    ProductName = p.ProductName,

                    Description = p.Description,

                    Thumbnail = p.Thumbnail,

                    Status = p.Status,

                    MinPrice = p.ProductVariants
                        .Min(v => (decimal?)v.Price),

                    MaxPrice = p.ProductVariants
                        .Max(v => (decimal?)v.Price),

                    BrandName = p.Brand.BrandName,

                    Country = p.Brand.Country,

                    Logo = p.Brand.Logo,

                    CategoryName = p.Category.CategoryName,

                    Images = p.ProductImages
                        .Select(i => i.ImageUrl)
                        .ToList()
                })
                .ToListAsync();
        }

        // GET BY ID
        public async Task<ProductDTOs?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .Where(p => p.ProductId == id)
                .Select(p => new ProductDTOs
                {
                    ProductId = p.ProductId,

                    ProductName = p.ProductName,

                    Description = p.Description,

                    Thumbnail = p.Thumbnail,

                    Status = p.Status,

                    MinPrice = p.ProductVariants
                        .Min(v => (decimal?)v.Price),

                    MaxPrice = p.ProductVariants
                        .Max(v => (decimal?)v.Price),

                    BrandName = p.Brand.BrandName,

                    Country = p.Brand.Country,

                    Logo = p.Brand.Logo,

                    CategoryName = p.Category.CategoryName,

                    Images = p.ProductImages
                        .Select(i => i.ImageUrl)
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ProductDTOs> CreateAsync(
     Product product,
     IFormFile? thumbnail,
     List<IFormFile>? images)
        {
            var request = _httpContextAccessor.HttpContext!.Request;

            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;

            if (product.Status == null)
            {
                product.Status = 1;
            }

            // THUMBNAIL
            if (thumbnail != null)
            {
                var thumbName =
                    Guid.NewGuid()
                    + Path.GetExtension(thumbnail.FileName);

                var thumbPath = Path.Combine(
                    _env.WebRootPath,
                    "images/products",
                    thumbName);

                using (var stream = new FileStream(thumbPath, FileMode.Create))
                {
                    await thumbnail.CopyToAsync(stream);
                }

                product.Thumbnail =
                    $"{request.Scheme}://{request.Host}/images/products/{thumbName}";
            }

            // IMAGES
            if (images != null && images.Any())
            {
                foreach (var image in images)
                {
                    var imageName =
                        Guid.NewGuid()
                        + Path.GetExtension(image.FileName);

                    var imagePath = Path.Combine(
                        _env.WebRootPath,
                        "images/products",
                        imageName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var imageUrl =
                        $"{request.Scheme}://{request.Host}/images/products/{imageName}";

                    product.ProductImages.Add(new ProductImage
                    {
                        ImageUrl = imageUrl
                    });
                }
            }

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            await _context.Entry(product)
                .Reference(p => p.Brand)
                .LoadAsync();

            await _context.Entry(product)
                .Reference(p => p.Category)
                .LoadAsync();

            return new ProductDTOs
            {
                ProductId = product.ProductId,

                ProductName = product.ProductName,

                Description = product.Description,

                Thumbnail = product.Thumbnail,

                Status = product.Status,

                BrandName = product.Brand.BrandName,

                Country = product.Brand.Country,

                Logo = product.Brand.Logo,

                CategoryName = product.Category.CategoryName,

                Images = product.ProductImages
                    .Select(i => i.ImageUrl)
                    .ToList()
            };
        }
        // UPDATE
        public async Task<ProductDTOs?> UpdateAsync(
     int id,
     Product updatedProduct,
     IFormFile? thumbnail,
     List<IFormFile>? images)
        {
            var request = _httpContextAccessor.HttpContext!.Request;

            var product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return null;

            /* =========================================
               UPDATE INFO
            ========================================= */

            if (!string.IsNullOrWhiteSpace(updatedProduct.ProductName))
            {
                product.ProductName = updatedProduct.ProductName;
            }

            if (!string.IsNullOrWhiteSpace(updatedProduct.Description))
            {
                product.Description = updatedProduct.Description;
            }

            if (updatedProduct.BrandId > 0)
            {
                product.BrandId = updatedProduct.BrandId;
            }

            if (updatedProduct.CategoryId > 0)
            {
                product.CategoryId = updatedProduct.CategoryId;
            }

            if (updatedProduct.Status != null)
            {
                product.Status = updatedProduct.Status;
            }

            product.UpdatedAt = DateTime.Now;

            /* =========================================
               UPDATE THUMBNAIL
            ========================================= */

            if (thumbnail != null)
            {
                // delete old thumbnail
                if (!string.IsNullOrEmpty(product.Thumbnail))
                {
                    var oldThumbnailName =
                        Path.GetFileName(product.Thumbnail);

                    var oldThumbnailPath = Path.Combine(
                        _env.WebRootPath,
                        "images/products",
                        oldThumbnailName);

                    if (File.Exists(oldThumbnailPath))
                    {
                        File.Delete(oldThumbnailPath);
                    }
                }

                // save new thumbnail
                var thumbnailName =
                    Guid.NewGuid()
                    + Path.GetExtension(thumbnail.FileName);

                var thumbnailPath = Path.Combine(
                    _env.WebRootPath,
                    "images/products",
                    thumbnailName);

                using (var stream = new FileStream(thumbnailPath, FileMode.Create))
                {
                    await thumbnail.CopyToAsync(stream);
                }

                product.Thumbnail =
                    $"{request.Scheme}://{request.Host}/images/products/{thumbnailName}";
            }

            /* =========================================
               UPDATE IMAGES
            ========================================= */

            if (images != null && images.Any())
            {
                // delete old images
                foreach (var oldImage in product.ProductImages.ToList())
                {
                    var oldImageName =
                        Path.GetFileName(oldImage.ImageUrl);

                    var oldImagePath = Path.Combine(
                        _env.WebRootPath,
                        "images/products",
                        oldImageName);

                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }

                    _context.ProductImages.Remove(oldImage);
                }

                // add new images
                foreach (var image in images)
                {
                    var imageName =
                        Guid.NewGuid()
                        + Path.GetExtension(image.FileName);

                    var imagePath = Path.Combine(
                        _env.WebRootPath,
                        "images/products",
                        imageName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var imageUrl =
                        $"{request.Scheme}://{request.Host}/images/products/{imageName}";

                    product.ProductImages.Add(new ProductImage
                    {
                        ImageUrl = imageUrl
                    });
                }
            }

            await _context.SaveChangesAsync();

            // reload
            await _context.Entry(product)
                .Reference(p => p.Brand)
                .LoadAsync();

            await _context.Entry(product)
                .Reference(p => p.Category)
                .LoadAsync();

            return new ProductDTOs
            {
                ProductId = product.ProductId,

                ProductName = product.ProductName,

                Description = product.Description,

                Thumbnail = product.Thumbnail,

                Status = product.Status,

                MinPrice = product.ProductVariants
                    .Any()
                    ? product.ProductVariants.Min(v => v.Price)
                    : null,

                MaxPrice = product.ProductVariants
                    .Any()
                    ? product.ProductVariants.Max(v => v.Price)
                    : null,

                BrandName = product.Brand.BrandName,

                Country = product.Brand.Country,

                Logo = product.Brand.Logo,

                CategoryName = product.Category.CategoryName,

                Images = product.ProductImages
                    .Select(i => i.ImageUrl)
                    .ToList()
            };
        }
        public async Task<bool> ChangeStatusAsync(int id, int status)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return false;

            product.Status = status;
            product.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }
    }

}