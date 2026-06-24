using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TripleTBE.Models;

public partial class CourtPayment
{
    [Key]
    public int PaymentId { get; set; }

    public int BookingId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [StringLength(50)]
    public string? PaymentMethod { get; set; }

    [StringLength(50)]
    public string? PaymentStatus { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PaymentDate { get; set; } = DateTime.Now;

    [ForeignKey("BookingId")]
    [InverseProperty("CourtPayments")]
    [JsonIgnore]
    public virtual CourtBooking Booking { get; set; } = null!;
}