using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripleTBE.Models;

namespace TripleTBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductVariantsController : ControllerBase
    {
        private readonly BadmintonDbContext _context;

        public ProductVariantsController(
            BadmintonDbContext context)
        {
            _context = context;
        }

        /* =========================================================
           GET ALL
        ========================================================= */

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var variants = await _context.ProductVariants
                .Include(v => v.Product)
                .Select(v => new
                {
                    v.VariantId,

                    v.ProductId,

                    ProductName = v.Product.ProductName,

                    v.Color,

                    v.Size,

                    v.Version,

                    v.SKU,

                    v.Price,

                    v.Stock,

          
                })
                .ToListAsync();

            return Ok(variants);
        }

        /* =========================================================
           GET BY ID
        ========================================================= */

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var variant = await _context.ProductVariants
                .Include(v => v.Product)
                .Where(v => v.VariantId == id)
                .Select(v => new
                {
                    v.VariantId,

                    v.ProductId,

                    ProductName = v.Product.ProductName,

                    v.Color,

                    v.Size,

                    v.Version,

                    v.SKU,

                    v.Price,

                    v.Stock,

      
                })
                .FirstOrDefaultAsync();

            if (variant == null)
            {
                return NotFound(new
                {
                    message = "Variant not found"
                });
            }

            return Ok(variant);
        }

        /* =========================================================
           CREATE
        ========================================================= */

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] ProductVariant variant)
        {
            var productExists = await _context.Products
                .AnyAsync(p => p.ProductId == variant.ProductId);

            if (!productExists)
            {
                return BadRequest(new
                {
                    message = "Product does not exist"
                });
            }

            // auto default
            if (variant.Stock < 0)
            {
                variant.Stock = 0;
            }

         
           

            _context.ProductVariants.Add(variant);

            await _context.SaveChangesAsync();

            return Ok( variant);
        }

        /* =========================================================
           UPDATE
        ========================================================= */

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] ProductVariant updatedVariant)
        {
            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(v => v.VariantId == id);

            if (variant == null)
            {
                return NotFound(new
                {
                    message = "Variant not found"
                });
            }

            // product
            if (updatedVariant.ProductId > 0)
            {
                variant.ProductId = updatedVariant.ProductId;
            }

            // color
            if (!string.IsNullOrWhiteSpace(updatedVariant.Color))
            {
                variant.Color = updatedVariant.Color;
            }

            // size
            if (!string.IsNullOrWhiteSpace(updatedVariant.Size))
            {
                variant.Size = updatedVariant.Size;
            }

            // version
            if (!string.IsNullOrWhiteSpace(updatedVariant.Version))
            {
                variant.Version = updatedVariant.Version;
            }

            // sku
            if (!string.IsNullOrWhiteSpace(updatedVariant.SKU))
            {
                variant.SKU = updatedVariant.SKU;
            }

            // price
            if (updatedVariant.Price > 0)
            {
                variant.Price = updatedVariant.Price;
            }

            // stock
            if (updatedVariant.Stock >= 0)
            {
                variant.Stock = updatedVariant.Stock;
            }

            // status
         

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Update variant success",
                data = variant
            });
        }

        /* =========================================================
           DELETE
        ========================================================= */

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(v => v.VariantId == id);

            if (variant == null)
            {
                return NotFound(new
                {
                    message = "Variant not found"
                });
            }

            _context.ProductVariants.Remove(variant);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Delete product variant success"
            });
        }
    }
}