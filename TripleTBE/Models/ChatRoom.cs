using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TripleTBE.Models;

public partial class ChatRoom
{
    [Key]
    public int RoomId { get; set; }

    public int CustomerId { get; set; }

    public int OwnerId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; } = DateTime.Now;

    [ForeignKey("CustomerId")]
    [InverseProperty("CustomerChatRooms")]
    [JsonIgnore]
    public virtual User Customer { get; set; } = null!;

    [ForeignKey("OwnerId")]
    [InverseProperty("OwnerChatRooms")]
    [JsonIgnore]
    public virtual User Owner { get; set; } = null!;

    [InverseProperty("Room")]
    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}