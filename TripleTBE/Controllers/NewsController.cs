using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripleTBE.Models;

namespace TripleTBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly BadmintonDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NewsController(
            BadmintonDbContext context,
            IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var news = await _context.News
                .Select(x => new
                {
                    x.NewsId,
                    x.Title,
                    x.Content,
                    x.Thumbnail,
                    x.CreatedDate,
                    x.UserId,
                    User = x.User != null ? new
                    {
                        x.User.UserId,
                        x.User.Email,
                        x.User.Role,
                        x.User.Status
                    } : null
                })
                .ToListAsync();

            return Ok(news);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var news = await _context.News
                .Where(x => x.NewsId == id)
                .Select(x => new
                {
                    x.NewsId,
                    x.Title,
                    x.Content,
                    x.Thumbnail,
                    x.CreatedDate,
                    x.UserId,
                    User = x.User != null ? new
                    {
                        x.User.UserId,
                        x.User.Email,
                        x.User.Role,
                        x.User.Status
                    } : null
                })
                .FirstOrDefaultAsync();

            if (news == null)
                return NotFound();

            return Ok(news);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromForm] CreateNewsDto dto, // Thay đổi ở đây
            IFormFile? thumbnail)
        {
            var request = _httpContextAccessor.HttpContext!.Request;

            // Ánh xạ từ DTO sang Entity chính
            var news = new News
            {
                Title = dto.Title,
                Content = dto.Content,
                UserId = dto.UserId,
                CreatedDate = DateTime.Now
            };

            // Tải tập tin ảnh lên
            if (thumbnail != null)
            {
                var folderPath = Path.Combine(_env.WebRootPath, "images/news");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath); // Tự động tạo thư mục nếu chưa có
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(thumbnail.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await thumbnail.CopyToAsync(stream);
                }

                news.Thumbnail = $"{request.Scheme}://{request.Host}/images/news/{fileName}";
            }

            _context.News.Add(news);
            await _context.SaveChangesAsync();

            return Ok(news);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromForm] UpdateNewsDto dto, // Thay đổi ở đây
            IFormFile? thumbnail)
        {
            var request = _httpContextAccessor.HttpContext!.Request;

            var news = await _context.News.FirstOrDefaultAsync(x => x.NewsId == id);
            if (news == null)
                return NotFound();

            // Cập nhật dữ liệu từ DTO nếu có truyền lên
            if (!string.IsNullOrEmpty(dto.Title))
            {
                news.Title = dto.Title;
            }

            if (!string.IsNullOrEmpty(dto.Content))
            {
                news.Content = dto.Content;
            }

            if (dto.UserId.HasValue && dto.UserId > 0)
            {
                news.UserId = dto.UserId.Value;
            }

            // Xử lý cập nhật ảnh đại diện
            if (thumbnail != null)
            {
                var folderPath = Path.Combine(_env.WebRootPath, "images/news");

                // Xóa ảnh cũ
                if (!string.IsNullOrEmpty(news.Thumbnail))
                {
                    var oldImageName = Path.GetFileName(news.Thumbnail);
                    var oldImagePath = Path.Combine(folderPath, oldImageName);

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Lưu ảnh mới
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(thumbnail.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await thumbnail.CopyToAsync(stream);
                }

                news.Thumbnail = $"{request.Scheme}://{request.Host}/images/news/{fileName}";
            }

            await _context.SaveChangesAsync();
            return Ok(news);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var news = await _context.News.FirstOrDefaultAsync(x => x.NewsId == id);
            if (news == null)
                return NotFound();

            // Xóa ảnh khi xóa bài viết
            if (!string.IsNullOrEmpty(news.Thumbnail))
            {
                var imageName = Path.GetFileName(news.Thumbnail);
                var imagePath = Path.Combine(_env.WebRootPath, "images/news", imageName);

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.News.Remove(news);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Delete news success" });
        }
    }
    public class CreateNewsDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int UserId { get; set; }
    }

    // DTO dùng cho việc Cập nhật
    public class UpdateNewsDto
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int? UserId { get; set; }
    }
}