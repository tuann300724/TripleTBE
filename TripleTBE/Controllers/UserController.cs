using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using TripleTBE.Models;

namespace TripleTBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BadmintonStoreDbContext _context;

        public UserController(BadmintonStoreDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL USERS
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users
                .Include(u => u.UserProfile)
                .Select(u => new
                {
                    u.UserId,
                 
                    u.Email,
                    u.Role,
                    u.Status,
                    u.CreatedAt,
                    u.UpdatedAt,

                    Profile = u.UserProfile == null ? null : new
                    {
                        u.UserProfile.FullName,
                        u.UserProfile.Phone,
                        u.UserProfile.Address,
                        u.UserProfile.Avatar
                    }
                })
                .ToListAsync();

            return Ok(users);
        }

        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserProfile)
                .Where(u => u.UserId == id)
                .Select(u => new
                {
                    u.UserId,
                 
                    u.Email,
                    u.Role,
                    u.Status,
                    u.CreatedAt,
                    u.UpdatedAt,

                    Profile = u.UserProfile
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound("User không tồn tại");

            return Ok(user);
        }

        // =========================
        // CREATE USER
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            // check trùng username/email (DB bạn có UNIQUE)
            var exists = await _context.Users
                .AnyAsync(x =>  x.Email == user.Email);

            if (exists)
                return BadRequest("Email đã tồn tại");

            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        // =========================
        // UPDATE USER
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, User input)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("User không tồn tại");

           
            user.Email = input.Email;
            user.Role = input.Role;
            user.Status = input.Status;
            user.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(user);
        }

        // =========================
        // DELETE USER
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("User không tồn tại");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("Deleted");
        }

        // =========================
        // CHANGE STATUS (ACTIVE / BLOCK)
        // =========================
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromQuery] string status)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("User không tồn tại");

            user.Status = status;
            user.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(user);
        }
    }
}