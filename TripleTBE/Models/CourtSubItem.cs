using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TripleTBE.Models;

public partial class CourtSubItem
{
    [Key]
    public int SubCourtId { get; set; }

    public int CourtId { get; set; }

    [StringLength(50)]
    public string SubCourtName { get; set; } = null!;

    public int? Status { get; set; } = 1;

    [ForeignKey("CourtId")]
    [InverseProperty("CourtSubItems")]
    [JsonIgnore]
    public virtual Court Court { get; set; } = null!;

    [InverseProperty("SubCourt")]
    [JsonIgnore]
    public virtual ICollection<CourtBookingDetail> CourtBookingDetails { get; set; } = new List<CourtBookingDetail>();
}