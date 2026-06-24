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
        private readonly BadmintonDbContext _context;
        private readonly IWebHostEnvironment _env; // Thêm biến này

        public UserProfileController(BadmintonDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env; // Gán giá trị từ DI
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
        public async Task<IActionResult> Update(int userId, [FromForm] UpdateProfileDto input)
        {
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
                return NotFound("Profile không tồn tại");

            // 1. Cập nhật các thông tin text cơ bản
            profile.FullName = input.FullName;
            profile.Phone = input.Phone;
            profile.Address = input.Address;

            // 2. Xử lý upload file ảnh nếu có dữ liệu truyền lên
            if (input.AvatarFile != null && input.AvatarFile.Length > 0)
            {
                var request = HttpContext.Request;

                // Tạo tên file độc nhất tránh trùng lặp
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(input.AvatarFile.FileName);

                // Đường dẫn vật lý để lưu file (Lưu vào wwwroot/images/avatars)
                var folderPath = Path.Combine(_env.WebRootPath, "images/avatars");

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var filePath = Path.Combine(folderPath, fileName);

                // Lưu file vào ổ cứng server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await input.AvatarFile.CopyToAsync(stream);
                }

                // Tạo chuỗi URL để client có thể truy cập qua trình duyệt
                var imageUrl = $"{request.Scheme}://{request.Host}/images/avatars/{fileName}";

                // Gán URL mới vào thuộc tính Avatar
                profile.Avatar = imageUrl;
            }

            // 3. Lưu tất cả thay đổi vào DB
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
    public class UpdateProfileDto
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public IFormFile? AvatarFile { get; set; } // Nhận file từ client
    }
}