using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using TripleTBE.Models;

namespace TripleTBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly BadmintonStoreDbContext _context;

        public UserProfileController(BadmintonStoreDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL PROFILES
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var profiles = await _context.UserProfiles
                .Include(p => p.User)
                .Select(p => new
                {
                    p.UserId,
                    p.FullName,
                    p.Phone,
                    p.Address,
                    p.Avatar,

                    Username = p.User.Username,
                    Email = p.User.Email,
                    Role = p.User.Role
                })
                .ToListAsync();

            return Ok(profiles);
        }

        // =========================
        // GET BY USER ID
        // =========================
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var profile = await _context.UserProfiles
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .Select(p => new
                {
                    p.UserId,
                    p.FullName,
                    p.Phone,
                    p.Address,
                    p.Avatar,

                    User = new
                    {
                        p.User.Username,
                        p.User.Email,
                        p.User.Role,
                        p.User.Status
                    }
                })
                .FirstOrDefaultAsync();

            if (profile == null)
                return NotFound("Profile không tồn tại");

            return Ok(profile);
        }

        // =========================
        // CREATE PROFILE
        // (phải có UserId trước)
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(UserProfile profile)
        {
            var userExists = await _context.Users
                .AnyAsync(u => u.UserId == profile.UserId);

            if (!userExists)
                return BadRequest("User không tồn tại");

            var existsProfile = await _context.UserProfiles
                .AnyAsync(p => p.UserId == profile.UserId);

            if (existsProfile)
                return BadRequest("User đã có profile rồi");

            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();

            return Ok(profile);
        }

        // =========================
        // UPDATE PROFILE
        // =========================
        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(int userId, UserProfile input)
        {
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
                return NotFound("Profile không tồn tại");

            profile.FullName = input.FullName;
            profile.Phone = input.Phone;
            profile.Address = input.Address;
            profile.Avatar = input.Avatar;

            await _context.SaveChangesAsync();

            return Ok(profile);
        }

        // =========================
        // DELETE PROFILE
        // =========================
        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(int userId)
        {
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
                return NotFound("Profile không tồn tại");

            _context.UserProfiles.Remove(profile);
            await _context.SaveChangesAsync();

            return Ok("Deleted");
        }
    }
}