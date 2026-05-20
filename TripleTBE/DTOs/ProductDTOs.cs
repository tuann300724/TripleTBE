namespace TripleTBE.DTOs
{
    public class ProductDTOs
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public decimal Price { get; set; }
        public int? Stock { get; set; }
        public int? Status { get; set; }
        public string? Description { get; set; }

        public string? Thumbnail { get; set; }
       
        public List<string> Images { get; set; } = new();
        public string BrandName { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string Logo { get; set; } = null!;

        public string CategoryName { get; set; } = null!;
    }
}
