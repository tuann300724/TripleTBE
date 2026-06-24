using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TripleTBE.Models;

public partial class CourtBooking
{
    [Key]
    public int BookingId { get; set; }

    public int UserId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [StringLength(50)]
    public string? BookingStatus { get; set; } = "Pending";

    [StringLength(50)]
    public string? PaymentStatus { get; set; } = "Unpaid";

    [StringLength(500)]
    public string? Note { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? BookingDate { get; set; } = DateTime.Now;

    [ForeignKey("UserId")]
    [InverseProperty("CourtBookings")]
    [JsonIgnore]
    public virtual User User { get; set; } = null!;

    [InverseProperty("Booking")]
    public virtual ICollection<CourtBookingDetail> CourtBookingDetails { get; set; } = new List<CourtBookingDetail>();

    [InverseProperty("Booking")]
    public virtual ICollection<CourtPayment> CourtPayments { get; set; } = new List<CourtPayment>();
}