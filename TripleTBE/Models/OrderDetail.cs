using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TripleTBE.Models;

public partial class OrderDetail
{
    [Key]
    public int OrderDetailId { get; set; }

    public int OrderId { get; set; }

    public int? VariantId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? VariantName { get; set; }

    public string? ProductImage { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public decimal? TotalPrice { get; set; }

    // ===================== RELATION =====================

    [ForeignKey(nameof(OrderId))]
    [JsonIgnore]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey(nameof(VariantId))]
    [JsonIgnore]
    public virtual ProductVariant? Variant { get; set; }
}