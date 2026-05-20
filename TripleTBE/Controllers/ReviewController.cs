using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using TripleTBE.Models;

namespace TripleTBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly BadmintonStoreDbContext _context;

        public ReviewController(BadmintonStoreDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL REVIEWS
        // =========================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetAll()
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
        }

        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetById(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.ReviewId == id);

            if (review == null)
                return NotFound("Review không tồn tại");

            return review;
        }

        // =========================
        // GET BY PRODUCT
        // =========================
        [HttpGet("product/{productId}")]
        public async Task<ActionResult> GetByProduct(int productId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .Include(r => r.User)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();

            var avgRating = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .AverageAsync(r => (double?)r.Rating) ?? 0;

            return Ok(new
            {
                data = reviews,
                averageRating = avgRating,
                total = reviews.Count
            });
        }

        // =========================
        // CREATE REVIEW
        // =========================
        [HttpPost]
        public async Task<ActionResult> Create(Review review)
        {
            // validate rating theo DB constraint (1-5)
            if (review.Rating < 1 || review.Rating > 5)
                return BadRequest("Rating phải từ 1 đến 5");

            review.ReviewDate = DateTime.Now;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(review);
        }

        // =========================
        // UPDATE REVIEW
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Review review)
        {
            if (id != review.ReviewId)
                return BadRequest("Id không hợp lệ");

            var existing = await _context.Reviews.FindAsync(id);

            if (existing == null)
                return NotFound("Review không tồn tại");

            existing.Rating = review.Rating;
            existing.Content = review.Content;
            existing.ReviewDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        // =========================
        // DELETE REVIEW
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
                return NotFound("Review không tồn tại");

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return Ok("Deleted");
        }
    }
}