using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TripleTBE.Models;

public partial class Court
{
    [Key]
    public int CourtId { get; set; }

    [StringLength(200)]
    public string CourtName { get; set; } = null!;

    [StringLength(255)]
    public string Address { get; set; } = null!;

    public string? Description { get; set; }

    [StringLength(255)]
    public string? Thumbnail { get; set; }

    public bool? IsApproved { get; set; } = false;

    [StringLength(20)]
    public string? Status { get; set; } = "Active";

    public int OwnerId { get; set; }

    // --- THUỘC TÍNH MỚI BỔ SUNG ---
    [StringLength(50)]
    public string Latitude { get; set; } = null!;

    [StringLength(50)]
    public string Longitude { get; set; } = null!;

    [Column(TypeName = "decimal(2,1)")]
    public decimal? Rating { get; set; } = 0m;
    // ------------------------------

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; } = DateTime.Now;

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; } = DateTime.Now;

    [ForeignKey("OwnerId")]
    [InverseProperty("Courts")]
    [JsonIgnore]
    public virtual User Owner { get; set; } = null!;

    [InverseProperty("Court")]
    public virtual ICollection<CourtSubItem> CourtSubItems { get; set; } = new List<CourtSubItem>();

    [InverseProperty("Court")]
    public virtual ICollection<CourtTimeSlot> CourtTimeSlots { get; set; } = new List<CourtTimeSlot>();
    [InverseProperty("Court")]
    public virtual ICollection<CourtReview> CourtReviews { get; set; } = new List<CourtReview>();
}