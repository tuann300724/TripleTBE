using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TripleTBE.Models;

[Index(nameof(UserId), nameof(OrderDate),
    Name = "IX_Orders_User",
    IsDescending = new[] { false, true })]
public partial class Order
{
    [Key]
    public int OrderId { get; set; }

    public int UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? OrderDate { get; set; }

    [StringLength(50)]
    public string? OrderStatus { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TotalAmount { get; set; }

    // ================= RELATION =================

    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual User User { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        = new List<OrderDetail>();

    [JsonIgnore]
    public virtual ICollection<Payment> Payments { get; set; }
        = new List<Payment>();
}