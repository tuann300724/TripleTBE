using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace TripleTBE.Models;

public partial class ProductVariant
{
    [Key]
    public int VariantId { get; set; }

    public int ProductId { get; set; }

    public string? Color { get; set; }

    public string? Size { get; set; }

    public string? Version { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int Stock { get; set; }

    public string? SKU { get; set; }

    // ===================== RELATION =====================

    [ForeignKey(nameof(ProductId))]
    [JsonIgnore]
    [ValidateNever]
    public virtual Product? Product { get; set; }

    [JsonIgnore]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        = new List<OrderDetail>();

    [JsonIgnore]
    public virtual ICollection<CartItem> CartItems { get; set; }
        = new List<CartItem>();
}