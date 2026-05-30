using Microsoft.AspNetCore.Identity.Data;
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
        // =========================
        // LOGIN
        // =========================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Email == request.Email &&
                    x.PasswordHash == request.Password);

            if (user == null)
            {
                return BadRequest("Email hoặc mật khẩu không đúng");
            }

            // check status
            if (user.Status != "Active")
            {
                return BadRequest("Tài khoản đã bị khóa");
            }

            return Ok(user);
        }
        public class ChangePasswordRequest
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }

        public class ResetPasswordByAdminRequest
        {
            public string NewPassword { get; set; }
        }


        [HttpPost("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("User không tồn tại");

            // Kiểm tra mật khẩu cũ có khớp không
            if (user.PasswordHash != request.OldPassword)
            {
                return BadRequest("Mật khẩu cũ không chính xác");
            }

            // Kiểm tra mật khẩu mới không được trùng mật khẩu cũ
            if (request.OldPassword == request.NewPassword)
            {
                return BadRequest("Mật khẩu mới không được trùng với mật khẩu cũ");
            }

            // Cập nhật mật khẩu mới và thời gian update
            user.PasswordHash = request.NewPassword;
            user.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Đổi mật khẩu thành công" });
        }


        [HttpPost("{id}/reset-password")]
        public async Task<IActionResult> ResetPasswordByAdmin(int id, [FromBody] ResetPasswordByAdminRequest request)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("User không tồn tại");

            user.PasswordHash = request.NewPassword;
            user.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Admin đã đặt lại mật khẩu thành công" });
        }

        // =========================
        // LOGIN DTO
        // =========================
        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}