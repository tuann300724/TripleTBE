using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripleTBE.Models;

namespace TripleTBE.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CourtsController : ControllerBase
{
    private readonly BadmintonDbContext _context;

    public CourtsController(BadmintonDbContext context)
    {
        _context = context;
    }

    // GET: api/Courts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Court>>> GetCourts()
    {
        return await _context.Courts.Include(c => c.CourtSubItems).ToListAsync();
    }

    // GET: api/Courts/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Court>> GetCourt(int id)
    {
        var court = await _context.Courts
            .Include(c => c.CourtSubItems)
            .Include(c => c.CourtTimeSlots)
            .FirstOrDefaultAsync(c => c.CourtId == id);

        if (court == null) return NotFound(new { message = "Không tìm thấy sân cầu lông này." });
        return court;
    }

    // POST: api/Courts
    [HttpPost]
    public async Task<ActionResult<Court>> PostCourt(Court court)
    {
        _context.Courts.Add(court);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCourt), new { id = court.CourtId }, court);
    }

    // PUT: api/Courts/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCourt(int id, Court court)
    {
        if (id != court.CourtId) return BadRequest();

        court.UpdatedAt = DateTime.Now;
        _context.Entry(court).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Courts.Any(e => e.CourtId == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    // DELETE: api/Courts/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourt(int id)
    {
        var court = await _context.Courts.FindAsync(id);
        if (court == null) return NotFound();

        _context.Courts.Remove(court);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}