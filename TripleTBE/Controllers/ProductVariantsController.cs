using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripleTBE.Models;

namespace TripleTBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductVariantsController : ControllerBase
    {
        private readonly BadmintonStoreDbContext _context;

        public ProductVariantsController(BadmintonStoreDbContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var variants = await _context.ProductVariants
                .Include(x => x.Product)
                .ToListAsync();

            return Ok(variants);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var variant = await _context.ProductVariants
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.VariantId == id);

            if (variant == null)
                return NotFound();

            return Ok(variant);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] ProductVariant variant)
        {
            _context.ProductVariants.Add(variant);

            await _context.SaveChangesAsync();

            return Ok(variant);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] ProductVariant updatedVariant)
        {
            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(x => x.VariantId == id);

            if (variant == null)
                return NotFound();

            // update product
            if (updatedVariant.ProductId > 0)
            {
                variant.ProductId = updatedVariant.ProductId;
            }

            // update color
            if (!string.IsNullOrEmpty(updatedVariant.Color))
            {
                variant.Color = updatedVariant.Color;
            }

            // update size
            if (!string.IsNullOrEmpty(updatedVariant.Size))
            {
                variant.Size = updatedVariant.Size;
            }

            // update version
            if (!string.IsNullOrEmpty(updatedVariant.Version))
            {
                variant.Version = updatedVariant.Version;
            }

            // update quantity
            if (updatedVariant.Quantity != null)
            {
                variant.Quantity = updatedVariant.Quantity;
            }

            await _context.SaveChangesAsync();

            return Ok(variant);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(x => x.VariantId == id);

            if (variant == null)
                return NotFound();

            _context.ProductVariants.Remove(variant);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Delete product variant success"
            });
        }
    }
}