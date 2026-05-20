using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TripleTBE.Models;

[Index("BrandName", Name = "UQ__Brands__2206CE9B004C8557", IsUnique = true)]
public partial class Brand
{
    [Key]
    public int BrandId { get; set; }

    [StringLength(100)]
    public string BrandName { get; set; } = null!;

    [StringLength(50)]
    public string? Country { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Logo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("Brand")]
 
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
