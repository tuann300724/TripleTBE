using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TripleTBE.Models;

public partial class CourtBookingDetail
{
    [Key]
    public int BookingDetailId { get; set; }

    public int BookingId { get; set; }

    public int SubCourtId { get; set; }

    [Column(TypeName = "date")]
    public DateTime PlayDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [StringLength(50)]
    public string? CheckInStatus { get; set; } = "NotAssigned";

    [ForeignKey("BookingId")]
    [InverseProperty("CourtBookingDetails")]
    [JsonIgnore]
    public virtual CourtBooking Booking { get; set; } = null!;

    [ForeignKey("SubCourtId")]
    [InverseProperty("CourtBookingDetails")]
    public virtual CourtSubItem SubCourt { get; set; } = null!;
}