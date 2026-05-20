using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TripleTBE.Models;

public partial class ProductImage
{
    [Key]
    public int ImageId { get; set; }

    public int ProductId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string ImageUrl { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("ProductImages")]
    [JsonIgnore]
    public virtual Product Product { get; set; } = null!;
}
