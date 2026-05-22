using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace TripleTBE.Models;

public partial class UserProfile
{
    [Key]
    public int UserId { get; set; }

    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [StringLength(15)]
    [Unicode(false)]
    public string? Phone { get; set; }

    [StringLength(255)]
    public string? Address { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Avatar { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserProfile")]
    [JsonIgnore]
    public virtual User? User { get; set; } = null!;
}
