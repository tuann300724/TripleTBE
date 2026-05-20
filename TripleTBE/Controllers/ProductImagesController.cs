using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripleTBE.Models;

namespace TripleTBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImagesController : ControllerBase
    {
        private readonly BadmintonStoreDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductImagesController(
            BadmintonStoreDbContext context,
            IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var images = await _context.ProductImages
                .Include(x => x.Product)
                .ToListAsync();

            return Ok(images);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var image = await _context.ProductImages
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.ImageId == id);

            if (image == null)
                return NotFound();

            return Ok(image);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromForm] int productId,
            IFormFile image)
        {
            var request = _httpContextAccessor.HttpContext!.Request;

            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.ProductId == productId);

            if (product == null)
            {
                return BadRequest(new
                {
                    message = "Product not found"
                });
            }

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

            var productImage = new ProductImage
            {
                ProductId = productId,
                ImageUrl = imageUrl
            };

            _context.ProductImages.Add(productImage);

            await _context.SaveChangesAsync();

            return Ok(productImage);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromForm] int? productId,
            IFormFile? image)
        {
            var request = _httpContextAccessor.HttpContext!.Request;

            var productImage = await _context.ProductImages
                .FirstOrDefaultAsync(x => x.ImageId == id);

            if (productImage == null)
                return NotFound();

            // update product id
            if (productId != null)
            {
                productImage.ProductId = productId.Value;
            }

            // update image
            if (image != null)
            {
                // delete old file
                if (!string.IsNullOrEmpty(productImage.ImageUrl))
                {
                    var oldFileName =
                        Path.GetFileName(productImage.ImageUrl);

                    var oldFilePath = Path.Combine(
                        _env.WebRootPath,
                        "images/products",
                        oldFileName);

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // save new file
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

                productImage.ImageUrl =
                    $"{request.Scheme}://{request.Host}/images/products/{fileName}";
            }

            await _context.SaveChangesAsync();

            return Ok(productImage);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var productImage = await _context.ProductImages
                .FirstOrDefaultAsync(x => x.ImageId == id);

            if (productImage == null)
                return NotFound();

            // delete image file
            if (!string.IsNullOrEmpty(productImage.ImageUrl))
            {
                var fileName =
                    Path.GetFileName(productImage.ImageUrl);

                var filePath = Path.Combine(
                    _env.WebRootPath,
                    "images/products",
                    fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.ProductImages.Remove(productImage);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Delete product image success"
            });
        }
    }
}