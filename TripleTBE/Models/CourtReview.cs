using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TripleTBE.Models;

public partial class CourtReview
{
    [Key]
    public int ReviewId { get; set; }

    public int CourtId { get; set; }

    public int UserId { get; set; }

    public int Rating { get; set; }

    [StringLength(1000)]
    public string? Comment { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; } = DateTime.Now;

    // Chỉ giữ lại ForeignKey để định nghĩa khóa ngoại, bỏ InverseProperty
    [ForeignKey("CourtId")]
    [JsonIgnore]
    public virtual Court Court { get; set; } = null!;

    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}