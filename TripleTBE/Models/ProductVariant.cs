using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TripleTBE.Models;

public partial class ProductVariant
{
    [Key]
    public int VariantId { get; set; }

    public int ProductId { get; set; }

    [StringLength(50)]
    public string? Color { get; set; }

    [StringLength(20)]
    public string? Size { get; set; }

    [StringLength(100)]
    public string? Version { get; set; }

    public int? Quantity { get; set; }

    [InverseProperty("Variant")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [InverseProperty("Variant")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [ForeignKey("ProductId")]
    [InverseProperty("ProductVariants")]
    [JsonIgnore]
    public virtual Product Product { get; set; } = null!;
}
