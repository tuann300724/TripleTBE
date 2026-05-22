namespace TripleTBE.DTOs
{
    public class ProductDTOs
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public string? Description { get; set; }

        public string? Thumbnail { get; set; }

        public int? Status { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public string BrandName { get; set; } = null!;

        public string Country { get; set; } = null!;

        public string Logo { get; set; } = null!;

        public string CategoryName { get; set; } = null!;

        public List<string> Images { get; set; } = new();
    }
}