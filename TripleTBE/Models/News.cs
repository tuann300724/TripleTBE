using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace TripleTBE.Models;

public partial class News
{
    [Key]
    public int NewsId { get; set; }

    [StringLength(255)]
    public string? Title { get; set; }

    public string? Content { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Thumbnail { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int UserId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("News")]
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
