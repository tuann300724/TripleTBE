using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TripleTBE.Models;

public partial class ChatMessage
{
    [Key]
    public long MessageId { get; set; }

    public int RoomId { get; set; }

    public int SenderId { get; set; }

    public string MessageContent { get; set; } = null!;

    public bool? IsRead { get; set; } = false;

    [Column(TypeName = "datetime")]
    public DateTime? SentAt { get; set; } = DateTime.Now;

    [ForeignKey("RoomId")]
    [InverseProperty("ChatMessages")]
    [JsonIgnore]
    public virtual ChatRoom Room { get; set; } = null!;

    [ForeignKey("SenderId")]
    [InverseProperty("ChatMessages")]
    [JsonIgnore]
    public virtual User Sender { get; set; } = null!;
}