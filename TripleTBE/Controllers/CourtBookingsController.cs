using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripleTBE.Models;

namespace TripleTBE.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CourtBookingsController : ControllerBase
{
    private readonly BadmintonDbContext _context;

    public CourtBookingsController(BadmintonDbContext context)
    {
        _context = context;
    }

    // GET: api/CourtBookings/User/2
    [HttpGet("User/{userId}")]
    public async Task<ActionResult<IEnumerable<CourtBooking>>> GetUserBookings(int userId)
    {
        return await _context.CourtBookings
            .Where(b => b.UserId == userId)
            .Include(b => b.CourtBookingDetails)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();
    }

    // GET: api/CourtBookings/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CourtBooking>> GetBooking(int id)
    {
        var booking = await _context.CourtBookings
            .Include(b => b.CourtBookingDetails)
            .Include(b => b.CourtPayments)
            .FirstOrDefaultAsync(b => b.BookingId == id);

        if (booking == null) return NotFound();
        return booking;
    }

    // POST: api/CourtBookings
    [HttpPost]
    public async Task<ActionResult<CourtBooking>> CreateBooking(CourtBooking booking)
    {
        // 1. Kiểm tra xem các ca trong chi tiết (Details) có bị ai đặt trước chưa
        foreach (var detail in booking.CourtBookingDetails)
        {
            bool isSlotTaken = await _context.CourtBookingDetails.AnyAsync(cbd =>
                cbd.SubCourtId == detail.SubCourtId &&
                cbd.PlayDate == detail.PlayDate &&
                cbd.StartTime == detail.StartTime);

            if (isSlotTaken)
            {
                return BadRequest(new
                {
                    message = $"Sân nhỏ ID {detail.SubCourtId} vào ngày {detail.PlayDate:dd/MM/yyyy} khung giờ {detail.StartTime} đã bị người khác đặt mất!"
                });
            }
        }

        // 2. Nếu hợp lệ, tiến hành lưu hóa đơn đặt sân
        booking.BookingDate = DateTime.Now;
        _context.CourtBookings.Add(booking);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, booking);
    }

    // PUT: api/CourtBookings/5/Status (Cập nhật trạng thái đơn đặt sân)
    [HttpPut("{id}/Status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
    {
        var booking = await _context.CourtBookings.FindAsync(id);
        if (booking == null) return NotFound();

        booking.BookingStatus = status;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}