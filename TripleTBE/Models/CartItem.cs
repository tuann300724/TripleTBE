using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace TripleTBE.Models;

public partial class CartItem
{
    [Key]
    public int CartItemId { get; set; }

    public int CartId { get; set; }

    public int? VariantId { get; set; }

    public int? Quantity { get; set; }

    [ForeignKey("CartId")]
    [InverseProperty("CartItems")]
    [JsonIgnore]
    public virtual Cart Cart { get; set; } = null!;

    [ForeignKey("VariantId")]
    [InverseProperty("CartItems")]
    [JsonIgnore]
    public virtual ProductVariant? Variant { get; set; }
}
