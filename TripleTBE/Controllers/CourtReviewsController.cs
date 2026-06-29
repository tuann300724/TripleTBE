using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripleTBE.Models;

namespace TripleTBE.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CourtReviewsController : ControllerBase
{
    private readonly BadmintonDbContext _context;

    public CourtReviewsController(BadmintonDbContext context)
    {
        _context = context;
    }

    // GET: api/CourtReviews/Court/5 (Lấy toàn bộ review của 1 sân cụ thể)
    [HttpGet("Court/{courtId}")]
    public async Task<ActionResult<IEnumerable<CourtReview>>> GetCourtReviews(int courtId)
    {
        return await _context.CourtReviews
            .Where(r => r.CourtId == courtId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    // POST: api/CourtReviews (Gửi đánh giá mới + Tự động cập nhật lại Rating trung bình của sân)
    [HttpPost]
    public async Task<ActionResult<CourtReview>> PostCourtReview(CourtReview review)
    {
        if (review.Rating < 1 || review.Rating > 5)
        {
            return BadRequest(new { message = "Điểm đánh giá phải từ 1 đến 5 sao." });
        }

        review.CreatedAt = DateTime.Now;
        _context.CourtReviews.Add(review);
        await _context.SaveChangesAsync();

        // --- LOGIC TỰ ĐỘNG TÍNH LẠI ĐIỂM RATING TRUNG BÌNH CỦA SÂN ---
        var court = await _context.Courts.FindAsync(review.CourtId);
        if (court != null)
        {
            // Lấy tất cả điểm rating hiện tại của sân đó
            var ratings = await _context.CourtReviews
                .Where(r => r.CourtId == review.CourtId)
                .Select(r => r.Rating)
                .ToListAsync();

            if (ratings.Any())
            {
                // Tính trung bình cộng và làm tròn đến 1 chữ số thập phân (Ví dụ: 4.5)
                court.Rating = Math.Round((decimal)ratings.Average(), 1);
                _context.Entry(court).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }
        // -------------------------------------------------------------

        return CreatedAtAction(nameof(GetCourtReviews), new { courtId = review.CourtId }, review);
    }
}