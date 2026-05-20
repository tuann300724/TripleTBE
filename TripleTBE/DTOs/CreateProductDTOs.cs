using Microsoft.AspNetCore.Http;

namespace TripleTBE.DTOs
{
    public class ProductCreateDTO
    {
        public string ProductName { get; set; } = null!;

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public string? Description { get; set; }

        public int BrandId { get; set; }

        public int CategoryId { get; set; }

        // ảnh đại diện
        public IFormFile? Thumbnail { get; set; }

        // nhiều ảnh
        public List<IFormFile>? Images { get; set; }
    }
}