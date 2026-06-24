using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TripleTBE.Models;

public partial class CourtTimeSlot
{
    [Key]
    public int SlotId { get; set; }

    public int CourtId { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [ForeignKey("CourtId")]
    [InverseProperty("CourtTimeSlots")]
    [JsonIgnore]
    public virtual Court Court { get; set; } = null!;
}