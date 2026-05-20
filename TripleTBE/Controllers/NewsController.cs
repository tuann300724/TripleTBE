using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripleTBE.Models;

namespace TripleTBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly BadmintonStoreDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NewsController(
            BadmintonStoreDbContext context,
            IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var news = await _context.News
                .Include(x => x.User)
                .ToListAsync();

            return Ok(news);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var news = await _context.News
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.NewsId == id);

            if (news == null)
                return NotFound();

            return Ok(news);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromForm] News news,
            IFormFile? thumbnail)
        {
            var request = _httpContextAccessor.HttpContext!.Request;

            // upload thumbnail
            if (thumbnail != null)
            {
                var fileName =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(thumbnail.FileName);

                var filePath = Path.Combine(
                    _env.WebRootPath,
                    "images/news",
                    fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await thumbnail.CopyToAsync(stream);
                }

                news.Thumbnail =
                    $"{request.Scheme}://{request.Host}/images/news/{fileName}";
            }

            news.CreatedDate = DateTime.Now;

            _context.News.Add(news);

            await _context.SaveChangesAsync();

            return Ok(news);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromForm] News updatedNews,
            IFormFile? thumbnail)
        {
            var request = _httpContextAccessor.HttpContext!.Request;

            var news = await _context.News
                .FirstOrDefaultAsync(x => x.NewsId == id);

            if (news == null)
                return NotFound();

            // update data
            if (!string.IsNullOrEmpty(updatedNews.Title))
            {
                news.Title = updatedNews.Title;
            }

            if (!string.IsNullOrEmpty(updatedNews.Content))
            {
                news.Content = updatedNews.Content;
            }

            if (updatedNews.UserId > 0)
            {
                news.UserId = updatedNews.UserId;
            }

            // update thumbnail
            if (thumbnail != null)
            {
                // delete old image
                if (!string.IsNullOrEmpty(news.Thumbnail))
                {
                    var oldImageName =
                        Path.GetFileName(news.Thumbnail);

                    var oldImagePath = Path.Combine(
                        _env.WebRootPath,
                        "images/news",
                        oldImageName);

                    if (System.IO.File.Exists(oldImagePath)
)
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // save new image
                var fileName =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(thumbnail.FileName);

                var filePath = Path.Combine(
                    _env.WebRootPath,
                    "images/news",
                    fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await thumbnail.CopyToAsync(stream);
                }

                news.Thumbnail =
                    $"{request.Scheme}://{request.Host}/images/news/{fileName}";
            }

            await _context.SaveChangesAsync();

            return Ok(news);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var news = await _context.News
                .FirstOrDefaultAsync(x => x.NewsId == id);

            if (news == null)
                return NotFound();

            // delete image
            if (!string.IsNullOrEmpty(news.Thumbnail))
            {
                var imageName =
                    Path.GetFileName(news.Thumbnail);

                var imagePath = Path.Combine(
                    _env.WebRootPath,
                    "images/news",
                    imageName);

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.News.Remove(news);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Delete news success"
            });
        }
    }
}