namespace TripleTBE.DTOs
{
    public class ProductCreateDTO
    {
        public string ProductName { get; set; } = null!;

        public string? Description { get; set; }

        public int BrandId { get; set; }

        public int CategoryId { get; set; }

        public int? Status { get; set; }

        public IFormFile? Thumbnail { get; set; }

        public List<IFormFile>? Images { get; set; }
    }
}