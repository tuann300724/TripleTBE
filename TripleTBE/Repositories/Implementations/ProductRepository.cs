using Azure.Core;
using Microsoft.EntityFrameworkCore;

using TripleTBE.DTOs;
using TripleTBE.Models;
using TripleTBE.Repositories.Interfaces;

namespace TripleTBE.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly BadmintonStoreDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductRepository(
              BadmintonStoreDbContext context,
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
       .Select(p => new ProductDTOs
       {
           ProductId = p.ProductId,
           ProductName = p.ProductName,
           Price = p.Price,
           Stock = p.Stock,
           Description = p.Description,
           Thumbnail = p.Thumbnail,
           Status = p.Status,
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
        .Where(p => p.ProductId == id)
        .Select(p => new ProductDTOs
        {
            ProductId = p.ProductId,
            ProductName = p.ProductName,
            Price = p.Price,
            Stock = p.Stock,
            Description = p.Description,
            Thumbnail = p.Thumbnail,
            Status = p.Status,
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

        // CREATE
        public async Task<ProductDTOs> CreateAsync(
            Product product,
            IFormFile? thumbnail,
            List<IFormFile>? images)
        {
            var request = _httpContextAccessor.HttpContext!.Request;

            // AUTO STATUS
            product.Status = product.Stock > 0 ? 1 : 2;

            // thumbnail
            if (thumbnail != null)
            {
                var thumbName =
                    Guid.NewGuid().ToString()
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

            // nhiều ảnh
            if (images != null && images.Any())
            {
                foreach (var image in images)
                {
                    var fileName =
                        Guid.NewGuid().ToString()
                        + Path.GetExtension(image.FileName);

                    var filePath = Path.Combine(
                        _env.WebRootPath,
                        "images/products",
                        fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var imageUrl =
                        $"{request.Scheme}://{request.Host}/images/products/{fileName}";

                    product.ProductImages.Add(new ProductImage
                    {
                        ImageUrl = imageUrl
                    });
                }
            }

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return new ProductDTOs
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Price = product.Price,
                Stock = product.Stock,
                Description = product.Description,
                Thumbnail = product.Thumbnail,
                Status = product.Status,

                BrandName = "",
                CategoryName = "",

                Images = product.ProductImages
                    .Select(x => x.ImageUrl)
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
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return null;

            // UPDATE INFO
            if (!string.IsNullOrEmpty(updatedProduct.ProductName))
            {
                product.ProductName = updatedProduct.ProductName;
            }

            if (updatedProduct.Price > 0)
            {
                product.Price = updatedProduct.Price;
            }

            if (updatedProduct.Stock >= 0)
            {
                product.Stock = updatedProduct.Stock;
            }

            if (!string.IsNullOrEmpty(updatedProduct.Description))
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
                // user tự chọn status
                product.Status = updatedProduct.Status.Value;
            }
            else
            {
                // tự động theo stock
                product.Status = product.Stock > 0 ? 1 : 2;
            }
            product.UpdatedAt = DateTime.Now;         

            // UPDATE THUMBNAIL
            if (thumbnail != null)
            {
                // DELETE OLD THUMBNAIL
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

                // SAVE NEW THUMBNAIL
                var thumbnailName =
                    Guid.NewGuid().ToString()
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

            // UPDATE IMAGES
            if (images != null && images.Any())
            {
                // DELETE OLD IMAGES
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

                // ADD NEW IMAGES
                foreach (var image in images)
                {
                    var imageName =
                        Guid.NewGuid().ToString()
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

            // LOAD LẠI BRAND CATEGORY
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
                Price = product.Price,
                Stock = product.Stock,
                Description = product.Description,
                Thumbnail = product.Thumbnail,
                Status = product.Status,

                BrandName = product.Brand.BrandName,
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