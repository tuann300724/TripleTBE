using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripleTBE.Models;

namespace TripleTBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly BadmintonDbContext _context;

        public CategoriesController(BadmintonDbContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _context.Categories
                .ToListAsync();

            return Ok(categories);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(x => x.CategoryId == id);

            if (category == null)
                return NotFound();

            return Ok(category);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] Category category)
        {
            _context.Categories.Add(category);

            await _context.SaveChangesAsync();

            return Ok(category);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] Category updatedCategory)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(x => x.CategoryId == id);

            if (category == null)
                return NotFound();

            // update data
            if (!string.IsNullOrEmpty(updatedCategory.CategoryName))
            {
                category.CategoryName = updatedCategory.CategoryName;
            }

            if (!string.IsNullOrEmpty(updatedCategory.Description))
            {
                category.Description = updatedCategory.Description;
            }

            await _context.SaveChangesAsync();

            return Ok(category);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(x => x.CategoryId == id);

            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Delete category success"
            });
        }
    }
}